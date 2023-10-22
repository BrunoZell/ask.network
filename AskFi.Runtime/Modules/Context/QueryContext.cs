using System.Diagnostics;
using AskFi.Runtime.Internal;
using AskFi.Runtime.Persistence;
using AskFi.Runtime.Platform;
using Microsoft.FSharp.Core;
using static AskFi.Runtime.DataModel;
using static AskFi.Sdk;

namespace AskFi.Runtime.Modules.Context;

/// <summary>
/// This represents a block of time series data, queryable through the SDKs query interface.
/// The data availability time frame must be defined upfront with an in-memory index required at load time.
/// </summary>
public sealed class QueryContext : IContextQueries
{
    private readonly ContentId _latestContextSequenceHead;
    private readonly ContextIndex _index;
    private readonly IPlatformPersistence _persistence;

    public static QueryContext Load(
        ContentId latestContextSequenceHead,
        DateTime earliestAvailableTimestamp,
        IPlatformPersistence persistence)
    {
        using (NoSynchronizationContextScope.Enter()) {
            var index = ContextIndex.Build(
                latestContextSequenceHead,
                earliestAvailableTimestamp,
                persistence).Result;

            return new(latestContextSequenceHead, index, persistence);
        }
    }

    public DateTime EarliestObservation => _index.EarliestObservation;
    public DateTime LatestObservation => _index.LatestObservation;

    private QueryContext(
        ContentId latestContextSequenceHead,
        ContextIndex index,
        IPlatformPersistence persistence)
    {
        _latestContextSequenceHead = latestContextSequenceHead;
        _index = index;
        _persistence = persistence;
    }

    public FSharpOption<CapturedObservation<TPercept>> latest<TPercept>()
    {
        ContextSequenceHead contextSequenceHead;

        using (NoSynchronizationContextScope.Enter()) {
            contextSequenceHead = _persistence.Get<ContextSequenceHead>(_latestContextSequenceHead).Result;
        }

        while (true) {
            if (contextSequenceHead is not ContextSequenceHead.Context context) {
                throw new InvalidOperationException($"No observations of type {typeof(TPercept).FullName} the context sequence. Reached the identity node of the context sequence. No more observations to inspect.");
            }

            if (context.Node.Observation.PerceptType == typeof(TPercept)) {
                // Percept type fits. Load and return.
                using (NoSynchronizationContextScope.Enter()) {
                    return _persistence.Get<CapturedObservation<TPercept>>(context.Node.Observation.Observation).Result;
                }
            } else {
                // Look for immediate predecessor.
                using (NoSynchronizationContextScope.Enter()) {
                    contextSequenceHead = _persistence.Get<ContextSequenceHead>(context.Node.Previous).Result;
                }
            }
        }
    }

    public IEnumerable<CapturedObservation<TPercept>> inTimeRange<TPercept>(DateTime from, DateTime to)
    {
        foreach (var nodeCid in _index.ForwardWalk(from, to)) {
            ContextSequenceHead contextSequenceHead;

            using (NoSynchronizationContextScope.Enter()) {
                contextSequenceHead = _persistence.Get<ContextSequenceHead>(nodeCid).Result;
            }

            if (contextSequenceHead is not ContextSequenceHead.Context context) {
                Debug.Fail("Indexer should only index actual context nodes, no context sequence identities.");
                yield break;
            }

            if (context.Node.Observation.PerceptType != typeof(TPercept)) {
                // Percept type not as requested. Skip.
            }

            // Load observation from context sequence node.
            CapturedObservation<TPercept> observation;

            using (NoSynchronizationContextScope.Enter()) {
                observation = _persistence.Get<CapturedObservation<TPercept>>(context.Node.Observation.Observation).Result;
            }

            yield return observation;
        }
    }

    public IEnumerable<(FSharpOption<CapturedObservation<TPercept1>>, FSharpOption<CapturedObservation<TPercept2>>)> inTimeRange<TPercept1, TPercept2>(DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }
}
