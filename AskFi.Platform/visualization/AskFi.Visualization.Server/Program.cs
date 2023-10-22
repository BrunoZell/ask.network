using AskFi.Runtime.Persistence;
using AskFi.Runtime.Persistence.Encoding;
using AskFi.Runtime.Platform;
using Microsoft.AspNetCore.Mvc;
using Rabot.Market.Visualization;
using Rabot.Visualization.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddSingleton<IPlatformPersistence>(p => {
    return new IpfsDiskPlatformPersistence(new Blake3JsonSerializer(), new DirectoryInfo("C:/Rabot/Persistence-rocks"));
});

builder.Services.AddSingleton<SmcVisualizationBuilder>();
builder.Services.AddSingleton<VisualizationStore>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapGet("/viz/{name}/latest", async (HttpContext context, string name, [FromServices] VisualizationStore store) => {
    var response = await store.GetLatest(name);
    var json = Rabot.Market.Visualization.Serialization.serializeLatest(response);
    await context.Response.WriteAsync(json);
})
.WithName("GetLatestCanvas")
.RequireCors(p => p.AllowAnyOrigin())
.WithOpenApi();

app.MapGet("/viz/{name}/since/{ordinal}", async (HttpContext context, string name, long ordinal) => {
    await context.Response.WriteAsJsonAsync(
        new GetDifferentialCanvasResponse(
        name,
        since: 0,
        ordinal: ordinal + 1,
        differential: new DifferentialCanvas(
            width: Pixel.NewPixel(800),
            height: Pixel.NewPixel(500),
            priceTimeGrid: null)));
})
.WithName("GetDifferentialCanvas")
.RequireCors(p => p.AllowAnyOrigin())
.WithOpenApi();

app.Run();
