# AskFi Runtime

The _AskFi Runtime_ is a library of composable modules that can be orchestrated into a mechanical brain.

## Implemented Runtime Modules

- [Observer Module](Modules/Observation/observer.md)
- [Observation Integration Module](Modules/ObservationPool/observation-integration.md)
- [Observation Deduplication Module](Modules/ObservationPool/observation-deduplication.md)
- [Strategy Module](Modules/Strategy/strategy.md)
- [Broker Module](Modules/Action/broker.md)

## Predefined Configurations

- [Observer Group](observer-group.md)
- [Knowledge Base Gossip](knowledge-base-gossip.md)
- [Live Strategy](live-strategy.md)
- [Broker Group](broker-group.md)

## Module Implementation Details

Modules are .NET types that are initialized with, respectively:

- A reference to `IPlatformPersistence` to read or write persistent storage
- A reference to `ChannelReader<TInput>` to receive input messages from
- User-defined configuration (like dlls with SDK implementations)

Once initialized, they can be composed with other modules by passing their `.Output` property into the input initialization of another module.

To start the module pipeline, call `Run(cancellationToken)` on all of the individual modules.

To stop the pipeline, cancel the passed `cancellationToken`.
