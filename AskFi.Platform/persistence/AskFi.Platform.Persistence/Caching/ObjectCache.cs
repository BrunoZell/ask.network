using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace AskFi.Runtime.Persistence.Caching;

/// <summary>
/// Maintains a mapping from CIDs to a .NET object instance that results from deserializing that CID.
/// </summary>
internal sealed class ObjectCache
{
    private readonly ConcurrentDictionary<byte[], WeakReference> _inMemoryCache = new();

    public void Set(ContentId cid, object obj)
    {
        _inMemoryCache.AddOrUpdate(
            key: cid.Raw,
            addValueFactory: (cid, obj) => new WeakReference(obj),
            updateValueFactory: (cid, weakRef, obj) => {
                weakRef.Target = obj;
                return weakRef;
            },
            factoryArgument: obj);
    }

    public bool TryGet(ContentId cid, [NotNullWhen(true)] out object? cached)
    {
        if (!_inMemoryCache.TryGetValue(cid.Raw, out var weakRef)) {
            cached = null;
            return false;
        }

        cached = weakRef.Target;
        return cached is not null;
    }
}
