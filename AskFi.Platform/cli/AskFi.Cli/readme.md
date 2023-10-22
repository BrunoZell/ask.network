# AskFi CLI

```text
askfi strategy backtest
    --project file/to/strategy.proj
    --strategy Rabot.Strategies.Bru2o
    --context-id B32...
    [--start x]
    [--end y]
    [--no-simulation]
    [--in-process]
```

Overview: Compile, [upload] and fast-forward run a strategy on historic observations and simulate it's pnl (or whatever import metric informs the strategy developer and operator).

1. Compiles project, loads dll, puts it in structure with -s value (the strategy type name in dll), and prints the cid. It is the unique identifier of the strategy [send strategy id + backtest request to remote cluster/ to platform]
2. Strategy executer takes that strategy id, and instantiates the type specified by --strategy by loading dll and type create. It also creates a backtest entry in the platform state, indicating it is active.
3. Further it takes the context sequence cid
produces stream of Decision Sequence, for each Context Sequence Node for which -s produced a non-inaction result. The latest decision sequence head is recorded in the platform state. For vnow, possibly rate-limited to not overload https api. For vlater eagerly locally apply new decisions to platform state and rely on eventual consistency.
4. Decisions are simulated on the domain model, with a latent state being injected between recorded history and strategy queries from now on, which may alter the result of a query according to simulated effects of counterfactual actions. Both simulation and observations share the same semantic query interface.
This may affect later strategy decisions. Whenever a query the strategy used was altered by a simulation, it is marked in the results as it may be inaccurate and depends on the assumptions of the domain model.

```text
askfi strategy simulate
    -p path/project.fsproj
    -s Rabot.Strategies.Bru2o
    -c B32livecontextid
    [--in-process]
```

Overview: Compile, [upload], live run, simulate action effects
To then later: run monitoring queries, eagerly build visualization canvas

```text 
askfi strategy deploy
    -p path/project.fsproj
    -s Rabot.Strategies.Bru2o
    -c B32livecontextid
    [--in-process]
```

Overview:  Compile, [upload], live run and live action execution via brokers

1. Strategy preparation: same as backtest
2. Remote executor initialization: same as backtest
3. Remote execution: stream context updates under passed --context-id (or default live context if omitted). Eagerly run strategy on new context [vperf: only run if new observation is causally linked to the strategy decision, via static analysis]
4. all non-inaction decisions are pushed to platform state, in reference to the live strategy execution entry. Further these decisions are executed by a matching broker. Live executor sends Redis message NewDecision with newest decision sequence [to matching broker, via the action-types dedicated message channel]. Broker then double-checks whether the decision actually came from a live strategy (via platform state) and only then executes it and posts the action trace to the state. 
