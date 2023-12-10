# Runtime Component: Observer Group

## Processing Pipeline

1. Observer Module with custom `IObserver`-Instances
2. Output `NewObservation`

## Message Usage

| Runtime Message Type          | listening | broadcasting |
| ----------------------------- | --- | --- |
| `NewObservationPool`          |     | ✅ |
| `PersistencePut`              | ✅ | ✅ |

## Persistence Usage

| Runtime Data Model Type       | cid | get | put | pin |
| ----------------------------- | --- | --- | --- | --- |
| `ObservationSequenceHead`     | ✅  |     | ✅ |  ✅  |
| `ObservationSequenceNode`     | ✅  |     | ✅ |  ✅  |
| `CapturedObservation`         | ✅  |     | ✅ |  ✅  |
