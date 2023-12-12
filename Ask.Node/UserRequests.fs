module Ask.Node.UserRequests
open Ask.Host.Persistence
open System

type ActiveQuery =
    | ContinuousHistoric of from:DateTime * ``to``:DateTime * query:CodeId * parameters:ContentId<unit>
    | OneShotHistoric of at:DateTime * query:CodeId * parameters:ContentId<unit>
    | OneShotLive of query:CodeId * parameters:ContentId<unit>
    | ContinuousLive of query:CodeId * parameters:ContentId<unit>

type ActiveVisualization =
    | VisualizationRun of at:DateTime * visualization:CodeId

type BacktestSession =
    | Backtest of strategies:Map<string, CodeId> * from:DateTime * ``to``:DateTime

type SimulationSession =
    | Simulation of strategies:Map<string, CodeId>

type LiveSession =
    | LiveDeploy of strategy:CodeId

// User-requested compute currently executing as part of active RPC requests
type UserRequests = {
    ActiveQueries: ContentId<ActiveQuery> list
    ActiveVisualizations: ContentId<ActiveVisualization> list
    BacktestSessions: ContentId<BacktestSession> list
    SimulationSessions: ContentId<SimulationSession> list
    LiveSession: ContentId<LiveSession>
}
