using System.Threading.Channels;
using AskFi.Runtime.Messages;
using AskFi.Runtime.Platform;
using static AskFi.Runtime.DataModel;

namespace AskFi.Runtime.Modules.Observation;

/// <summary>
/// A single instance to pipe observations from all <see cref="ObserverInstance"/> (<see cref="Sdk.IObserver{Percept}"/>)
/// through in an async way. This is to sequence it in a first-come-first-server way. After new observations
/// are written to the <see cref="ObserverGroup"/>, their observation order is set, and all conclusions derived are
/// deterministic (and thus reproducible) thereafter.
/// </summary>
internal sealed class ObserverGroup : IAsyncDisposable
{
    private readonly IReadOnlyCollection<ObserverInstance> _observers;
    private readonly Channel<NewInternalObservation> _incomingObservations;
    private readonly ChannelWriter<NewObservation> _output;
    private readonly IPlatformPersistence _persistence;
    private readonly CancellationTokenSource _cancellation;
    private readonly Task _observationSequencing;

    private ObserverGroup(
        IReadOnlyCollection<ObserverInstance> observers,
        Channel<NewInternalObservation> incomingObservations,
        IPlatformPersistence persistence,
        ChannelWriter<NewObservation> output,
        CancellationTokenSource cancellation)
    {
        _observers = observers;
        _incomingObservations = incomingObservations;
        _output = output;
        _persistence = persistence;
        _cancellation = cancellation;
        _observationSequencing = SequenceObservations();
    }

    public static ObserverGroup StartNew(
        /*IObserver<'Percept> (where Percept = .Key)*/ IReadOnlyDictionary<Type, object> observers,
        IPlatformPersistence persistence,
        ChannelWriter<NewObservation> output,
        CancellationToken cancellationToken)
    {
        var distinctObserverInstances = observers.Values
            .Distinct(ReferenceEqualityComparer.Instance)
            .Count();

        if (distinctObserverInstances != observers.Count) {
            throw new ArgumentException("Each distinct IObserver-instance can only be operated once by the Observer Module. The passed list contains double entries.");
        }

        var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var incomingObservations = Channel.CreateUnbounded<NewInternalObservation>();
        var observerInstances = observers
            .Select(o => ObserverInstance.StartNew(o.Key, o.Value, incomingObservations, persistence, linkedCancellation.Token))
            .ToArray();

        return new ObserverGroup(
            observerInstances,
            incomingObservations,
            persistence,
            output,
            linkedCancellation);
    }

    /// <summary>
    /// Long-running background task that reads all new observations pooled across all IObserver instances and
    /// sequences them into a <see cref="ObservationSequenceHead"/>.
    /// This sequence introduces a relative ordering in time between observations of the same Observer Group.
    /// </summary>
    private async Task SequenceObservations()
    {
        var observationSequence = ObservationSequenceHead.NewIdentity(nonce: 0ul);
        var observationSequenceIdentity = _persistence.Cid(observationSequence);
        var observationSequenceCid = observationSequenceIdentity;

        // Sequentially receives all observations from IObserver-instances in this group as they happen.
        await foreach (var newInternalObservation in _incomingObservations.Reader.ReadAllAsync(_cancellation.Token)) {
            observationSequence = ObservationSequenceHead.NewObservation(new ObservationSequenceNode(
                previous: observationSequenceCid,
                capture: newInternalObservation.CapturedObservation));

            // Perf: Generate CID locally and upload in the background
            observationSequenceCid = await _persistence.Put(observationSequence);

            await _output.WriteAsync(new NewObservation(
                identity: observationSequenceIdentity,
                head: observationSequenceCid));
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cancellation.Cancel();

        // To throw and observe possible exceptions.
        await Task.WhenAll(
            _observers.Select(o => o.DisposeAsync().AsTask()).Append(_observationSequencing).ToArray());

        _cancellation.Dispose();
    }
}
