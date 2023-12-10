# Knowledge Base

Holds all IO information generated at the platform instances system boundary. That is, in _Observers_, _Brokers_, and time-sensitive _Sequencers_.

```yaml
observation-sequences:
- identity: B32observationSequenceId
  head: B32observationSequenceHead
action-sequences:
- identity: B32actionSequenceId
  head: B32actionSequenceHead
context-sequences:
- identity: B32contextSequenceId
  head: B32contextSequenceHead
  history: B32contextHistory
```
