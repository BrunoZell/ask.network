using AskFi.Runtime.Persistence.Caching;
using AskFi.Runtime.Persistence.Encoding;
using AskFi.Runtime.Platform;

namespace AskFi.Runtime.Persistence;

public class IpfsDiskPlatformPersistence : IPlatformPersistence
{
    private readonly ISerializer _serializer;

    private readonly ObjectCache _inMemoryObjectCache = new();
    private readonly RocksDbCache _diskCache;

    public IpfsDiskPlatformPersistence(
        ISerializer serializer,
        DirectoryInfo localPersistenceDirectory)
    {
        _serializer = serializer;
        _diskCache = new(localPersistenceDirectory);
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

        // 3. Write data to disk for persistence
        _diskCache.WriteToDisk(cid, raw);

        // 4. Upload to IPFS Cluster
        // Todo: Call IPFS Cluster API

        return new(cid);
    }
}
