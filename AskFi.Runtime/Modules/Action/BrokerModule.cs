using System.Threading.Channels;
using AskFi.Runtime.Messages;
using AskFi.Runtime.Platform;
using static AskFi.Runtime.DataModel;

namespace AskFi.Runtime.Modules.Execution;
public class BrokerModule
{
    private readonly BrokerMultiplexer _brokerMultiplexer;
    private readonly IPlatformPersistence _persistence;
    private readonly ChannelReader<NewDecision> _input;
    private readonly Channel<ActionExecuted> _output;

    public ChannelReader<ActionExecuted> Output => _output.Reader;

    public BrokerModule(
        IReadOnlyDictionary<Type, object> brokers,
        IPlatformPersistence persistence,
        ChannelReader<NewDecision> input)
    {
        _brokerMultiplexer = new BrokerMultiplexer(brokers, persistence);
        _persistence = persistence;
        _input = input;
        _output = Channel.CreateUnbounded<ActionExecuted>();
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        var actionSequence = ActionSequenceHead.NewIdentity(nonce: 0ul);
        var actionSequenceIdentity = await _persistence.Put(actionSequence);
        var actionSequenceCid = actionSequenceIdentity;

        await foreach (var decision in _input.ReadAllAsync(cancellationToken)) {
            var executionTasks = new List<Task<ActionExecutionResult>>();
            var executionActionMapping = new Dictionary<Task<ActionExecutionResult>, DataModel.Action>();

            var decisionHead = await _persistence.Get<DecisionSequenceHead>(decision.Head);
            var decisionNode = decisionHead as DecisionSequenceHead.Decision;
            var actionSet = await _persistence.Get<ActionSet>(decisionNode.Item.ActionSet);

            // Assign all action initiations an id and send to according broker instance
            foreach (var action in actionSet.Actions) {
                if (_brokerMultiplexer.TryStartActionExecution(action, out var actionExecution)) {
                    // Broker available
                    executionTasks.Add(actionExecution);
                    executionActionMapping.Add(actionExecution, action);
                } else {
                    // No matching IBroker instance available. Do nothing.
                }
            }

            // Wait for all individual action initiations to complete.
            // Then immediately build the execution sequence.
            while (executionTasks.Count > 0) {
                var completed = await Task.WhenAny(executionTasks);
                var action = executionActionMapping[completed];
                var result = await completed;
                executionTasks.Remove(completed);

                // Write and publish execution sequence
                actionSequence = ActionSequenceHead.NewAction(new ActionSequenceNode(
                    previous: actionSequenceCid,
                    executed: action,
                    result: result));

                actionSequenceCid = await _persistence.Put(actionSequence);

                await _output.Writer.WriteAsync(new ActionExecuted(
                    identity: actionSequenceIdentity,
                    head: actionSequenceCid));
            }
        }
    }
}
