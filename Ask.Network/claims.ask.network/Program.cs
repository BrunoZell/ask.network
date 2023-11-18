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
    // Implement logic to return the entire collection as JSON
    return Results.Ok(/* collection data */);
})
.WithName("CollectionJson")
.WithOpenApi();

app.MapGet("/{id}.json", (int id) =>
{
    // Implement logic to return a specific item's metadata as JSON
    return Results.Ok(new NftMetadata(
        Name: "ask.network Treasury Claim #" + id,
        Description: "3.5 SOL deposit certificate in ask.network treasury from 2023-11-16 22:12",
        Image: "https://claims.ask.network/" + id + ".svg",
        AnimationUrl: "https://claims.ask.network/" + id + ".glb",
        ExternalUrl: "https://claims.ask.network/" + id,
        Attributes: new Attribute[]
        {
            new Attribute("unit_of_value", "SOL"),
            new Attribute("deposit_amount", 3.5),
            new Attribute("time_of_deposit", "1700168939")
        }
    ));
})
.WithName("ClaimJson")
.WithOpenApi();

app.Run();

record NftMetadata(
    string Name,
    string Description,
    string Image,
    string AnimationUrl,
    string ExternalUrl,
    Attribute[] Attributes);

record Attribute(string TraitType, object Value);
