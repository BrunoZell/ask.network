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
│   │   ├───AskFi.Platform.Observer.Node                [deployment] (hosts Runtime.ObserverGroup; Sdk.IObserver; produces DataModel.ObservationSequence)
│   │   ├───AskFi.Platform.Sequencer.Node               [deployment] (hosts Runtime.Sequencer; produces DataModel.ContextSequence)
│   │   ├───AskFi.Platform.Broker.Node                  [deployment] (hosts Runtime.BrokerGroup; Sdk.IBroker; produces DataModel.ActionSequence)
│   │   ├───AskFi.Platform.HistoricQuery.Worker         [deployment]
│   │   ├───AskFi.Platform.HistoricSimulation.Worker    [deployment]
│   │   ├───AskFi.Platform.LiveQuery.Worker             [deployment] (hosts Runtime.LiveQuery; produces DataModel.QueryResultSequence)
│   │   ├───AskFi.Platform.LiveSimulation.Worker        [deployment] (hosts Runtime.LiveSimulation; produces DataModel.SimulationSequence)
│   │   ├───AskFi.Platform.LiveStrategy.Worker          [deployment]
│   │   └───AskFi.Platform.WorkloadScheduler            [deployment]
│   ├───state
│   │   ├───AskFi.Platform.StateStorageCell             [library]    (implements WATCH, PUT, GET using Redis PUB/SUB)
│   │   ├───AskFi.Platform.KnowledgeBase.Node           [deployment] (gossiping AskFi.DataModel.KnowledgeBase CRDT within permissioned platform bounds: observations, actions, contexts)
│   │   ├───AskFi.Platform.OperatorState                [library]    (CRDT state of: active query-workers (live, historic), simulations (live, historic), observer-daemons, broker-daemons, sequencers)
│   │   ├───AskFi.Platform.UserState                    [library]    (CRDT state of: active queries, active simulations, active live strategy)
│   │   ├───AskFi.Platform.UserSpace                    [library]    (CRDT state of: query results, simulation results, visualizations, live executions)
│   │   ├───AskFi.Platform.OperatorState.Node           [deployment] (gossiping operator state CRDT within permissioned platform bounds)
│   │   ├───AskFi.Platform.UserState.Node               [deployment] (gossiping user state CRDT within permissioned platform bounds)
│   │   └───AskFi.Platform.UserSpace.Node               [deployment] (gossiping user space CRDT within permissioned platform bounds)
│   ├───messaging
│   │   ├───AskFi.Platform.Messaging                    [library, C#] (implements RedisPlatformMessaging : IPlatformMessaging; consumed by StateStorageCell)
│   │   └───redis                                       [k8s manifestos]
│   ├───persistence
│   │   ├───AskFi.Platform.Persistence                  [library, C#] (implements CID, STORE, LOAD, PIN, GC using IPFS Cluster, local disk, in-memory cache: IPlatformPersistence)
│   │   └───ipfs-cluster                                [k8s manifestos]
│   ├───operator-api
│   │   ├───AskFi.Platform.OperatorApi.Models           [library]    (data models for HTTP and WebSocket operator APIs)
│   │   ├───AskFi.Platform.OperatorApi.Rest             [deployment] (mutable CURD interface into platform-bounded operator state CRDT)
│   │   ├───AskFi.Platform.OperatorApi.WebSocket        [deployment] (stream-based interface into platform-bounded operator state CRDT)
│   ├───user-api
│   │   ├───AskFi.Platform.UserApi.Models               [library]    (data models for HTTP and WebSocket user APIs)
│   │   ├───AskFi.Platform.UserApi.Rest                 [deployment] (mutable CRUD interface into platform-bounded user state CRDT)
│   │   ├───AskFi.Platform.UserApi.WebSocket            [deployment] (stream-based interface into platform-bounded user state CRDT)
│   ├───cli
│   │   ├───AskFi.Cli                                   [executable] (CLI wrapper into OperatorApi and UserApi)
│   │   └───AskFi.Askbot                                [executable] (CLI to stream ExecutionSequences from UserApi and immediately execute them via inline Runtime.BrokerGroup; Sdk.IBroker)
│   ├───visualization
│   │   ├───AskFi.Visualization.Server                  [deployment] (backend for d3.fs browser frontend, exposing DataModel.VisualizationSequence in browser-processable manner)
│   │   └───AskFi.Visualization.Viewer                  [deployment] (static d3.fs browser frontend, connecting to AskFi.Visualization.Server via WebSocket)
│   ├───studio
│   │   └───AskFi.Studio.Server                         [deployment] (AskFi Studio browser interface, wrapping UserApi, iframing VisualizationViewer)
│   ├───clockwork                                       [k8s manifestos]
│   ├───istio                                           [k8s manifestos]
│   ├───kubernetes-cluster                              [terraform module]
│   └───vault                                           [k8s manifestos]
└───Ask.Network
    ├───contracts
    ├───frontend
    └───hosting
```
