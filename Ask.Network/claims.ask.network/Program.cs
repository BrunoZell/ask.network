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
    // Implement logic to return a specific item's metadata as JSON
    return Results.Ok(new NftMetadata(
        name: "ask.network Treasury Claim #" + id,
        symbol: "ASK-T",
        description: "3.5 SOL deposit certificate in ask.network treasury from 2023-11-16 22:12",
        image: "https://claims.ask.network/" + id + ".svg",
        animation_url: "https://claims.ask.network/" + id + ".glb",
        external_url: "https://claims.ask.network/" + id,
        attributes: new NftAttribute[]
        {
            new NftAttribute("unit_of_value", "SOL"),
            new NftAttribute("deposit_amount", "3.5"),
            new NftAttribute("time_of_deposit", "1700168939")
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