using System.Diagnostics;
using AskFi.Runtime;
using AskFi.Runtime.Modules.Context;
using AskFi.Runtime.Persistence;
using AskFi.Runtime.Platform;
using Microsoft.FSharp.Core;
using Rabot.Bybit;
using Rabot.Market;
using Rabot.Market.Visualization;
using Rabot.SmartMoneyConcepts;
using static AskFi.Runtime.DataModel;
using static AskFi.Sdk;

namespace Rabot.Visualization.Server;

public class SmcVisualizationBuilder
{
    private readonly IPlatformPersistence _persistence;

    public SmcVisualizationBuilder(IPlatformPersistence persistence) =>
        _persistence = persistence;

    public async ValueTask<PriceTimeGrid> Build(ContentId tradeContextSequenceHead)
    {
        var allObservations = await IterateObservationsInContext(_persistence, tradeContextSequenceHead)
            .SelectAwait(async o => await _persistence.Get<Observation<TradeV5.Percept>>(o.Observation))
            .ToListAsync();

        var allTrades = allObservations
            .SelectMany(observation => observation.Percepts)
            .ToList();

        Console.WriteLine($"Loaded {allTrades.Count} trades.");

        var tickSize = 0.1m;
        var earliestTime = allTrades.Select(t => t.Time).Min();
        var latestTime = allTrades.Select(t => t.Time).Max();
        var lowestPrice = allTrades.Select(t => t.Price).Min();
        var highestPrice = allTrades.Select(t => t.Price).Max();

        var tradeObjects = allTrades
            .Select(t => PriceTimeObject.NewCircle(
                _center: new Tuple<Price, Timestamp>(Price.FromTicks(tickSize, t.Price), Timestamp.NewTimestamp(t.Time)),
                _width: Pixel.NewPixel(3),
                _color: Color.NewColor("#326446")))
            .ToList();

        var query = QueryContext.Load(tradeContextSequenceHead, _persistence);
        var context = new Context(query);
        var latestTrade = context.Query.latest<TradeV5.Percept>();

        if (!FSharpOption<CapturedObservation<TradeV5.Percept>>.get_IsSome(latestTrade)) {
            // There is no observation of type trade. So no visualization.
            throw new InvalidOperationException("Need at least one trade");
        }

        var actualNow = latestTrade.Value.At;
        var candleInterval = TimeSpan.FromMinutes(3);
        var chartWidth = latestTime - earliestTime;

        var series = new Candles.CandleSeriesIdentity(
            trades: new Trades.TradeSeriesIdentity(Trades.Exchange.Bybit, "BTCUSDT"),
            referenceTimestamp: new DateTime(2007, 01, 01, 00, 00, 00, DateTimeKind.Utc),
            duration: candleInterval);

        var openCandleIndex = Candles.candleIndex(series, actualNow);
        var firstCandleIndex = Candles.candleIndex(series, actualNow - chartWidth);

        // Draw all candles
        for (long i = firstCandleIndex; i < openCandleIndex; i++) {
            var candleProbeId = new Candles.CandleIdentity(series, i);
            var candleProbe = Candles.get(candleProbeId, context);

            if (!FSharpOption<Candles.Candle>.get_IsSome(candleProbe)) {
                continue;
            }

            var candle = candleProbe.Value;
            var candleMidTimestamp = Candles.midTimestamp(candleProbeId);
            var wicks = PriceTimeObject.NewLine(
                _start: new Tuple<Price, Timestamp>(Price.FromTicks(tickSize, candle.High), Timestamp.NewTimestamp(candleMidTimestamp)),
                _end: new Tuple<Price, Timestamp>(Price.FromTicks(tickSize, candle.Low), Timestamp.NewTimestamp(candleMidTimestamp)),
                _color: Color.NewColor("#326446"));

            var body = PriceTimeObject.NewArea(
                _width: new Market.Visualization.TimeRange(
                    start: Timestamp.NewTimestamp(Candles.openTimestamp(candleProbeId)),
                    end: Timestamp.NewTimestamp(Candles.closeTimestamp(candleProbeId))),
                _heigth: new PriceRange(
                    from: Price.FromTicks(tickSize, candle.Open),
                    to: Price.FromTicks(tickSize, candle.Close)),
                _color: candle.Open < candle.Close
                    ? Color.NewColor("#00bf06") // up
                    : Color.NewColor("#c74e1e")); // down

            tradeObjects.Add(wicks);
            tradeObjects.Add(body);

            var candleProgress = i / (float)(openCandleIndex - firstCandleIndex);
            Console.WriteLine($"Candle progress: {candleProgress:%}");
        }

        // Draw all pullbacks
        for (long i = firstCandleIndex; i < openCandleIndex; i++) {
            var candleProbeId = new Candles.CandleIdentity(series, i);
            var candleMidTimestamp = Candles.midTimestamp(candleProbeId);
            var lowPullback = Pullback.isLowPullback(candleProbeId, context);

            if (lowPullback is Pullback.LowPullbackProbe.Valid validLowPullback) {
                var lowPullbackMarker = PriceTimeObject.NewCircle(
                    _center: new Tuple<Price, Timestamp>(Price.FromTicks(tickSize, validLowPullback.price), Timestamp.NewTimestamp(candleMidTimestamp)),
                    _width: Pixel.NewPixel(25),
                    _color: Color.NewColor("rgba(50, 137, 168, 50)"));

                tradeObjects.Add(lowPullbackMarker);
            }

            var highPullback = Pullback.isHighPullback(candleProbeId, context);

            if (highPullback is Pullback.HighPullbackProbe.Valid validHighPullback) {
                var highPullbackMarker = PriceTimeObject.NewCircle(
                    _center: new Tuple<Price, Timestamp>(Price.FromTicks(tickSize, validHighPullback.price), Timestamp.NewTimestamp(candleMidTimestamp)),
                    _width: Pixel.NewPixel(25),
                    _color: Color.NewColor("rgba(252, 186, 3, 50)"));

                tradeObjects.Add(highPullbackMarker);
            }
        }

        var grid = new PriceTimeGrid(
            tickSize,
            earliestTime: Timestamp.NewTimestamp(earliestTime),
            latestTime: Timestamp.NewTimestamp(latestTime),
            lowestPrice: Price.FromTicks(tickSize, lowestPrice),
            highestPrice: Price.FromTicks(tickSize, highestPrice),
            objects: tradeObjects.ToArray());

        return grid;
    }

    private static async IAsyncEnumerable<CapturedObservation> IterateObservationsInContext(IPlatformPersistence persistence, ContentId contextSequenceHead)
    {
        var head = await persistence.Get<ContextSequenceHead>(contextSequenceHead);

        while (true) {
            if (head is ContextSequenceHead.Context context) {
                yield return context.Node.Observation;

                // Load previous node in tree
                head = await persistence.Get<ContextSequenceHead>(context.Node.Previous);
                continue;
            }

            if (head.IsIdentity) {
                yield break;
            } else {
                Debug.Fail("ContextSequenceHead should only have two cases: Identity | Context");
                yield break;
            }
        }
    }

    // Later:
    // Expose api for visualization workloads to send latest or historic visualizations to.
    // This service caches them in-memory (v1) and pins them in IPFS Cluster (v2).
    // It proactively builds differential canvases for each available visualization.
    // Those diffs are then served via http to the frontend running d3.fs and F# Fable.
}
