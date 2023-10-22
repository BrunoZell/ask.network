using Blake3;
using Newtonsoft.Json;

namespace AskFi.Runtime.Persistence.Encoding;

public class Blake3JsonSerializer : ISerializer
{
    public (ContentId Cid, byte[] Raw) Serialize<TDatum>(TDatum datum)
    {
        var json = JsonConvert.SerializeObject(datum);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        var hash = Hasher.Hash(bytes);
        var hashRaw = hash.AsSpanUnsafe().ToArray();
        return (ContentId.NewContentId(hashRaw), bytes);
    }

    public TDatum Deserialize<TDatum>(ContentId cid, byte[] raw)
    {
        var json = System.Text.Encoding.UTF8.GetString(raw);
        var datum = JsonConvert.DeserializeObject<TDatum>(json);
        return datum!;
    }
}
