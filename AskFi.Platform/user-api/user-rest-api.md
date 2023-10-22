# User REST API

Todo: Turn into OpenAPI spec

Data:

```text
GET /api/query/list       [{ name: string; query-id: B32QYiaus; parameters: [ exchange: string; instrument: string ]; result: [high: number; low: number]}
POST /api/query/{name}    { code: string; type: string; name: string; } -> #Query
DELETE /api/query/{name}

GET /api/visualization/list       [{ name: string; viz-id: B32QYiaus; parameters: [ exchange: string; instrument: string ]}
POST /api/visualization/{name}    { code: string; type: string; name: string; } -> #Visualization
DELETE /api/visualization/{name}

GET /api/strategy/list       [{ name: string; strategy-id: B32QYiaus; parameters: [ exchange: string; instrument: string ]; actions: ["AskFi:PlaceMarketOrder"]}
POST /api/strategy/{name}    { code: string; type: string; name: string; } -> #Strategy
DELETE /api/strategy/{name}
```

Behavior:

```text
POST /api/query/{name}/run    { query: #query-id; parameters: {}; at: #DateTime; }
GET /api/query/{name}/run/list
GET /api/query/{name}/run/{run-id}
WS /api/query/{name}/stream?p=[..;..]   > yielding 'Result

POST /api/visualization/{name}/run    { query: #viz-id; parameters: {}; at: #DateTime; }
GET /api/visualization/{name}/run/list
GET /api/visualization/{name}/run/{run-id}
WS /api/visualization/{name}/stream?p=[..;..]`   > yielding Canvas

GET /api/backtest/list
POST /api/backtest          { strategy: #strategy-id; parameters: {}; from: #DateTime; to: #DateTime; }
GET /api/backtest/{backtest-id}
DELETE /api/backtest/{backtest-id}

GET /api/simulation/list
POST /api/simulation            { strategy: #strategy-id; parameters: {}; }
GET /api/simulation/{simulation-id}
DELETE /api/simulation/{simulation-id}
WS /api/strategy/{name}/stream?p=[..;..]&q=B32asdoer&qp=[..]`   > yielding 'Result from result query q

GET /api/live/list            -> [#session-id-1; #session-id-2; ]
POST /api/live/deploy       { strategy: #strategy-id; parameters: {}; } -> { session-id: #session-id; }
GET /api/live/{session-id}            -> [#session-id-1; #session-id-2; ]
DELETE /api/live/{session-id}
```

## More Details Data API Endpoints

This API is used to manage user code. User code is queries, visualizations, and strategies.

They all are posted as F# code, compiled and verified on the servier, with an identifier being returned to the user which is a handle on the user code. That handle is later used to reference user code in analysis workloads.

### User Queries

#### `GET /api/query/list`

Response:

```json
{
    list: [
        {
            name: string,
            query-id: B32QYiaus,
            parameter-scheme: { exchange: string, instrument: string },
            result-scheme: { high: number, low: number }
        }
    ]
}
```

#### `POST /api/query/{name}`

Request:

```json
{
    code: string;
    type: string;
    name: string;
}
```

Response:

```json
{
    name: string,
    query-id: B32QYiaus,
    parameter-scheme: { exchange: string, instrument: string },
    result-scheme: { high: number, low: number }
}
```

#### `DELETE /api/query/{name}`

Response: 200 OK

### User Visualizations

#### `GET /api/visualization/list`

Response:

```json
{
    list: [
        {
            name: string,
            visualization-id: B32QYiaus,
            parameter-scheme: { exchange: string, instrument: string }
        }
    ]
}
```

#### `POST /api/visualization/{name}`

Request:

```json
{
    code: string;
    type: string;
    name: string;
}
```

- Compiles F# code
- Obtains dll
- Verifies function has correct signature to qualify as a visualization: `'Parameters -> Context -> Canvas`
- Extract `'Parameters` type schema (mapped to CUE primitive valie lattice), put in response

Response:

```json
{
    name: string,
    visualization-id: B32QYiaus,
    parameter-scheme: { exchange: string, instrument: string }
}
```

Or HTTP 422 (Unprocessable Entity) if:

- F# does not compile
- wrong signature
- `'Parameters` do not recursively resolve to CUE primitive values

#### `DELETE /api/visualization/{name}`

Response: 200 OK

### User Strategies

GET /api/strategy/list       [{ name: string; strategy-id: B32QYiaus; parameters: [ exchange: string; instrument: string ]; actions: ["AskFi:PlaceMarketOrder"]}
POST /api/strategy/{name}    { code: string; type: string; name: string; } -> #Strategy
DELETE /api/strategy/{name}

#### `GET /api/strategy/list`

Response:

```json
{
    list: [
        {
            name: string,
            strategy-id: B32QYiaus,
            parameter-scheme: { exchange: string, instrument: string },
            actions: ["Binance:PlaceMarketOrder"] // v1: 'Action type names, v2: CUE constraints on 'Action types
        }
    ]
}
```

#### `POST /api/strategy/{name}`

- Compiles F# code
- Obtains dll
- Verifies function has correct signature to qualify as a visualization: `'Parameters -> Reflection -> Context -> Decision`
- Extract `'Parameters` type schema (mapped to CUE primitive valie lattice), put in response

If compiled & valid, it overwrites any previously uploaded strategy with the same name.
This creates a versioning historiy for strategies.
The user edits a strategy under a name in the UI. Whenever he saves it, it's pushed to the server and is updated accordingly.

Request:

```json
{
    code: string;
    type: string;
    name: string;
}
```

Response:

```json
{
    name: string,
    query-id: B32QYiaus,
    parameter-scheme: { exchange: string, instrument: string },
    actions: ["Binance:PlaceMarketOrder"]
}
```

Or HTTP 422 (Unprocessable Entity) if:

- F# does not compile
- wrong signature
- `'Parameters` do not recursively resolve to CUE primitive values

#### `DELETE /api/strategy/{name}`

Response: 200 OK
