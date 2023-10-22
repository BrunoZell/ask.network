namespace AskFi.Runtime.Messages

open AskFi.Runtime.Persistence

/// Message from Observer Group emitted on each new received observation,
/// in parallel with updating that observation sequence in the platform state.
/// If valid, it's appended to the previous version of the single observation sequence an Observer Group produces.
type NewObservation = {
    /// Cid to the original ObservationSequenceHead.Identity of the newly produced observation sequence head.
    Identity: ContentId

    /// Cid to the newly produced ObservationSequenceHead.Observation
    /// To be a valid message, this.Head must carry the identity of this.Identity
    Head: ContentId
}

/// Message from Sequencer whenever 
type NewContext = {
    /// Cid to the original ContextSequenceHead.Identity of the newly produced context sequence head.
    Identity: ContentId

    /// Cid to the newly produced and currently latest ContextSequenceHead.Observation
    /// To be a valid message, this.Head must carry the identity of this.Identity
    Head: ContentId

    /// Cid to the full history, including rewinds, the sequencer has produced.
    /// To be a valid message, this.Head must be the latest context sequence head from thus.History
    History: ContentId
}

/// Message emitted from Strategy Module when a non-inaction decision has been made
type NewDecision = {
    /// Cid to the original DecisionSequenceHead.Identity of the newly produced decision sequence head.
    Identity: ContentId

    /// Cid to the newly created decision sequence head (DataModel.DecisionSequenceHead)
    Head: ContentId
}

/// Represents the result of a completed action execution, successful or not, by a broker
type ActionExecuted = {
    /// Cid to the original ActionSequenceHead.Identity of the newly produced action sequence head.
    Identity: ContentId

    /// Cid to the newly created action sequence head (DataModel.ActionSequenceHead)
    Head: ContentId
}

/// Broadcasted by persistence system for other nodes to eagerly receive data referenced in other messages.
type PersistencePut = {
    Cid: ContentId
    Content: byte array
    TDatum: System.Type
}
