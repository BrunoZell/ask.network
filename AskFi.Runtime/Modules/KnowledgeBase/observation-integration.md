# Observation Deduplication Module

## Processing Pipeline

1. Input `NewObservation`
2. Convert `NewObservation` to `ObservationPool`
3. Observation Pool Merge
4. Output `NewObservationPool`

## Message Usage

| Runtime Message Type          | listening | broadcasting |
| ----------------------------- | --- | --- |
| `NewObservationPool`          | ✅ | ✅ |
