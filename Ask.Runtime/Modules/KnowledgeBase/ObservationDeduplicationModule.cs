using System.Threading.Channels;
using Ask.Host.Persistence;

namespace Ask.Runtime.Modules.Perspective;

/// <summary>
/// Deduplicates NewKnowledgeBase messages by only emitting those that added information to the locally ever growing pool.
/// </summary>
internal class ObservationDeduplicationModule
{
    private readonly ChannelReader<NewKnowledgeBase> _input;
    private readonly Channel<NewKnowledgeBase> _output;
    private readonly IHostPersistence _persistence;

    public ChannelReader<NewKnowledgeBase> Output => _output.Reader;

    public ObservationDeduplicationModule(
        IHostPersistence persistence,
        ChannelReader<NewKnowledgeBase> input)
    {
        _output = Channel.CreateUnbounded<NewKnowledgeBase>();
        _persistence = persistence;
        _input = input;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        // Local pool starts out with an empty pool
        var localHeaviestObservationPool = new KnowledgeBase(
            observations: null,
            actions: null);

        var localHeaviestObservationPoolCid = _persistence.Cid(localHeaviestObservationPool);

        await foreach (var pool in _input.ReadAllAsync(cancellationToken)) {
            // Merge incoming pool with local pool, creating a new heaviest local pool
            var incomingKnowledgeBase = await _persistence.Store<KnowledgeBase>(pool.KnowledeBase);
            var mergedKnowledgeBase = await KnowledgeBaseMerge.Join(localHeaviestObservationPool, incomingKnowledgeBase, _persistence);
            var mergedKnowledgeBaseCid = _persistence.Cid(mergedKnowledgeBase);

            if (!mergedKnowledgeBaseCid.Raw.Equals(localHeaviestObservationPoolCid.Raw)) {
                // Found new information.
                localHeaviestObservationPool = mergedKnowledgeBase;
                localHeaviestObservationPoolCid = mergedKnowledgeBaseCid;

                // Share it with others.
                var newObservationPool = new NewKnowledgeBase(mergedKnowledgeBaseCid);
                await _output.Writer.WriteAsync(newObservationPool);
            }
        }
    }
}
