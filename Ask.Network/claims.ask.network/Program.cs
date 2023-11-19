using System.Security.Cryptography;
using System.Text;
using Hexarc.Borsh;
using Hexarc.Borsh.Serialization;
using Solnet.Rpc;
using Solnet.Rpc.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/collection.json", () =>
{
    return Results.Ok(new NftMetadata(
        name: "ask.network Treasury Claims",
        symbol: "ASK-T",
        description: "Certificate of deposit into ask.network treasury",
        image: "https://claims.ask.network/collection.svg",
        animation_url: "https://claims.ask.network/collection.glb",
        external_url: "https://claims.ask.network",
        attributes: Array.Empty<NftAttribute>()
    ));
})
.WithName("CollectionJson")
.WithOpenApi();

app.MapGet("/{id}.json", async (ulong id) =>
{
    var client = ClientFactory.GetClient(Cluster.DevNet, logger: null);

    // Calculate the anchor discriminator for TreasuryClaim
    byte[] discriminator = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("TreasuryClaim"))[0..8];

    // Set up the filters
    var filters = new List<MemCmp>
    {
        new() { Offset = 0, Bytes = Convert.ToBase64String(discriminator) }, // Filter for Anchor discriminator (all accounts of type 'TreasuryClaim')
        new() { Offset = 8, Bytes = Convert.ToBase64String(BitConverter.GetBytes(id)) } // Filter for ordinal
    };

    // Fetch the accounts
    var matchingAccounts = await client.GetProgramAccountsAsync("AKVXMk2HpyozBHvMc66jDNRdKMbq2oCzdWBNx64mZsc1", memCmpList: filters);
    var treasuryClaimAccountKey = matchingAccounts.Result.FirstOrDefault();

    if (matchingAccounts.WasSuccessful && treasuryClaimAccountKey is not null)
    {
        // Decode the base64-encoded data from the accountInfo
        byte[] decodedData = Convert.FromBase64String(treasuryClaimAccountKey.Account.Data[0]);

        // Deserialize the data into the TreasuryClaim structure
        var treasuryClaim = BorshSerializer.Deserialize<TreasuryClaim>(decodedData);

        // Validate if the claim ID exists
        if (treasuryClaim.Ordinal != id)
        {
            return Results.NotFound("Claim ID not found");
        }

        return Results.Ok(new NftMetadata(
            name: "ask.network Treasury Claim #" + treasuryClaim.Ordinal,
            symbol: "ASK-T",
            description: $"SOL deposit certificate in ask.network treasury from {UnixTimeStampToDateTime(treasuryClaim.DepositTimestamp)}",
            image: $"https://claims.ask.network/{id}.svg",
            animation_url: $"https://claims.ask.network/{id}.glb",
            external_url: $"https://claims.ask.network/{id}",
            attributes:
            [
                new("unit_of_value", treasuryClaim.UnitOfValue.ToString()),
                new("deposit_amount", treasuryClaim.DepositAmount.ToString()),
                new("time_of_deposit", treasuryClaim.DepositTimestamp.ToString())
            ]
        ));
    }
    else
    {
        return Results.NotFound("No matching TreasuryClaim account found.");
    }
})
.WithName("ClaimJson")
.WithOpenApi();

app.MapGet("/collection.svg", () =>
{
    // Implement logic to return the entire collection as SVG
    return Results.Ok(/* SVG data */);
})
.WithName("CollectionSvg")
.WithOpenApi();

app.MapGet("/{id}.svg", (int id) =>
{
    // Implement logic to return a specific item's SVG
    return Results.Ok(/* SVG data for item id */);
})
.WithName("ClaimSvg")
.WithOpenApi();

app.Run();

// Helper method to convert Unix timestamp to DateTime
static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
{
    // Unix timestamp is seconds past epoch
    DateTime dtDateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
    return dtDateTime;
}

/// <summary>
/// Represents the metadata of an NFT asset.
/// </summary>
record NftMetadata(
    // Name of the asset.
    string name,
    // Symbol of the asset.
    string symbol,
    // Description of the asset.
    string description,
    // URI pointing to the asset's logo.
    string image,
    // URI pointing to the asset's animation.
    string animation_url,
    // URI pointing to an external URL defining the asset.
    string external_url,
    // Array of attributes defining the characteristics of the asset.
    NftAttribute[] attributes);

/// <summary>
/// Represents an attribute of an NFT asset.
/// </summary>
record NftAttribute(
    // The type of attribute.
    string trait_type,
    // The value for that attribute.
    string value);

[BorshObject]
public class TreasuryClaim
{
    [BorshPropertyOrder(0)]
    public ulong Ordinal { get; set; }

    [BorshPropertyOrder(1)]
    public TreasuryCurrency UnitOfValue { get; set; }

    [BorshPropertyOrder(2)]
    public ulong DepositAmount { get; set; }

    [BorshPropertyOrder(3)]
    public long DepositTimestamp { get; set; }
}

public enum TreasuryCurrency
{
    SOL, // Solana, in Lamports
    USDC,
    ETH
}
