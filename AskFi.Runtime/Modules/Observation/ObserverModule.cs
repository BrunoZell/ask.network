using System.Threading.Channels;
using AskFi.Runtime.Messages;
using AskFi.Runtime.Platform;

namespace AskFi.Runtime.Modules.Observation;

/// <summary>
/// A single instance to pipe observations from all <see cref="ObserverInstance"/> (<see cref="Sdk.IObserver{Percept}"/>)
/// through in an async way. This is to sequence it in a first-come-first-server way. After new observations
/// are written to the <see cref="ObserverModule"/>, their observation order is set, and all conclusions derived are
/// deterministic (and thus reproducible) thereafter.
/// </summary>
internal sealed class ObserverModule
{
    private readonly IReadOnlyDictionary<Type, object> _observers;
    private readonly IPlatformPersistence _persistence;
    private readonly Channel<NewObservation> _output;

    public ChannelReader<NewObservation> Output => _output.Reader;

    public ObserverModule(
        /*IObserver<'Percept> (where Percept = .Key)*/ IReadOnlyDictionary<Type, object> observers,
        IPlatformPersistence persistence)
    {

        _observers = observers;
        _persistence = persistence;
        _output = Channel.CreateUnbounded<NewObservation>();
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        await using var observerGroupTask = ObserverGroup.StartNew(_observers, _persistence, _output.Writer, cancellationToken);

        try {
            // Wait until canceled.
            await Task.Delay(0, cancellationToken);
        } catch (OperationCanceledException) {
            // Cancellation expected.
        }

        // dispose observer group
    }
}
