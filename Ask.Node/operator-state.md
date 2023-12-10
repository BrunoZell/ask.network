# Operator State

This specifies all active workers that should be schedules in Kubernetes right now. It defines the desired state.

The _Platform Operator_ maintains this state and pushes according k8s resource manifestos.

```yaml
observer-workers:
broker-workers:
sequencer-workers:
query-workers:
backtest-workers:
simulation-workers:
live-strategy-worker:
  strategies:
  - user: B32userId
    strategy: B32strategyId
```
