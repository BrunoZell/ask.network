using AskFi.Runtime.Platform;
using static AskFi.Runtime.DataModel;

namespace AskFi.Runtime.Modules.Perspective;

#if !KNOWLEDGE_BASE

public static class KnowledgeBaseMerge
{
    public static ValueTask<KnowledgeBase> Join(KnowledgeBase a, KnowledgeBase b, IPlatformPersistence persistence)
    {
        throw new NotImplementedException();
    }
}

#else

internal class ObservationPoolJoin
{
    private ObservationPoolJoin(
        ImmutableHashSet<ContentId> observationSet,
        ImmutableSortedDictionary<DateTime, ContentId> timestampMap)
    {
        _allIncludedCapturedObservations = observationSet;
        _absouteTimestampMap = timestampMap;
    }

    private ObservationPoolJoin()
    {
        _allIncludedCapturedObservations = ImmutableHashSet<ContentId>.Empty;
        _absouteTimestampMap = ImmutableSortedDictionary<DateTime, ContentId>.Empty;
    }

    /// <summary>
    /// Set of all <see cref="ContentId"/> of <see cref="CapturedObservation"/> included in this builders perspective
    /// </summary>
    private readonly ImmutableHashSet<ContentId> _allIncludedCapturedObservations;

    /// <summary>
    /// Maps every recorded discrete timestamp to the <see cref="ContentId"/> of the <see cref="KnowledgeBase.Observations"/> with
    /// the latest observation recorded at that timestamp.
    /// </summary>
    private readonly ImmutableSortedDictionary<DateTime, ContentId> _absouteTimestampMap;

    public static async ValueTask<KnowledgeBase> Add(KnowledgeBase a, KnowledgeBase b, IPlatformPersistence persistence)
    {
        // 1: Find first common ancestor (which is the point where all previous captured observations are exactly the same)

        async ValueTask<ContentId> FirstCommonAncestor()
        {
            // 1. Peek at ancestor pool of a' or b' with the latest latest timestamp
            // 2. If that perspectives content id is in the set, it's the first common ancestor
            // 3. If not, add it to the set and continue crawling throug ancestors
        }

        // 2: Re-apply remaining known observations by smallest timestamp first.

        // Remove all perspective cids from the set from 1. which are included in the first common ancestor.
        // Then sort all observations referenced by the remaining perspective cids by their trusted timestamp.
        // Pick best & build perspective until no observations are left

        // 3: Memoize all perspective-cids that existed in either a or b but not in the result anymore and add them to 'droppedPerspectives'.

        var newObservationSet = _allIncludedCapturedObservations.Add(linkedObservationCid);

        if (newObservationSet == _allIncludedCapturedObservations) {
            // Observation already included
            return this;
        }

        var capturedObservation = await persistence.Get<CapturedObservation>(linkedObservation.Observation);

        var invalidatedPerspectives = _absouteTimestampMap
            .Where(kvp => kvp.Key > capturedObservation.At);

        var trimmedTimestampMap = _absouteTimestampMap
            .RemoveRange(invalidatedPerspectives.Select(kvp => kvp.Key));

        // Todo: Rebuild all Perspective Sequence Nodes that changed in history. For now they are just removed.
        //foreach (var (timestamp, invalidatedPerspectiveSequenceHeadCid) in invalidatedPerspectives) {
        //    var invalidatedPerspectiveHead = await persistence.Get<PerspectiveSequenceHead>(invalidatedPerspectiveSequenceHeadCid);
        //    var invalidatedPerspectiveNode = (invalidatedPerspectiveHead as PerspectiveSequenceHead.Happening).Node;
        //    var l = invalidatedPerspectiveNode.LinkedObservation;
        //}

        if (_absouteTimestampMap.TryGetValue(capturedObservation.At, out var latestPerspectiveSequenceCidOnSameTimestamp)) {
            // There already is an observation on that exact same discrete timestamp.
            var newObservationPerspective = PerspectiveSequenceHead.NewHappening(new(
                previous: latestPerspectiveSequenceCidOnSameTimestamp,
                linkedObservation: linkedObservationCid));

            var newObservationPerspectiveCid = await persistence.Put(newObservationPerspective);

            var updatedTimestampMap = trimmedTimestampMap.Add(capturedObservation.At, newObservationPerspectiveCid);
            return new ObservationPoolJoin(newObservationSet, updatedTimestampMap);
        } else {
            // New discrete timestamp. Build on perspective before
            var newObservationPerspective = PerspectiveSequenceHead.NewHappening(new(
                previous: latestPerspectiveSequenceCidOnSameTimestamp,
                linkedObservation: linkedObservationCid));

            var newObservationPerspectiveCid = await persistence.Put(newObservationPerspective);

            var updatedTimestampMap = trimmedTimestampMap.Add(capturedObservation.At, newObservationPerspectiveCid);
            return new ObservationPoolJoin(newObservationSet, updatedTimestampMap);
        }
    }
}

#endif
