module Ask.Sdk
open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Ask.Host.Persistence

// ##############
// ####  IO  ####
// ##############

/// Unique identifier for a single IO-Instance (Observer-Instance or Broker-Instance)
/// from controller behavior new network protocol sessions are created.
type InteractorIdentity =
    | InteractorIdentity of StartTimestamp:DateTime * NodeId:int64 * Nonce:int64

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

/// Implemented by a persistence backend that accepts and stores
/// network session states as they are captured.
type INetworkProtocolPersistence<'SessionIdentity, 'Message> =
    abstract member Store: 'SessionIdentity -> 'Message -> Task

/// Interface for interactors that manage multiple network protocols and capture their communications.
type IInteractor<'MessageSpace> =
    abstract member Interact : unit -> IAsyncEnumerable<CapturedMessage<'MessageSpace>>

// ######################
// #### OBSERVATIONS ####
// ######################

/// The instance of a single observation from the given 'ObservationSpace.
type Percept<'ObservationSpace> = {
    SensoryInformation: 'ObservationSpace
}

/// An atomic appearance of sensory information emitted from a single IObserver instance.
/// Includes one or more Percepts, which is a typed (via a domain model) representation of the newly observed sensory information.
/// An observation can have multiple Percepts in case they all appeared at the same instant (point in time),
/// with each Percept representing a single piece of sensory information, each with a single 'Observation type selected from 'ObservationSpace.
/// An individual observation, by definition, appeared at a singular instant (point in time), and must've originated from some IObserver<'Observation>.
/// The 'Percept is strongly typed here, reflecting the IObserver's interpretation of the observed data.
[<IsReadOnly; Struct>]
type Observation<'ObservationSpace> = {
    Percepts: Percept<'ObservationSpace> array
}

/// Implemented by domain modules. An IObserver implementation defines imperative networking behavior to interact with
/// an external system with the aim of observing it to later infer what was happening. The observer implementation
/// interprets the networking traffic and emits strongly-typed 'Percepts whenever there are new measurements obtained.
type IObserver<'ObservationSpace> =
    abstract member Observations : unit -> IAsyncEnumerable<Observation<'ObservationSpace>>

// ###################
// #### DECISIONS ####
// ###################

/// Virtually represents a specific abstract action with some 'Action type from 'ActionSpace
type Action<'ActionSpace> = {
    Action: 'ActionSpace
}

/// Represents the strategies intent to execute one or more specified actions immediately.
/// Where "immediately" is analogous to "now" in the evaluated context.
/// With "now" being the contexts latest instant (point in time).
/// With the contexts latest instant (point in time) being the latest observation in the context sequence.
type Initiative<'ActionSpace> = {
    Actions: Action<'ActionSpace> array
}

/// Represents the complete decision of a strategy when evaluated against a context.
/// It can either decide to do nothing (Inaction),
/// or decide to initiate one or more actions from the strategies 'ActionSpace.
/// An 'ActionSpace is a discriminated union over one or more 'Action types,
/// allowing the strategy to decide on multiple different 'Actions simultaneously,
/// with each Initiative<'Action> selecting exactly one 'Action from that 'ActionSpace
/// which then can be executed by an according IBroker<'Action, _>.
type Decision<'ActionSpace> =
    | Inaction
    | Initiate of Initiative<'ActionSpace>

// #################
// #### QUERIES ####
// #################

/// Represents an observation emitted by some IObserver. It is returned from a Context which serves as a window
/// into all past observations and action traces.
/// A measurement is an atomic appearance of sensory information for callers of IContextQueries,
/// originating from captured observations of a single IObserver<'Observation> instance.
/// An individual observation, by definition, appeared at a singular instant (point in time) from a defined IObserver<'Observation>.
/// When this observation was captured, a runtime timestamp was attached to later filter it by query-provided unobserved timestamps.
[<IsReadOnly; Struct>]
type Measurement<'ObservationSpace> = {
    /// Sensory information captured by the producing IObserver.
    Observation: Observation<'ObservationSpace>

    /// Runtime timestamp at which this observation was produced by the IObserver.
    At: DateTime
}

/// Represents success or failure of a single IBroker<_, 'Response> action execution.
type Evidence<'Response> = 
    /// Data emitted by the IBroker<'Action, 'Response> action execution.
    /// Responses could include an execution id, transaction, or validity proofs, but are abstracted out at the data structure level.
    | Success of Trail: ContentId<'Response>
    /// IBroker action execution failed. This holds an exception message, if any, encountered during user code execution.
    | Error of Exception: string option

/// Represents a completed action execution. Similarly as a measurement, it is an interface
/// into new environment information and can be queried via the Context.
type Act<'Action, 'Response> = {
    /// Virtual representation of the domain action that has been executed
    Action: ContentId<'Action>

    /// The information obtained by the broker executing this action.
    /// Of a domain-defined type 'Response.
    Evidence: Evidence<'Response>

    /// Runtime-measured timestamps of when the used IBroker implementation started executing.
    InitiationTimestamp: DateTime

    /// Runtime-measured timestamps of when the used IBroker implementation completed executing.
    CompletionTimestamp: DateTime
}

/// Public query interface into a given Context.
/// All queries on Context have a type parameter 'ActionSpace.
/// The strategy compiler verifies it only used with this "Reflection<'ActionSpace>" action space type.
/// Used by strategies, visualizations, and standalone analysis code to retrieve information from a Context.
/// Where 'ObservationSpace is a discriminated union over one or more distinct 'Percept types.
type Context = 
    /// Get the latest received perception of the requested type.
    /// Returns `None` if no observation of the requested type has been made yet.
    abstract member latest<'ObservationSpace> : unit -> Measurement<'ObservationSpace> option
    
    /// Get an iterator the all Observations of type `'Perception` since the passed `from` until `to`
    /// (as determined by the runtime clock used during context sequencing).
    abstract member inTimeRange<'ObservationSpace> : from: DateTime * ``to``: DateTime -> Measurement<'ObservationSpace> seq
    
    /// Get the latest received perception of the requested type.
    /// Returns `None` if no observation of the requested type has been made yet.
    abstract member latest<'Action, 'Response> : unit -> Act<'Action, 'Response> option
    
    /// Get an iterator the all Observations of type `'Perception` since the passed `from` until `to`
    /// (as determined by the runtime clock used during context sequencing).
    abstract member inTimeRange<'Action, 'Response> : from: DateTime * ``to``: DateTime -> Act<'Action, 'Response> seq

/// A query that solely depends on observations or action traces to compute its result.
/// Really a query is just a parameterized transformer mapping a context to an instance of some type 'Result.
type Query<'Query, (*'ObservationSpace,*) 'Result> =
    'Query -> Context -> 'Result

/// An indirect view on a context, where only specific queries lying on a 'QuerySurface are readable.
type World = 
    /// Get the latest query result of the requested type.
    abstract member latest<'Query, 'Result> : unit -> 'Result
    
    /// Get an iterator of all the query result of the requested type since the passed `from` until `to`.
    /// This essentially iterates all happenings in the underlying context sequence and checks of the query
    /// result has changed. If it has, a new element in this iterator is yielded.
    abstract member inTimeRange<'Query, 'Result> : from: DateTime * ``to``: DateTime -> 'Result seq

/// A query that solely depends on other queries for its evaluation.
type CompositeQuery<'Query, (*'QuerySurface,*) 'Result> =
    'Query -> World -> 'Result

// ##################
// #### STRATEGY ####
// ##################
//
// Strategies additionally have access to their own past decision sequence, allowing them to
// memorize executed decisions and have available to be conditioned on in the strategy.
/// This is important so the strategy can remember it just did some 'Action,
/// to not do it again even if there is no external sensory-information hinting on it.

/// Represents a strategies past decision of previous points in time on the evaluated context.
type DecisionReflection<'ActionSpace> = {
    /// The decision that is reflected on
    Decision: Decision<'ActionSpace>

    /// Timestamp of the context on which this decision was made.
    VirtualTimestamp: DateTime
    
    /// Runtime timestamp on which this decision was made.
    /// For live execution, the difference between virtual and actual timestamp is the strategy evaluation time.
    /// For backtests, the difference between virtual and actual timestamp is the time duration looking back in time
    /// (i.e. the age of the evaluated context)
    ActualTimestamp: DateTime
}

/// Query interface into decisions from a decision sequence "Ask.DataModel.DecisionSequenceHead".
/// All queries on Reflection have a type parameter 'ActionSpace.
/// The strategy compiler verifies it only used with this "Reflection<'ActionSpace>" action space type.
/// Used by strategies to reflect on past actions. This is important so the strategy can remember it just
/// did some 'Action to not do it again even if there is no external sensory-information hinting on it.
type Reflection =
    /// Get the latest decisions of actions contained in the the strategies 'ActionSpace' or narrower.
    /// Please note that strategies can decide on multiple initiatives of different 'Action within their 'ActionSpace.
    /// Static analysis ensures that the strategy only calls Reflection queries with their explicitly specified 'ActionSpace.
    /// Returns `None` if no decisions of the requested type have been made yet.
    abstract member latest<'ActionSpace> : unit -> DecisionReflection<'ActionSpace> option
    
    /// Get an iterator of all decisions since the passed `from` until `to` that contain initiatives of an action-type in the fn-type-param 'ActionSpace,
    /// where that parameter must be fully contained in the strategies 'ActionSpace' or narrower.
    /// Please note that strategies can decide on multiple initiatives of different 'Action within their 'ActionSpace.
    /// Static analysis ensures that the strategy only calls Reflection queries with their explicitly specified 'ActionSpace.
    /// Returns `None` if no decisions of the requested type have been made yet.
    abstract member inTimeRange<'ActionSpace> : from: DateTime * ``to``: DateTime -> DecisionReflection<'ActionSpace> seq

/// Contains the code of a strategy decision, called upon each evolution of the Askbot sessions world (i.e. on every change in a query result).
type Strategy<'Parameters, (*'QuerySurface,*) 'ActionSpace> =
    'Parameters -> Reflection -> World -> Decision<'ActionSpace>

// ###################
// #### EXECUTION ####
// ###################

type IBroker<'Action, 'Response> =
    abstract member Execute : 'Action -> Task<'Response>
