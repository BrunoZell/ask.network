# Ask Finance Platform

The AskFi Platform is an additional layer of abstraction that introduces the benefits of the AskFi SDK + Runtime in a managed way, by implementing the necessary infrastructure required to offer it as a hosted service.

## Platform Service Interface

### Messaging

#### Platform Messaging Interface

PUB 'Message [channel]
SUB 'Message [channel] [filter]

_where 'Message can be one of:_

- NewObservation
- NewContext
- NewDecision
- ActionExecuted

Mapped into the platform internal channel space.

### Internal Message Channel Space

```yaml
public:
  newObservation:
  - [ObservationSequenceIdentity_1] -> #NewObservation where '$msg.identity = ObservationSequenceIdentity_1'
    [ObservationSequenceIdentity_n] -> #NewObservation where '$msg.identity = ObservationSequenceIdentity_n'
  newContext:
  - [ContextSequenceIdentity_1] -> #NewContext where ^^^
    [ContextSequenceIdentity_n]
  newDecision:
  - [DecisionSequenceIdentity_1] -> #NewDecision where ^^^
    [DecisionSequenceIdentity_n]
  actionExecuted:
  - [ActionSequenceIdentity_1] -> #ActionExecuted where ^^^
    [ActionSequenceIdentity_n]
internal:
  state:
    watch:
    - [PlatformStateSchemaKeySelector] -> #StateUpdate
  persistence:
    ingest -> #PersistencePut
```

### State

#### Public Platform State

PUT [key] [cid(value)]  _where cid is of same type than specified in the schema for this key_
GET [key]

WATCH [key]
-> implemented as SUB StateUpdate internal.state.[key]

#### Internal Platform State

```yaml
operator:
  observers:
  - identity:
    k8s-worker:
  sequencers:
  - identity:
    k8s-worker:
  brokers:
  - identity:
    k8s-worker:
public:
  observations:
  - identity:
    head:
  contexts:
  - identity:
    history:
    head:
  acts:
  - identity:
    head:
```

#### Platform State Operations

Admin Actions (we when Saas):

- Schedule Observer
- Schedule Sequencer
- Schedule Broker
- Update Secret

Platform operator picks those up, schedules k8s services.

Those services then produce data and integrate them into the platform state with these operations:

Sensory-motricity (platform internal, from user-defined deamons):

- Append observation sequence
- Append context sequence
- Append decision sequence
- Append action sequence

The produced data trail is then analyzed by the user.
He runs queries, designs visualizations, develops strategies, simulates them and puts them live.

User Actions (customer when SaaS):

- Run Query
- View Visualization
- Run Backtest [+Visualization]
- Deploy Live Simulation [+Visualization]
- Deploy Live Strategy [+Visualization]

Those commands are picked up by a continuously running platform operator who schedules the according k8s deployments.
That operator then reports back an abstract state of each process to the platform state so that it can be displayed
in the user interface. Like what strategies are deployed, what queries are running, and all these.

Operator Actions (platform internal, from static k8s operator)

- Observer Deployed
- Broker Deployed
- Sequencer Deployed
- Backtest Deployed
- Live Simulation Deployed
- Live Strategy Updated

User Library:

- Domains [own & subscribed]
- Queries
- Visualizations
- Strategies

User Workspace:

- Analysis (query result)
- Canvas (static price-time gird)
- Backtests
- Live Simulations
- Live Trading

### Persistence

STORE
LOAD
PIN
CID

INGEST '

## State

The platform keeps state information in a dedicated subsystem that is based on etcd.

Important state to keep track of:

- All observers which should operate:
  - domain code (dll with IObserver implementation and required types)
  - configuration (dll with constructor function)
- Context config: Configure named contexts
  - For every name, a set of active observer instances that form the context
- All brokers which should operate:
  - domain code (dll with IBroker implementation and required types)
  - configuration (dll with constructor function and interface into secrets)
- The current live strategy
  - The strategy code itself (or explicit no-op)
  - Name of the context to use as data (from context config)
  - A white list of 'Action the strategy is allowed to execute

## Api

- Update live strategy
- Start/stop live strategy
- Set live context observer set
- Set live broker white list
- Start new observer
- Stop active observer
- Start new broker (replaces active broker of same Action type)
- Stop active broker
