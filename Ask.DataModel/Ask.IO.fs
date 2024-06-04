module Ask.IO

open Ask.Host.Persistence
open Sdk
open System

/// All network sessions originating from an IO-Instance (as embedded in IObserver and IBroker instances)
/// are recorded into protocol session sequences.
/// These sequences reference the origin IO-Instance in it's first node and sequence all network
/// message exchanged through that network session.
/// What constitutes a session is defined by each protocol implementation separately.
/// Network sessions with external IT-Systems are the only entry point for new information into the system.
/// Special network protocol clients are used in IObserver and IBroker implementations to implicitly capture
/// all network messages sent and received.
type MessageSequenceHead<'Session, 'Message> =
    | Identity of ProtocolSessionIdentity<'Session>
    | Message of Node:MessageSequence<'Session, 'Message>
and MessageSequence<'Session, 'Message> = {
    /// Links previous MessageSequence to form a temporal order.
    Previous: ContentId<MessageSequenceHead<'Session, 'Message>>

    /// Cid to the then latest captured message that caused this protocol session head to be appended.
    Capture: CapturedMessage<'Message>
}

/// Interface to observe an Sdk.Interactor instance that controls multiple network protocol types to capture their communications.
type IObservableInteractor =
    /// An observable that notifies subscribers whenever a new protocol session is started.
    abstract member NewSession<'Protocol> : unit -> IObservable<'Protocol>

    /// An observable that notifies subscribers of new messages within any session.
    abstract member NewMessage<'Protocol, 'Message> : unit -> IObservable<MessageSequenceHead<'Protocol, 'Message>>
