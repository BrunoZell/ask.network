namespace Ask.Node
open Ask.Host.Persistence
open System
open Ask.DataModel

type ActiveObserver = {
    StartTimestamp: DateTime
    ActionSequenceId: ObservationSequenceHead<_> // union case ObservationSequenceHead.Identity
    Type: Observer
    Parameters: ContentId<unit>
}

type ActiveBroker = {
    StartTimestamp: DateTime
    ActionSequenceId: ActionSequenceHead<_, _> // union case ActionSequenceHead.Identity
    Type: Broker
    Parameters: ContentId<unit>
}

type ActiveSequencer = {
    StartTimestamp: DateTime
    Slug: string
    ContextSequenceId: ContextSequenceHead<_, _, _> // union case ContextSequenceHead.Identity
    ConsumingObservers: ContentId<ActiveObserver> list
    ConsumingBrokers: ContentId<ActiveBroker> list
}

// User-requested compute at IO boundary
type OperatorState = {
    ActiveObservers: ContentId<ActiveObserver> list
    ActiveSequencers: ContentId<ActiveSequencer> list
    ActiveBrokers: ContentId<ActiveBroker> list
}
