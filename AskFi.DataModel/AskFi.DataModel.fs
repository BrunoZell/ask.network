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
type CapturedObservation = {
    /// Absolute timestamp of when this observation was recorded.
    /// As of runtime clock.
    At: DateTime

    /// The 'Percept from Observation<'Percept> (type of the originating observer instance)
    PerceptType: Type

    /// All percepts that appeared at this instant, as emitted by an IObserver<'Percept> instance.
    Observation: ContentId // Sdk.Observation<'Percept>
}

/// All captured observations within an observer group are sequenced into
/// an observation sequence. Isolated observation sequences are a form of
/// entry point for new information into the system. CIDs to such sequences
/// are passed around to share information.
type ObservationSequenceHead =
    | Identity of Nonce:uint64
    | Observation of Node:ObservationSequenceNode
and ObservationSequenceNode = {
    /// Links previous ObservationSequenceHead to form a temporal order.
    Previous: ContentId // ObservationSequenceHead

    /// Cid to the then latest CapturedObservation that caused this observation sequence head to be appended.
    Capture: CapturedObservation
}

// #####################
// ####  EXECUTION  ####
// #####################
//
// Action execution traces are the other entry point of data flowing into the system,
// which include the information gained from executing certain actions.
// Keeping a handle on created action execution sequences therefore is important
// if the data should not get lost.


type ActionSet = {
    /// All actions the strategy has decided to initiate.
    Actions: Action array
}
and Action = {
    /// 'Action to route to according IBroker<'Action>.
    /// This type is taken from what the strategy emitted in its decision.
    ActionType: Type

    /// Cid to the action information. Has type of ActionType.
    ActionCid: ContentId
}

type ActionExecutionTrace =
    /// Data emitted by the IBroker action execution. Could include an execution id, transaction, or validity proofs.
    | Success of trace: byte[] option
    /// IBroker action execution failed. This holds an exception message, if any, encountered during user code execution.
    | Error of ``exception``: string option

type ActionExecutionResult = {
    /// What action has been executed.
    Executed: Action

    /// Trace output from broker.
    Trace: ActionExecutionTrace

    /// When the used IBroker implementation started executing.
    InitiationTimestamp: DateTime

    /// When the used IBroker implementation completed executing.
    CompletionTimestamp: DateTime
}

/// An action sequence is produced by a Broker Group, which forms the second type
/// of data entry into the system, holding information we got from executing actions.
type ActionSequenceHead =
    | Identity of Nonce:uint64
    | Action of Node:ActionSequenceNode
and ActionSequenceNode = {
    /// Links previous decision.
    Previous: ContentId // ActionSequenceHead

    /// Holds timestamps and information obtained through the broker.
    Result: ActionExecutionResult
}

// ##################
// ####  MEMORY  ####
// ##################

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
    Observations: Map<ContentId, ContentId list> // Map<ObservationSequenceHead, ObservationSequenceHead list>
    
    /// Maps ActionSequenceHead.Identity as the observation sequence id
    /// to the latest known ActionSequenceHead.Action
    Actions: Map<ContentId, ContentId list> // Map<ActionSequenceHead, ActionSequenceHead list>

    /// Maps ContextSequenceHead.Identity as the heaviest full context history,
    /// referencing all (abandoned) forks ever produced. This preserves the full history
    /// of real time ordering. Sequencing information is impure information and must be
    /// preserved for fully deterministic replays.
    Contexts: Map<ContentId, ContentId> // Map<ContextSequenceHead, ContextHistory>
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

type Happening =
    | Observation of CapturedObservation
    | Action of ActionExecutionResult

type ContextSequenceHead =
    | Identity of Nonce:uint64
    | Context of Node:ContextSequenceNode
and ContextSequenceNode = {
    /// Links previous context sequence head
    Previous: ContentId // ContextSequenceHead

    /// What new observation got appended to this context sequence.
    Observation: CapturedObservation
}

/// Output type produced by a wrapped sequencer, referencing all context sequence heads it every produced,
/// even if there was a rewind and on top of another head got built.
type ContextHistory = {
    // The last published context sequence, with the most information of all references context sequences.
    Latest: ContentId // ContextSequence

    // Referencing all published context sequences that since have been abandoned due to a rewind from late arriving data.
    Dropped: ContentId list // ContextSequence list
}

// ####################
// ####  Strategy  ####
// ####################
//
// Strategies are encoded roles that analyze information from a context sequence

/// Decision sequence for strategy executions along a context sequence, where
/// decisions are made from now into the future using the same strategy.
/// It is produced by the runtime modules 'Live Strategy' and 'Backtester',
/// with decision sequences of the live strategy module possibly being routed to an according 'Broker Group'.
type DecisionSequenceHead =
    | Start of DecisionSequenceStart
    | Decision of DecisionSequenceNode
and DecisionSequenceStart = {
    /// References the strategy to be used for every decision to be a valid decision sequence.
    Strategy:ContentId // Sdk.Strategy

    /// The first context the strategy should produce decisions on, with all later contexts being
    /// part of the same decision sequence for this to be a valid decision sequence.
    FirstContext:ContentId // ContextSequenceHead
}
and DecisionSequenceNode = {
    /// Links previous backtest evaluation.
    Previous: ContentId // BacktestEvaluationStart

    /// What actions have been decided on by the evaluated strategy.
    ActionSet: ContentId // ActionSet
}
