module Ask.DataModel

open System
open Ask.Host.Persistence

/// Generated on every network message sent or received from within an IObserver implementation.
/// 'Message is a Protocols Message-type
type CapturedMessage<'Message> = {
    /// Absolute timestamp of when this observation was recorded.
    /// As of runtime clock.
    At: DateTime

    /// All percepts that appeared at this instant, as emitted by an IObserver<'Percept> instance.
    Message: ContentId<'Message>
}

/// All captured observations within an observer group are sequenced into
/// an observation sequence. Isolated observation sequences are a form of
/// entry point for new information into the system. CIDs to such sequences
/// are passed around to share information.
type ProtocolSessionHead<'Message> =
    | Identity of Nonce:uint64
    | Message of Node:ProtocolSessionMessage<'Message>
and ProtocolSessionMessage<'Message> = {
    /// Links previous ProtocolSessionHead to form a temporal order.
    Previous: ContentId<ProtocolSessionHead<'Message>>

    /// Cid to the then latest captured message that caused this protocol session head to be appended.
    Capture: CapturedMessage<'Message>
}
