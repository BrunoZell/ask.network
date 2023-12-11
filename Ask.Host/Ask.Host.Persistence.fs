namespace Ask.Host.Persistence

open System.Runtime.CompilerServices
open System.Threading.Tasks

// Todo: Make this an IPFS CID respecting multihash and multicodec
[<IsReadOnly; Struct>]
type ContentId<'TDatum> = ContentId of Raw: byte array with
    static member Zero
        with get() = ContentId <| Array.zeroCreate<byte> 0

type IHostPersistence =
    abstract member Cid<'TDatum> : 'TDatum -> ContentId<'TDatum>
    abstract member Load<'TDatum> : ContentId<'TDatum> -> ValueTask<'TDatum>
    abstract member Store<'TDatum> : 'TDatum -> ValueTask<ContentId<'TDatum>>
    abstract member Pin<'TDatum> : ContentId<'TDatum> -> ValueTask<bool>
