module Rabot.Scenario.Visualizer

open Fable.Core
open Fable.SimpleHttp
open Rabot.Market.Visualization

let d3 = D3.d3

let [<Global>] console: JS.Console = jsNative
console.log "Start loading page..."

Async.StartImmediate <| async {
    console.log "Fetching latest canvas..."
    let! (statusCode, responseText) = Http.get "http://localhost:5127/viz/test/latest"

    let result =
        match statusCode with
        | 200 -> Ok responseText
        | _ -> Error (statusCode, responseText)

    match result with
    | Error (statusCode, responseText) ->
        console.log ("Failed to fetch latest chart: " + statusCode.ToString() + ": " + responseText)
    | Ok responseText ->
        let chart = Rabot.Visualization.Viewer.Serialization.deserializeLatest responseText

        console.log chart

        let canvas = chart.FullCanvas
        let priceTimeGrid = canvas.PriceTimeGrid

        let chartDuration =
            priceTimeGrid.LatestTime.datetime - priceTimeGrid.EarliestTime.datetime

        let timePerPixel =
            chartDuration / float canvas.Width.raw

        let timestampToPixel (timestamp: Timestamp) =
            let diffToOrigin = timestamp.datetime - priceTimeGrid.EarliestTime.datetime
            let position = diffToOrigin / timePerPixel
            int position
            
        let ticksPerPixel =
            let chartHeight = priceTimeGrid.HighestPrice.ticks - priceTimeGrid.LowestPrice.ticks
            float chartHeight / float canvas.Height.raw

        let ticksToPixel (price: Price) =
            let diffToTop = priceTimeGrid.HighestPrice.ticks - price.ticks
            let position = float diffToTop / ticksPerPixel
            int position

        console.log ("time per pixel: " + timePerPixel.ToString() + "    ticks per pixel: " + ticksPerPixel.ToString())

        let svg : D3.Selection.Selection<obj,obj,Browser.Types.HTMLElement,obj option> =
            d3.select("#visualization")
                .append("svg")
                .attr("width", canvas.Width.raw)
                .attr("height", canvas.Height.raw)

        for priceTimeObject in priceTimeGrid.Objects do
            match priceTimeObject with
            | Area (width, height, color) ->
                svg.append("rect")
                    .style("fill", color.raw)
                    .attr("width", abs (timestampToPixel width.End - timestampToPixel width.Start))
                    .attr("height", abs (ticksToPixel height.From - ticksToPixel height.To))
                    .attr("x", timestampToPixel (min width.Start width.End))
                    .attr("y", ticksToPixel (max height.From height.To)) |> ignore
            | Circle ((price, timestamp), width, color) ->
                svg.append("circle")
                    .style("fill", color.raw)
                    .attr("r", width.raw / 2)
                    .attr("cx", timestampToPixel timestamp)
                    .attr("cy", ticksToPixel price) |> ignore
            | Line ((priceStart, timestampStart), (priceEnd, timestampEnd), color) ->
                svg.append("line")
                    .style("fill", color.raw)
                    .attr("x1", timestampToPixel timestampStart)
                    .attr("x2", timestampToPixel timestampEnd)
                    .attr("y1", ticksToPixel priceStart)
                    .attr("y2", ticksToPixel priceEnd) |> ignore
}
