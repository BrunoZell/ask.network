namespace AskFi.Runtime.Persistence

open System.Runtime.CompilerServices

// Todo: Make this an IPFS CID respecting multihash and multicodec
[<IsReadOnly; Struct>]
type ContentId = ContentId of Raw: byte array with
    static member Zero
        with get() = ContentId <| Array.zeroCreate<byte> 0


// DataModel.KnowledgeBase.CapturedObservation [Sdk.Observation<'P>, 'P]
// DataModel.KnowledgeBase.ObservationSequenceHead
// DataModel.KnowledgeBase.ActionSequenceHead
// DataModel.KnowledgeBase.ActionSequenceHead
// DataModel.KnowledgeBase.ContextSequenceHead
// DataModel.KnowledgeBase.ContextHistory
// DataModel.KnowledgeBase
// DataModel.UserSpace.QueryResultHead
// DataModel.UserSpace.DecisionSequenceHead
// DataModel.UserSpace.SimulationSequenceHead
// DataModel.UserSpace.VisualizationSequenceHead
// DataModel.UserSpace.ExecutionSequenceHead

type IPlatformPersistence =
    abstract member Cid<'TDatum> : 'TDatum -> ContentId
    abstract member Load<'TDatum> : ContentId -> ValueTask<'TDatum>
    abstract member Store<'TDatum> : 'TDatum -> ValueTask<ContentId>
    abstract member Pin : ContentId -> ValueTask<bool>
