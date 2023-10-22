using AskFi.Runtime.Messages;
using AskFi.Runtime.Persistence.Caching;
using AskFi.Runtime.Persistence.Encoding;
using AskFi.Runtime.Platform;

namespace AskFi.Runtime.Persistence;

public class PushingIpfsDiskPlatformPersistence : IPlatformPersistence, IAsyncDisposable
{
    private readonly ISerializer _serializer;
    private readonly IPlatformMessaging _messaging;
    private readonly Task _platformPutListener;
    private readonly CancellationTokenSource _cancellation = new();

    private readonly ObjectCache _inMemoryObjectCache = new();
    private readonly RocksDbCache _diskCache;

    public PushingIpfsDiskPlatformPersistence(
        ISerializer serializer,
        DirectoryInfo localPersistenceDirectory,
        IPlatformMessaging messaging)
    {
        _serializer = serializer;
        _messaging = messaging;
        _diskCache = new(localPersistenceDirectory);
        _platformPutListener = Task.Run(ListenToPersistentPutMessages);
    }

    public ContentId Cid<TDatum>(TDatum datum)
    {
        // Generate CID locally
        var (cid, raw) = _serializer.Serialize(datum);

        // Insert into in-memory cid->obj mapping for future GET requests on that CID.
        _inMemoryObjectCache.Set(cid, datum);

        return cid;
    }

    public ValueTask<TDatum> Get<TDatum>(ContentId cid)
    {
        // 1. Try read from in-memory cid->obj mapping
        if (_inMemoryObjectCache.TryGet(cid, out var c) && c is TDatum cached) {
            return new(cached);
        }

        // 2. Try read from local disk
        var fromDisk = _diskCache.TryReadFromDisk(cid);
        if (fromDisk is not null) {
            // Deserialize loaded raw data
            var datum = _serializer.Deserialize<TDatum>(cid, fromDisk);

            // Insert into in-memory cid->obj mapping for future GET requests on that CID.
            _inMemoryObjectCache.Set(cid, datum);

            return new(datum);
        }

        // 3. Try read from IPFS Cluster

        throw new NotImplementedException("Reading from IPFS Cluster is not yet implemented.");
    }

    public ValueTask<bool> Pin(ContentId cid)
    {
        // Todo: Pin in IPFS Cluster
        return new(false);
    }

    public ValueTask<ContentId> Put<TDatum>(TDatum datum)
    {
        // 1. Generate CID and raw bytes locally
        var (cid, raw) = _serializer.Serialize(datum);

        // 2. Insert into in-memory cid->obj mapping for future GET requests on that CID.
        _inMemoryObjectCache.Set(cid, datum);

        // 3. Broadcast PUT < 4KB via platform message 'PersistencePut' (if payload is below 4KB)
        if (raw.Length < 1024 * 4) {
            _messaging.Emit(new PersistencePut(cid, raw, typeof(TDatum)));
        }

        // 4. Write data to disk for persistence
        _diskCache.WriteToDisk(cid, raw);

        // 5. Upload to IPFS Cluster
        // Todo: Call IPFS Cluster API

        return new(cid);
    }

    private async Task ListenToPersistentPutMessages()
    {
        while (true) {
            _cancellation.Token.ThrowIfCancellationRequested();

            // Continuously listen to persistence put platform messages and store puts in this instances memory cache.
            await foreach (var put in _messaging.Listen<PersistencePut>(_cancellation.Token)) {
                // Call DeserializeAndLoad<TDatum> with TDatum = put.TDatum
                var method = typeof(PushingIpfsDiskPlatformPersistence).GetMethod(nameof(DeserializeAndLoad));
                var generic = method!.MakeGenericMethod(put.TDatum);
                generic.Invoke(this, new object[] { put.Cid, put.Content });
            }
        }
    }

    /// <summary>
    /// Deserializes received content and stores it in the in memory object cache
    /// </summary>
    private void DeserializeAndLoad<TDatum>(ContentId cid, byte[] content)
    {
        // Deserialize loaded raw data
        var datum = _serializer.Deserialize<TDatum>(cid, content);

        // Insert into in-memory cid->obj mapping for future GET requests on that CID.
        _inMemoryObjectCache.Set(cid, datum);
    }

    public async ValueTask DisposeAsync()
    {
        _cancellation.Cancel();
        await _platformPutListener;
    }
}
