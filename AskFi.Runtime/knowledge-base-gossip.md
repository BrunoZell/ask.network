# Runtime Configuration: Knowledge Base Gossip

Actively listening to new observations to further gossip a more informative observation pool

## Processing Pipeline

1. Input `NewObservation`
2. Observation Integration Module
3. Output `NewObservationPool`

## Message Usage

| Runtime Message Type          | listening | broadcasting |
| ----------------------------- | --- | --- |
| `NewObservationPool`          | ✅ | ✅ |
