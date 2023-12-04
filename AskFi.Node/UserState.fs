module AskFi.Node.UserState

type UserState = {
    Users: Map<string, UserData>
}

type UserData = {
    // User-defined code
    Queries: Query list
    Strategies: Strategy list
    Visualizations: Visualization list

    // User-requested compute
    ActiveQueries: ContentId<ActiveQuery> list
    ActiveVisualizations: ContentId<ActiveVisualization> list
    BacktestSessions: ContentId<BacktestSession> list
    SimulationSessions: ContentId<SimulationSession> list
    LiveSession: ContentId<LiveSession>
}

type Query = {
    Name: string
    Code: CodeId
}

type Strategy = {
    Name: string
    Code: CodeId
}

type Visualization = {
    Name: string
    Code: CodeId
}

type ActiveQuery =
    | ContinuousHistoric of from:DateTime * to:DateTime * query:CodeId * parameters:ContentId
    | OneShotHistoric of at:DateTime * query:CodeId * parameters:ContentId
    | OneShotLive of query:CodeId * parameters:ContentId
    | ContinuousLive of query:CodeId * parameters:ContentId

type ActiveVisualization =
    | VisualizationRun of at:DateTime * visualization:CodeId

type BacktestSession =
    | Backtest of strategies:Map<string, CodeId> * from:DateTime * to:DateTime

type SimulationSession =
    | Simulation of strategies:Map<string, CodeId>

type LiveSession =
    | LiveDeploy of strategy:CodeId
