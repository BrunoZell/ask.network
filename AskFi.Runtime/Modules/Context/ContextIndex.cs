using System.Diagnostics;
using AskFi.Runtime.Internal;
using AskFi.Runtime.Persistence;
using AskFi.Runtime.Platform;
using static AskFi.Runtime.DataModel;

namespace AskFi.Runtime.Modules.Context;

internal sealed class ContextIndex
{
    private readonly SortedDictionary<DateTime, ContentId> _timestampNodeMap;

    public DateTime EarliestObservation { get; }
    public DateTime LatestObservation { get; }

    private ContextIndex(
        SortedDictionary<DateTime, ContentId> timestampNodeMap,
        DateTime earliestObservation,
        DateTime latestObservation)
    {
        _timestampNodeMap = timestampNodeMap;
        EarliestObservation = earliestObservation;
        LatestObservation = latestObservation;
    }

    public IEnumerable<ContentId> ForwardWalk(DateTime from, DateTime to)
    {
        foreach (var (timestamp, node) in _timestampNodeMap) {
            if (timestamp >= from) {
                yield return node;
            } else if (timestamp >= to) {
                yield break;
            }
        }
    }

    public static async ValueTask<ContextIndex> Build(ContentId contextSequenceHeadCid, DateTime minimumAvailableTimestamp, IPlatformPersistence persistence)
    {
        var currentContextSequenceHeadCid = contextSequenceHeadCid;
        var timestamps = new SortedDictionary<DateTime, ContentId>();
        var earliest = default(DateTime?);
        var latest = default(DateTime?);
        var contextSequenceHead = persistence.Get<ContextSequenceHead>(currentContextSequenceHeadCid).Result;

        while (true) {
            if (contextSequenceHead is not ContextSequenceHead.Context context) {
                // Found identity node. No more observations hereafter
                Debug.Assert(contextSequenceHead is ContextSequenceHead.Identity, $"{nameof(ContextSequenceHead)} should have only two union cases: Identity | Context");

                if (!latest.HasValue) {
                    // Not a single observation loaded. Instead of returning an empty index, throw for now.
                    throw new InvalidOperationException("Passed contxt sequence head cid was not valid. Check that you passed the cid of the head, not the cid of the initial identity node.");
                } else {
                    // At least one observation loaded
                    return new ContextIndex(timestamps, earliest.Value, latest.Value);
                }
            }

            var timestamp = context.Node.Observation.At;
            timestamps.Add(timestamp, currentContextSequenceHeadCid);

            // Gets set to the timestamp of the very first analyzed node, which is the highest
            // timestamp in this index (assuming a valid context sequence).
            if (!latest.HasValue) {
                latest = timestamp;
            }

            earliest = timestamp;

            if (timestamp < minimumAvailableTimestamp) {
                // Context nodes timestamp is earlier than the requested availability time range.
                // Stop indexing here and return. Older data is not relevant for the caller.
                return new ContextIndex(timestamps, earliestObservation: timestamp, latestObservation: latest.Value);
            } else {
                // Load previous context sequence node
                using (NoSynchronizationContextScope.Enter()) {
                    currentContextSequenceHeadCid = context.Node.Previous;
                    contextSequenceHead = persistence.Get<ContextSequenceHead>(currentContextSequenceHeadCid).Result;
                }
            }
        }
    }
}
