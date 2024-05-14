module Ask.DataModel

open System
open Ask.Host.Persistence

// ####################
// #### PRIMITIVES ####
// ####################
 
type Primitive =
    | Boolean of bool // 4 bytes
    | Integer of int64 // 8 bytes
    | Decimal of decimal // 16 bytes
    | String of string // 4 bytes + (len * 8 bytes) [UTF8]
    | Bytes of Byte[] // 4 bytes + (len * 1 bytes)
    | Link of ContentId<unit> // 8 bytes .NET reference to a ContentId<'Type>

// ###################
// ####  DOMAINS  ####
// ###################

type TypeTerm =
    | Error // 0
    | Unit // 1
    | Sum of t1:TypeTerm * t2:TypeTerm
    | Product of t1:TypeTerm * t2:TypeTerm
    | Primitive of Primitive

// Addressing a specific TypeTerm by a well-formed hash of the term
type TypeHash = TypeHash of uint64

type ProbabilityDistribution<'T when 'T : comparison> = P of Map<'T, float>
type Variable = Variable of domain:TypeHash * label:string * information:ProbabilityDistribution<unit> (*P(u)*)

type Intervention<'Action, 'X> = {
    (*F_i*) Intervention: 'Action -> 'X
}

type Endogenous<'Z> = {
    (*f_i(pa_i, u_i)*) Mechanism: Sdk.Context * Sdk.World -> 'Z
    // Sdk.Context exposes Observations O ⊆ V, basically passing P(u) as PMF, with static validation ensuring P(u) = P(u_i)
    // Sdk.World exposes Queries Z ⊆ V over all times t <= now, basically passing the nodes parents pa as a recursive SCM
    // the mechanisms function signature and static analysis of calls to Context and World implies:
    // background variables input P(u_i)
    // latent variables input Z_i
    // observed variables input O_i
}

type CausalModel = {
    (*X*) Actions: Map<Variable, Intervention<unit, unit>>
    (*Z*) Covariates: Map<Variable, Endogenous<unit>>
    (*Y*) Outcome: Variable list
}

type Node = Variable
type DirectedEdge = Node * Node
type BiDirectedEdge = Node * Node

/// 1. A directed graph is a pair G = (V, E), where V is a set of nodes and E is a set of directed
/// edges, which is a subset E ⊆ V × V of ordered pairs of nodes. Each element (i, j) ∈ E can
/// be represented by the directed edge i → j or equivalently j ← i. In particular, (i, i) ∈ E
/// represents a self-cycle i → i.
/// 2. A directed mixed graph is a triple G = (V, E,B), where the pair (V, E) forms a directed
/// graph and B is a set of bidirected edges, which is a subset B ⊆ {{i, j} : i, j ∈ V, i 6= j}
/// of unordered (distinct) pairs of nodes. Each element {i, j} ∈ B can be represented by the
/// bidirected edge i ↔ j or equivalently j ↔ i. Note that a directed graph can be considered
/// as a directed mixed graph without bidirected edges.
type CausalGraph = {
    (*V*) Nodes : Node list;
    (*E*) DirectedEdges : DirectedEdge list;
    (*B*) BiDirectedEdges : BiDirectedEdge list
}

// Address a specific 'Observation by its union case number from a Domain.ObservationSpace
type ObservationType = ObservationType of uint64
// Address a specific 'Action by its union case number from a Domain.ActionSpace
type ActionType = ActionType of uint64
// Address a specific 'Variable by its union case number from a Domain.LatentVariables
type VariableType = VariableType of uint64
// Address a specific Label by its ID from a Domain.Labels
type LabelId = LabelId of uint64

// Everything that represents a domain module in one data structure.
// Meant to be serialized, transmitted, interpreted, and validated.
type Domain = {
    Labels: Map<LabelId, TypeHash> // indexed by ordinal
    Types: Map<TypeHash, TypeTerm> // where Key = Value.TypeHash()
    // Facts: For each type, a custom validation function. Alternatively, CQL path equations to allow for a formal prover.
    ObservationSpace: LabelId list // indexed-set of ordinal indexes of the label that is considered a valid observation and is free to be used in SCM mechanisms
    ActionSpace: LabelId list // indexed-set of ordinal indexes of the label that is considered an action, and can be routed to an according IBroker when available.
    CausalAssumptions: CausalModel
}

// ######################
// #### OBSERVATIONS ####
// ######################

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

// [measurment]->ObservationSequence
// ObservationSequence.append(ObservationSequence) = ObservationSequence

// [happening]->strategy->[decision]->broker->[act]
// ContextSequence + Strategy = DecisionSequence (where each decision.'Action is element of strategy.'ActionSpace)
// DecisionSequence + BrokerGroup = ActionSequence (where each decision.'Action is element of BrokerGroup.'ActionSpace)


// EnvironmentResponses = Context + 'Action -> Counterfactual[TwinNetwork(domain.CausalAssumptions, do 'Action)]
// walk(EnvironmentResponses) = iteration of all action sequnces among all agents in EnvironmentResponses (in APG as infinite stream) ; which is, what commitment is triggered next, and what agent does that action
// SimulationSequence = EnvironmentSequence + Strategy (with Actions from Strategies decision)
// SimulationSequence.latest := ContextSequence
// SimulationSequence = EnvironmentSequence + Actions (or with arbitrary other action set)

// to integrate newest observations while keeping latent effects of previously simulated actions, like account balance debits:
// SimulationSequence = SimulationSequence + ContextSequence (where SimulationSequence.latestContext.traverseToIdentity() == ContextSequence.traverseToIdentity())

// every such state action sequence can be evaluated at each intermediary superstate.
// a specifiy non-branching sequence of evaluations of those states is a trajectory, with each trajectory ending when a steady state ecomomy is reached.
// Trajectory = SimulationSequence[context->action->simulation[->simulation]->action] + ValueTree
// Trajectory.step.askFulfillment = [evaluation of all users asks]
// Trajectory.step.preferenceScore = [weighted aggregation of all individual ask fulfillments]
// Trajectory.step.aggregatedUtility
// Trajectory.totalUtility