module Ask.DataModel

open System
open Ask.Host.Persistence

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

// with 'ActionSpace = Map<'Action, 'Response>, it picks any single 'Response as type
type response_of_any_action_from<'ActionSpace> = struct end

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

type ActionExecutionResult<'ActionSpace> = {
    /// What specific action has been executed by the IBroker
    Executed: Sdk.Action<'ActionSpace>

    /// Trace output from broker.
    Trace: ActionExecutionTrace<response_of_any_action_from<'ActionSpace>>
    // where trace.ActionExecutionTrace.'Response = executed.Actions.map('Action.'Response).any()

    /// When the used IBroker implementation started executing.
    InitiationTimestamp: DateTime

    /// When the used IBroker implementation completed executing.
    CompletionTimestamp: DateTime
}

/// An action sequence is produced by a Broker Group, which forms the second type
/// of data entry into the system, holding information we got from executing actions.
type ActionSequenceHead<'ActionSpace> =
    | Identity of Nonce:uint64
    | Action of Node:ActionSequenceNode<'ActionSpace>
and ActionSequenceNode<'ActionSpace> = {
    /// Links previous decision.
    Previous: ContentId<ActionSequenceHead<'ActionSpace>>

    /// Holds timestamps and information obtained through the broker.
    Result: ActionExecutionResult<'ActionSpace>
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

type Happening<'ObservationSpace, 'ActionSpace> =
    | Observation of CapturedObservation<'ObservationSpace>
    | Action of ActionExecutionResult<'ActionSpace>

type ContextSequenceHead<'ObservationSpace, 'ActionSpace> =
    | Identity of Nonce:uint64
    | Context of Node:ContextSequenceNode<'ObservationSpace, 'ActionSpace>
and ContextSequenceNode<'ObservationSpace, 'ActionSpace> = {
    /// Links previous context sequence head
    Previous: ContentId<ContextSequenceHead<'ObservationSpace, 'ActionSpace>>

    /// The latest happening of this context sequence.
    Happening: Happening<'ObservationSpace, 'ActionSpace>
}

// ###################
// ####   QUERY   ####
// ###################

/// Decision sequence for strategy executions along a context sequence, where
/// decisions are made from now into the future using the same strategy.
/// It is produced by the runtime modules 'Live Strategy' and 'Backtester',
/// with decision sequences of the live strategy module possibly being routed to an according 'Broker Group'.
type QuerySequenceHead<'Query, 'Result> =
    | Start of QuerySequenceStart<'Query, 'Result>
    | Decision of Node:QuerySequenceNode<'Query, 'Result>
and QuerySequenceStart<'Query, 'Result> = {
    /// References the strategy to be used for every decision to be a valid decision sequence.
    Query: ContentId<Sdk.Query<'Query, (*'ObservationSpace,*) 'Result>> // -> space of pssibly queryed 'ObservationSpace implied by the queries typed use of Context(latest, inTimeRange)-queries and World(latest, inTimeRange)-queries.

    /// The first context the strategy should evaluate the query on, with all later contexts being
    /// part of the same query sequence for this to be a valid query sequence.
    FirstContext: ContentId<ContextSequenceHead<unit, unit>>
}
and QuerySequenceNode<'Query, 'Result> = {
    /// Links previous decision head of this decision sequence.
    Previous: ContentId<QuerySequenceHead<'Query, 'Result>>

    /// What actions have been decided on by the evaluated strategy.
    Result: ContentId<'Result>
}

// ####################
// ####  STRATEGY  ####
// ####################

/// Decision sequence for strategy executions along a context sequence, where
/// decisions are made from now into the future using the same strategy.
/// It is produced by the runtime modules 'Live Strategy' and 'Backtester',
/// with decision sequences of the live strategy module possibly being routed to an according 'Broker Group'.
type DecisionSequenceHead<'StrategyParameters, 'ActionSpace> =
    | Start of DecisionSequenceStart<'StrategyParameters, 'ActionSpace>
    | Decision of Node:DecisionSequenceNode<'StrategyParameters, 'ActionSpace>
and DecisionSequenceStart<'StrategyParameters, 'ActionSpace> = {
    /// References the strategy to be used for every decision to be a valid decision sequence.
    Strategy: ContentId<Sdk.Strategy<'StrategyParameters, (*'ObservationSpace,*) 'ActionSpace>>

    /// The first context the strategy should produce decisions on, with all later contexts being
    /// part of the same decision sequence for this to be a valid decision sequence.
    FirstContext: ContentId<ContextSequenceHead<unit, 'ActionSpace>>
}
and DecisionSequenceNode<'StrategyParameters, 'ActionSpace> = {
    /// Links previous decision head of this decision sequence.
    Previous: ContentId<DecisionSequenceHead<'StrategyParameters, 'ActionSpace>>

    /// What actions have been decided on by the evaluated strategy.
    Decision: ContentId<Sdk.Decision<'ActionSpace>>
}
