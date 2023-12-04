module AskFi.DataModel

open AskFi.Runtime.Persistence
open System

// ######################
// #### OBSERVATIONS ####
// ######################
//
// Observations are the main entry point of data flowing into the system.
// Keeping a handle on created observation sequences therefore is important
// if the data should not get lost.

/// Generated immediately after an IObserver emitted a new observation grouping
/// the observation with the latest local timestamp as of the runtime clock.
type CapturedObservation<'ObservationSpace> = {
    /// Absolute timestamp of when this observation was recorded.
    /// As of runtime clock.
    At: DateTime

    /// All percepts that appeared at this instant, as emitted by an IObserver<'Percept> instance.
    Observation: ContentId<Sdk.Observation<'ObservationSpace>>
}

/// All captured observations within an observer group are sequenced into
/// an observation sequence. Isolated observation sequences are a form of
/// entry point for new information into the system. CIDs to such sequences
/// are passed around to share information.
type ObservationSequenceHead<'ObservationSpace> =
    | Identity of Nonce:uint64
    | Observation of Node:ObservationSequenceNode<'ObservationSpace>
and ObservationSequenceNode<'ObservationSpace> = {
    /// Links previous ObservationSequenceHead to form a temporal order.
    Previous: ContentId<ObservationSequenceHead<'ObservationSpace>>

    /// Cid to the then latest CapturedObservation that caused this observation sequence head to be appended.
    Capture: CapturedObservation<'ObservationSpace>
}

// #####################
// ####  EXECUTION  ####
// #####################

// Action execution traces are the other entry point of data flowing into the system,
// which include the information gained from executing certain actions.
// Keeping a handle on created action execution sequences therefore is important
// if the data should not get lost
type ActionExecutionTrace<'Response> =
    /// Data emitted by the IBroker<'Action, 'Response> action execution.
    /// Responses could include an execution id, transaction, or validity proofs, but are abstracted out at the data structure level.
    | Success of trace: ContentId<'Response>
    /// IBroker action execution failed. This holds an exception message, if any, encountered during user code execution.
    | Error of ``exception``: string option

type ActionExecutionResult<'ActionSpace, 'Response> = {
    /// What specific action has been executed by the IBroker
    Executed: Sdk.Action<'ActionSpace>

    /// Trace output from broker.
    Trace: ActionExecutionTrace<'Response>

    /// When the used IBroker implementation started executing.
    InitiationTimestamp: DateTime

    /// When the used IBroker implementation completed executing.
    CompletionTimestamp: DateTime
}

/// An action sequence is produced by a Broker Group, which forms the second type
/// of data entry into the system, holding information we got from executing actions.
type ActionSequenceHead<'ActionSpace, 'Response> =
    | Identity of Nonce:uint64
    | Action of Node:ActionSequenceNode<'ActionSpace, 'Response>
and ActionSequenceNode<'ActionSpace, 'Response> = {
    /// Links previous decision.
    Previous: ContentId<ActionSequenceHead>

    /// Holds timestamps and information obtained through the broker.
    Result: ActionExecutionResult<'ActionSpace, 'Response>
}

// ###################
// ####  CONTEXT  ####
// ###################
//
// Producing context sequences is the act of sequencing observations one by one across multiple observation sequences.
// Sequencer produce context sequences, with each context adding one and only one new observation (or act) to the sequence per node.
// Different sequencer implementations may:
// - sequence on different timestamps (observer or sequencer), or
// - handle late arriving data differently (drop or rewind, up to a threshold)

type Happening<'ObservationSpace, 'ActionSpace, 'Response> =
    | Observation of CapturedObservation<'ObservationSpace>
    | Action of ActionExecutionResult<'ActionSpace, 'Response>

type ContextSequenceHead<'ObservationSpace, 'ActionSpace, 'Response> =
    | Identity of Nonce:uint64
    | Context of Node:ContextSequenceNode<'ObservationSpace, 'ActionSpace, 'Response>
and ContextSequenceNode<'ObservationSpace, 'ActionSpace, 'Response> = {
    /// Links previous context sequence head
    Previous: ContentId<ContextSequenceHead<'ObservationSpace, 'ActionSpace, 'Response>>

    /// The latest happening of this context sequence.
    Happening: Happening<'ObservationSpace, 'ActionSpace, 'Response>
}

// ####################
// ####  Strategy  ####
// ####################

/// Decision sequence for strategy executions along a context sequence, where
/// decisions are made from now into the future using the same strategy.
/// It is produced by the runtime modules 'Live Strategy' and 'Backtester',
/// with decision sequences of the live strategy module possibly being routed to an according 'Broker Group'.
type DecisionSequenceHead<'ObservationSpace, 'QuerySurface, 'ActionSpace, 'Response> =
    | Start of DecisionSequenceStart<'ObservationSpace, 'QuerySurface, 'ActionSpace, 'Response>
    | Decision of Node:DecisionSequenceNode<'ObservationSpace, 'QuerySurface, 'ActionSpace, 'Response>
and DecisionSequenceStart<'ObservationSpace, 'QuerySurface, 'ActionSpace, 'Response> = {
    /// References the strategy to be used for every decision to be a valid decision sequence.
    Strategy: ContentId<Sdk.Strategy<'ObservationSpace, 'QuerySurface, 'ActionSpace>>

    /// The first context the strategy should produce decisions on, with all later contexts being
    /// part of the same decision sequence for this to be a valid decision sequence.
    FirstContext: ContentId<ContextSequenceHead<'ObservationSpace, 'ActionSpace, 'Response>>
}
and DecisionSequenceNode<'ObservationSpace, 'QuerySurface, 'ActionSpace, 'Response> = {
    /// Links previous decision head of this decision sequence.
    Previous: ContentId<DecisionSequenceHead<'ObservationSpace, 'QuerySurface, 'ActionSpace, 'Response>>

    /// What actions have been decided on by the evaluated strategy.
    Decision: ContentId<Sdk.Decision<'ActionSpace>>
}
