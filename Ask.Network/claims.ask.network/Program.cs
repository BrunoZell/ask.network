using Hexarc.Borsh;
using Solnet.Rpc;

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

app.MapGet("/{id}.json", (int id) =>
{
    var client = new RpcClient("https://api.mainnet-beta.solana.com"); // Adjust to your Solana RPC endpoint

    // Replace `TreasuryClaimAccountAddress` with the address of the TreasuryClaim account
    var accountInfo = await client.GetAccountInfoAsync("TreasuryClaimAccountAddress");
    if (!accountInfo.WasSuccessful)
    {
        // Handle error, account not found or other issues
        return Results.NotFound();
    }

    // Decode the base64-encoded data from the accountInfo
    byte[] decodedData = Convert.FromBase64String(accountInfo.Result.Value.Data[0]);

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
        attributes: new NftAttribute[]
        {
            new NftAttribute("unit_of_value", treasuryClaim.UnitOfValue.ToString()),
            new NftAttribute("deposit_amount", treasuryClaim.DepositAmount.ToString()),
            new NftAttribute("time_of_deposit", treasuryClaim.DepositTimestamp.ToString())
        }
    ));
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

[BorshObject]
public enum TreasuryCurrency
{
    [BorshPropertyOrder(0)]
    SOL, // Solana, in Lamports

    [BorshPropertyOrder(1)]
    USDC,

    [BorshPropertyOrder(2)]
    ETH
}

// Helper method to convert Unix timestamp to DateTime
DateTime UnixTimeStampToDateTime(long unixTimeStamp)
{
    // Unix timestamp is seconds past epoch
    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
    return dtDateTime;
}
