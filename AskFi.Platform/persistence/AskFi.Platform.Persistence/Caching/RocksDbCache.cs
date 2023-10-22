using RocksDbSharp;

namespace AskFi.Runtime.Persistence.Caching;

internal sealed class RocksDbCache
{
    private readonly RocksDb _database;

    public RocksDbCache(DirectoryInfo localPersistenceDirectory)
    {
        var options = new DbOptions().SetCreateIfMissing(true);
        _database = RocksDb.Open(options, localPersistenceDirectory.FullName);
    }

    public byte[]? TryReadFromDisk(ContentId cid)
    {
        try {
            // Todo: Handle case when another process is still writing this file and hasn't completed that operation yet. Try to verify the CID after load. Or use file sharing that locks out every read while write.
            var content = _database.Get(cid.Raw);

            return content;
        } catch (FileNotFoundException) {
            return null;
        } catch (DirectoryNotFoundException) {
            return null;
        }
    }

    public void WriteToDisk(ContentId cid, byte[] raw)
    {
        _database.Put(cid.Raw, raw);
    }
}
