namespace Ask.Node

open Ask.Host.Persistence

/// Output type produced by a wrapped sequencer, referencing all context sequence heads it every produced,
/// even if there was a rewind and on top of another head got built.
type ContextHistory = {
    // The last published context sequence, with the most information of all references context sequences.
    Latest: ContentId<unit(*ContextSequenceHead*)>

    // Referencing all published context sequences that since have been abandoned due to a rewind from late arriving data.
    /// Set<ContextSequenceHead.Happening>
    /// APG> where all ContextSequenceHead.Identity relating to those heads are the same, and equal to Latest.Identity
    Dropped: ContentId<unit(*ContextSequenceHead*)> list
}

/// With observation sequences and execution sequences being the root data entry point of all external data
/// flowing into the system, a knowledge base instance references a specific set of those sequences.
/// When two observation pools are merged, their greatest sum is taken, i.e. that with most information,
/// what include all information from both.
/// Each sequence is identified by its very first root node, and is then mapped to all latest known sequence heads sharing the same first node.
/// If all producing observers and brokers where honest and bug free, then there always would be one latest version. However, in case there are
/// multiple competing versions for the same sequence head, all will be referenced by this data structure to not loose data.
/// This is fine in a permissioned environment.
type KnowledgeBase = {
    /// Maps ObservationSequenceHead.Identity as the observation sequence id
    /// to the latest known ObservationSequenceHead.Observation
    /// Map<ObservationSequenceHead.Identity, ObservationSequenceHead.Observation>
    Observations: Map<ContentId<unit(*ObservationSequenceHead*)>, ContentId<unit(*ObservationSequenceHead*) > list>
    
    /// Maps ActionSequenceHead.Identity as the action sequence id
    /// to the latest known ActionSequenceHead.Action
    /// Map<ActionSequenceHead.Identity, ActionSequenceHead.Action>
    Actions: Map<ContentId<unit(*ActionSequenceHead*)>, ContentId<unit(*ActionSequenceHead*)> list>

    /// Maps ContextSequenceHead.Identity as the heaviest full context history,
    /// referencing all (abandoned) forks ever produced. This preserves the full history
    /// of real time ordering. Sequencing information is impure information and must be
    /// preserved for fully deterministic replays.
    /// Map<ContextSequenceHead.Identity, ContextHistory>
    ContextSequences: Map<ContentId<unit(*ContextSequenceHead*)>, ContentId<ContextHistory>>
}
