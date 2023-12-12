namespace Ask.Node

open Ask.Host.Persistence
open System

type ActiveQuery =
    | ContinuousHistoric of from:DateTime * ``to``:DateTime * query:Query * parameters:ContentId<unit>
    | OneShotHistoric of at:DateTime * query:Query * parameters:ContentId<unit>
    | OneShotLive of query:Query * parameters:ContentId<unit>
    | ContinuousLive of query:Query * parameters:ContentId<unit>

type ActiveVisualization =
    | VisualizationRun of at:DateTime * visualization:Visualization

type BacktestSession =
    | Backtest of strategies:Map<string, Strategy> * from:DateTime * ``to``:DateTime

type SimulationSession =
    | Simulation of strategies:Map<string, Strategy>

type LiveSession =
    | LiveDeploy of strategy:Strategy

// User-requested compute currently executing as part of active RPC requests
type UserRequests = {
    ActiveQueries: ContentId<ActiveQuery> list
    ActiveVisualizations: ContentId<ActiveVisualization> list
    BacktestSessions: ContentId<BacktestSession> list
    SimulationSessions: ContentId<SimulationSession> list
    LiveSession: ContentId<LiveSession>
}
