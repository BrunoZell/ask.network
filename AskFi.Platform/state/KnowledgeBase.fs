namespace AskFi.Platform

open System.Threading.Tasks
open System
open System.Collections.Generic
open System.Threading
open AskFi.Platform.Persistence
open AskFi.DataModel

type KnowledgeBasePutRejected =
    /// The passed new sequence head would delete nodes previously appended
    | AlternativeForkRejected
    | SequenceIdMismatch

/// Consumer perspective to the KnowledgeBase State CRDT
type IKnowledgeBase =
    /// ### PUT ###

    /// Updates latest observation sequence head, merging it with locally available Knowledge Base state CRDT
    /// and returning the latest observation head after merge.
    abstract member PutObservationSequenceHead : (observationSequenceId: ContentId, newHead: ContentId) -> Result<ObservationSequenceHead, KnowledgeBasePutRejected>

    /// Updates latest action sequence head, merging it with locally available Knowledge Base state CRDT
    /// and returning the latest action head after merge.
    abstract member PutActionSequenceHead : (actionSequenceId: ContentId, newHead: ContentId) -> Result<ActionSequenceHead, KnowledgeBasePutRejected>

    /// Updates latest context sequence head, merging it with locally available Knowledge Base state CRDT
    /// and returning the latest context head after merge.
    abstract member PutContextSequenceHead : (contextSequenceId: ContentId, newHead: ContentId) -> Result<ContextSequenceHead, KnowledgeBasePutRejected>


    /// ### GET ###
    
    /// Reads latest observation sequence head from locally available Knowledge Base state CRDT.
    abstract member GetObservationSequenceHead : (observationSequenceId: ContentId) -> ObservationSequenceHead

    /// Reads latest action sequence head from locally available Knowledge Base state CRDT.
    abstract member GetActionSequenceHead : (actionSequenceId: ContentId) -> ActionSequenceHead
    
    /// Reads latest context sequence head from locally available Knowledge Base state CRDT.
    abstract member GetContextSequenceHead : (contextSequenceId: ContentId) -> ContextSequenceHead


    /// ### WATCH ###

    /// Streams all future updates to a specific observation sequence, always returning the latest sequence head.
    abstract member WatchObservationSequence : (observationSequenceId: ContentId, CancellationToken) -> IAsyncEnumerable<ObservationSequenceHead>
    
    /// Streams all future updates to a specific action sequence, always returning the latest sequence head.
    abstract member WatchActionSequence : (actionSequenceId: ContentId, CancellationToken) -> IAsyncEnumerable<ActionSequenceHead>
    
    /// Streams all future updates to a specific context sequence, always returning the latest sequence head.
    abstract member WatchContextSequence : (contextSequenceId: ContentId, CancellationToken) -> IAsyncEnumerable<ContextSequenceHead>
