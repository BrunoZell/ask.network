# Observer Module

## SDK Types

### `IObserver<'Percept>`. `Observation<'Percept>`

An _Observation_ is an atomic appearance of sensory information. It consists of one or more _Percepts_, all of the same type. Percepts are domain-specific types to represent the newly observed information, sourced from the external world. _Observers_ sit at the boundary between the external world and the Ask Node.

An _Observation_ can have multiple _Percepts_ in case multiple percepts appeared at the same instant (same point in time). An individual observation, by definition, appeared at a singular instant and thus the list of percepts within a single observation does not specify an order.

## Runtime Data Types

### `CapturedObservation<'Percept>`

Generated immediately after an _`IObserver`-Instance_ emitted a new observation. An absolute timestamp of when this observation was recorded is attached. For that, the configured runtime clock. is used.

## Component Execution

Observations from all Observer-instances are eagerly pulled and turned into `CapturedObservation<_>` by attaching the current timestamp as by the runtime clock.
