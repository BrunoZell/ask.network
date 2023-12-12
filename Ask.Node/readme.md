# Ask Node

A single-machine client hosting the Ask Runtime.

It implements `Ask.Host.Persistence.IHostPersistence` which the Ask Runtime depends on and which is used by the node itself to store app data.

Code for observers, brokers, queries and strategies is read once at startup into a `DomainStore`, sourced from a local JSON file which points to .NET-dlls and typenames within those DLLs to be loaded as those.

Observers and brokers are configured as fixed configuration at startup once as type `OperatorState`. Future versions may allow in-flight adjustments to running observers and brokers.

There always runs a single default context sequencer, sequencing all observations and actions of this node.

A running node maintains a database of all captured information on the local disk as type `KnowledgeBase`, holding observations, actions and sequence information.

The node exposes an GRPC request to be called by the Ask CLI through which the user can do:

- Start live trading: live strategy with real action execution
- Start a simulation: live strategy with simulated effects
- Start a backtest: historic strategy with simulated effects
- Evaluate query on latest data
- Start a continuous live query evaluation
- Evaluate a query on a range of historic contexts
- Request a query evaluation on a historic context
