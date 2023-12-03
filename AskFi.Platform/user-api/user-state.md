# User State

```yaml
queries:
- name: Candlesticks
  code: B32queryId
visualizations:
- name: Order Book Heatmap
  code: B32visualizationId
strategies:
- name: Strategy A
  code: B32strategyId
query-runs:
  active:
  - identity: B32queryRunId
  completed:
  - identity: B32queryRunId
backtests:
  active:
  - identity: B32backtestId
  completed:
  - identity: B32backtestId
simulations:
  active:
  - identity: B32simulationId
  completed:
  - identity: B32simulationId
execution:
  active: B32strategyId
  previous:
  - started: TS
    stopped: TS
    strategy: B32strategyId
```