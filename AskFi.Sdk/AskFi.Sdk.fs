module AskFi.Sdk
open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading.Tasks

// ######################
// #### OBSERVATIONS ####
// ######################

/// The instance of a single observation from the given 'ObservationSpace.
type Percept<'ObservationSpace> = {
    SensoryInformation: specific_observation_of<'ObservationSpace>
}

/// An atomic appearance of sensory information from within an IObserver<'Percept>.
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
    abstract member Observations : IAsyncEnumerable<Observation<'ObservationSpace>>

// #################
// #### QUERIES ####
// #################

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

/// A query really is just a parameterized transformer mapping a context to an instance of some type 'Result.
type Query<'ObservationSpace, 'Query, 'Result> =
    'Query -> Context -> 'Result

// ###################
// #### DECISIONS ####
// ###################

/// Virtually represents a specific abstract action with some 'Action type from 'ActionSpace
type Action<'ActionSpace> = {
    Action: specific_action_of<'ActionSpace>
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

// ##################
// #### STRATEGY ####
// ##################

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

/// Query interface into decisions from a decision sequence "AskFi.Runtime.DataModel.DecisionSequenceHead".
/// All queries on Reflection have a type parameter 'ActionSpace.
/// The strategy compiler verifies it only used with this "Reflection<'ActionSpace>" action space type.
/// Used by strategies to reflect on past actions. This is important so the strategy can remember it just
/// did some 'Action to not do it again even if there is no external sensory-information hinting on it.
type Reflection = 
    /// Get the latest received perception of the requested type.
    /// Returns `None` if no observation of the requested type has been made yet.
    abstract member latest<'ActionSpace> : unit -> DecisionReflection<'ActionSpace> option
    
    /// Get an iterator the all Observations of type `'Perception` since the passed `from` until `to`
    /// (as determined by the runtime clock used during context sequencing).
    abstract member inTimeRange<'ActionSpace> : from: DateTime * ``to``: DateTime -> DecisionReflection<'ActionSpace> seq

/// Contains the code of a strategy decision, called upon each evolution of the Askbot sessions context (i.e. on every new observation).
type Strategy<'ObservationSpace, 'QuerySurface, 'ActionSpace> =
    Reflection -> Context -> Decision<'ActionSpace>

// ###################
// #### EXECUTION ####
// ###################

type IBroker<'Action, 'Response> =
    abstract member Execute : 'Action -> Task<'Response>
