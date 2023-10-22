using System.Threading.Channels;
using AskFi.Runtime.Messages;
using AskFi.Runtime.Persistence;
using AskFi.Runtime.Platform;
using Microsoft.FSharp.Collections;
using static AskFi.Runtime.DataModel;

namespace AskFi.Runtime.Modules.Perspective;

/// <summary>
/// Turns NewObservation messages into NewKnowledgeBase messages given that they indeed contained new information.
/// </summary>
internal class ObservationIntegrationModule
{
    private readonly ChannelReader<NewObservation> _input;
    private readonly Channel<NewKnowledgeBase> _output;
    private readonly IPlatformPersistence _persistence;

    public ChannelReader<NewKnowledgeBase> Output => _output.Reader;

    public ObservationIntegrationModule(
        IPlatformPersistence persistence,
        ChannelReader<NewObservation> input)
    {
        _output = Channel.CreateUnbounded<NewKnowledgeBase>();
        _persistence = persistence;
        _input = input;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        // Local pool starts out with an empty pool
        var localHeaviestKnowledgeBase = new KnowledgeBase(observations: null, actions: null);
        var localHeaviestKnowledgeBaseCid = _persistence.Cid(localHeaviestKnowledgeBase);

        await foreach (var observation in _input.ReadAllAsync(cancellationToken)) {
            // Transform incoming observation into observation pool to merge
            var incomingKnowledgeBase = new KnowledgeBase(
                observations: new FSharpMap<ContentId, FSharpList<ContentId>>(
                    elements: new[] { new Tuple<ContentId, FSharpList<ContentId>>(
                        item1: observation.Identity,
                        item2: new FSharpList<ContentId>(observation.Head, FSharpList<ContentId>.Empty)) }),
                actions: null);

            // Merge incoming pool with local pool, creating a new heaviest local pool
            var mergedKnowledgeBase = await KnowledgeBaseMerge.Join(localHeaviestKnowledgeBase, incomingKnowledgeBase, _persistence);
            var mergedKnowledgeBaseCid = _persistence.Cid(mergedKnowledgeBase);

            if (!mergedKnowledgeBaseCid.Raw.Equals(localHeaviestKnowledgeBaseCid.Raw)) {
                // Found new information.
                localHeaviestKnowledgeBase = mergedKnowledgeBase;
                localHeaviestKnowledgeBaseCid = mergedKnowledgeBaseCid;

                // Share it with others.
                var newKnowledgeBase = new NewKnowledgeBase(mergedKnowledgeBaseCid);
                await _output.Writer.WriteAsync(newKnowledgeBase);
            }
        }
    }
}
