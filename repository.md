# Repository Structure

```text
├───AskFi.Sdk
├───AskFi.DataModel
├───AskFi.Runtime
│   ├───ObserverGroup       [module]
│   ├───ContextSequencer    [module]
│   ├───BrokerGroup         [module]
│   ├───KnowledgeBaseMerge  [module]
│   ├───HistoricQuery       [module]
│   ├───HistoricSimulation  [module]
│   ├───LiveQuery           [module]
│   ├───LiveSimulation      [module]
│   └───LiveStrategy        [module]
├───AskFi.Platform
│   ├───workloads
│   │   ├───AskFi.Platform.Observer.Node                [deployment]
│   │   ├───AskFi.Platform.Sequencer.Node               [deployment]
│   │   ├───AskFi.Platform.Broker.Node                  [deployment]
│   │   ├───AskFi.Platform.HistoricQuery.Worker         [deployment]
│   │   ├───AskFi.Platform.HistoricSimulation.Worker    [deployment]
│   │   ├───AskFi.Platform.LiveQuery.Worker             [deployment]
│   │   ├───AskFi.Platform.LiveSimulation.Worker        [deployment]
│   │   ├───AskFi.Platform.LiveStrategy.Worker          [deployment]
│   │   └───AskFi.Platform.WorkloadScheduler            [deployment]
│   ├───state
│   │   ├───AskFi.Platform.StateStorageCell             [library]    (implements WATCH, PUT, GET using Redis PUB/SUB)
│   │   ├───AskFi.Platform.KnowledgeBase.Node           [deployment] (observations, actions, contexts)
│   │   ├───AskFi.Platform.UserState.Node               [deployment] (active queries, active simulations, active live strategy)
│   │   ├───AskFi.Platform.OperatorState.Node           [deployment] (active query-workers (live, historic), simulations (live, historic), observer-daemons, broker-daemons, sequencers)
│   │   └───AskFi.Platform.UserSpace.Node               [deployment] (query results, simulation results, visualizations, live executions)
│   ├───messaging
│   │   ├───AskFi.Platform.Messaging                    [library, C#] (implements RedisPlatformMessaging : IPlatformMessaging; consumed by StateStorageCell)
│   │   └───redis                                       [k8s manifestos]
│   ├───persistence
│   │   ├───AskFi.Platform.Persistence                  [library, C#] (implements CID, STORE, LOAD, PIN, GC using IPFS Cluster, local disk, in-memory cache: IPlatformPersistence)
│   │   └───ipfs-cluster                                [k8s manifestos]
│   ├───operator-api
│   │   ├───AskFi.Platform.OperatorApi.Rest             [deployment]
│   │   ├───AskFi.Platform.OperatorApi.WebSocket        [deployment]
│   ├───user-api
│   │   ├───AskFi.Platform.UserApi.Rest                 [deployment]
│   │   ├───AskFi.Platform.UserApi.WebSocket            [deployment]
│   ├───cli
│   │   ├───AskFi.Cli                                   [executable]
│   │   └───AskFi.Askbot                                [executable]
│   ├───visualization
│   │   ├───AskFi.Visualization.Server                  [deployment] (backend for d3.fs browser frontend)
│   │   └───AskFi.Visualization.Viewer                  [deployment] (static d3.fs browser frontend)
│   ├───studio
│   │   └───AskFi.Studio.Server                         [deployment] (AskFi Studio browser interface)
│   ├───clockwork                                       [k8s manifestos]
│   ├───istio                                           [k8s manifestos]
│   ├───kubernetes-cluster                              [terraform module]
│   └───vault                                           [k8s manifestos]
└───Ask.Network
    ├───contracts
    ├───frontend
    └───hosting
```
