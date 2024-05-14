module Ask.IO

open System
open Ask.Host.Persistence

// ######################
// ####  Networking  ####
// ######################

/// Implemented by a persistence backend that accepts and stores
/// network session states as they are captured.
type INetworkProtocolPersistence<'SessionIdentity, 'Message> =
    abstract member Store: 'SessionIdentity -> 'Message -> Task

// ##############
// ####  IO  ####
// ##############

/// Unique identifier for a single IO-Instance (Observer-Instance or Broker-Instance)
/// from controller behavior new network protocol sessions are created.
type InteractorInstanceIdentity =
    | InteractorInstanceIdentity of StartTimestamp:DateTime * NodeId:int64 * Nonce:int64

type ProtocolSessionIdentity<'Session> =
    | ProtocolSessionIdentity of Origin:InteractorIdentity * InitiationTimestamp:DateTime * Session:'Session * Nonce:int64

/// Generated on every network message sent or received from within an interactor implementation.
/// 'Message is a network protocols 'Message-type
type CapturedMessage<'Message> = {
    /// Absolute timestamp of when this observation was recorded.
    /// As of runtime clock.
    At: DateTime

    /// All percepts that appeared at this instant, as emitted by an IObserver<'Percept> instance.
    Message: ContentId<'Message>
}

/// All network sessions originating from an IO-Instance (as embedded in IObserver and IBroker instances)
/// are recorded into protocol session sequences.
/// These sequences reference the origin IO-Instance in it's first node and sequence all network
/// message exchanged through that network session.
/// What constitutes a session is defined by each protocol implementation separately.
/// Network sessions with external IT-Systems are the only entry point for new information into the system.
/// Special network protocol clients are used in IObserver and IBroker implementations to implicitly capture
/// all network messages sent and received.
type ProtocolSessionHead<'Session, 'Message> =
    | Identity of ProtocolSessionIdentity<'Session>
    | Message of Node:ProtocolSessionMessage<'Session, 'Message>
and ProtocolSessionMessage<'Session, 'Message> = {
    /// Links previous ProtocolSessionHead to form a temporal order.
    Previous: ContentId<ProtocolSessionHead<'Session, 'Message>>

    /// Cid to the then latest captured message that caused this protocol session head to be appended.
    Capture: CapturedMessage<'Message>
}

type IInteractor =
    abstract member Messages : unit -> IAsyncEnumerable<Message<'Protocol, 'Message>>