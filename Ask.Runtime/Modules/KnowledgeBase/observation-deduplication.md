# Observation Deduplication Module

## Processing Pipeline

1. Input `NewObservationPool`
2. Observation Pool Merge
3. Output `NewObservationPool`

## Message Usage

| Runtime Message Type          | listening | broadcasting |
| ----------------------------- | --- | --- |
| `NewObservationPool`          | ✅ | ✅ |
