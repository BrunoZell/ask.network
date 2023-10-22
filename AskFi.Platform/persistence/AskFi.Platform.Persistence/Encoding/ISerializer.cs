namespace AskFi.Runtime.Persistence.Encoding;

/// <summary>
/// Defines how to translate between an in-process .NET object and raw bytes addressed via a CID.
/// A serializer must define an encoding (translate between .NET object and bytes) and a hash
/// function that is used to generate the CID.
/// </summary>
public interface ISerializer
{
    (ContentId Cid, byte[] Raw) Serialize<TDatum>(TDatum datum);
    TDatum Deserialize<TDatum>(ContentId cid, byte[] raw);
}
