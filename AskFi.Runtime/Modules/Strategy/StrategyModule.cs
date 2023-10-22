using System.Threading.Channels;
using AskFi.Runtime.Messages;
using AskFi.Runtime.Persistence;
using AskFi.Runtime.Platform;
using static AskFi.Runtime.DataModel;
using static AskFi.Sdk;

namespace AskFi.Runtime.Modules.Strategy;

internal class StrategyModule
{
    private readonly Func<Reflection, Sdk.Context, Decision> _strategy;
    private readonly IPlatformPersistence _persistence;
    private readonly ChannelReader<NewKnowledgeBase> _input;
    private readonly Channel<NewDecision> _output;

    public ChannelReader<NewDecision> Output => _output;

    public StrategyModule(
        Func<Reflection, Sdk.Context, Decision> strategy,
        IPlatformPersistence persistence,
        ChannelReader<NewKnowledgeBase> input)
    {
        _strategy = strategy;
        _persistence = persistence;
        _input = input;
        _output = Channel.CreateUnbounded<NewDecision>();
    }

    public async Task Run(CancellationToken sessionShutdown)
    {
        var decisionSequence = DecisionSequenceHead.NewStart(new DecisionSequenceStart(
            strategy: ContentId.Zero, // Todo: reference transferable implementation of _strategy
            firstContext: ContentId.Zero)); // Todo: Link virtual start timestamp as a user defined knowledge base

        var decisionSequenceIdentity = await _persistence.Put(decisionSequence);
        var decisionSequenceCid = decisionSequenceIdentity;

        await foreach (var pool in _input.ReadAllAsync(sessionShutdown)) {
            var context = new Sdk.Context(query: null);
            var reflection = new Reflection(query: null);
            var decision = _strategy(reflection, context); // evaluating a strategy runs all required queries

            if (decision is not Decision.Initiate initiate) {
                // Do no more accounting if the decision is to do nothing.
                continue;
            }

            // Strategy decided to do something.

            // Build action set
            var actions = new List<DataModel.Action>();
            foreach (var initiative in initiate.Initiatives) {
                var actionCid = await _persistence.Put(initiative.Action);
                actions.Add(new DataModel.Action(initiative.Type, actionCid));
            }

            var actionSet = new ActionSet(actions.ToArray());
            var actionSetCid = await _persistence.Put(actionSet);

            // Append this action set to the decision sequence
            decisionSequence = DecisionSequenceHead.NewDecision(new DecisionSequenceNode(
                previous: decisionSequenceCid,
                actionSet: actionSetCid));

            decisionSequenceCid = await _persistence.Put(decisionSequence);

            await _output.Writer.WriteAsync(new NewDecision(
                identity: decisionSequenceIdentity,
                head: decisionSequenceCid));
        }
    }
}
