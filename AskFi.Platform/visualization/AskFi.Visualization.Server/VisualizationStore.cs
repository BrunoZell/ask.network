using AskFi.Runtime.Persistence;
using AskFi.Runtime.Persistence.Caching;
using Rabot.Market.Visualization;

namespace Rabot.Visualization.Server;

public class VisualizationStore
{
    private readonly SmcVisualizationBuilder _smcVisualizationBuilder;

    public VisualizationStore(SmcVisualizationBuilder smcVisualizationBuilder)
    {
        _smcVisualizationBuilder = smcVisualizationBuilder;
    }

    public async ValueTask<GetLatestCanvasResponse> GetLatest(string name)
    {
        var contextSequenceCid = ContentId.NewContentId(Base32.FromBase32String("CSFDXENIPO3ITEL7MQ5VSFVEJLDELW6PNIYXXNH3GBDYCPTERGJA"));
        var priceTimeGrid = await _smcVisualizationBuilder.Build(contextSequenceCid);

        return new GetLatestCanvasResponse(
            name,
            ordinal: 0,
            fullCanvas: new Canvas(
                width: Pixel.NewPixel(1000),
                height: Pixel.NewPixel(600),
                priceTimeGrid));
    }
}
