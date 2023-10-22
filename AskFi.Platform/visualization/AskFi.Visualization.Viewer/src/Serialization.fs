module Rabot.Visualization.Viewer.Serialization

open Thoth.Json
open Rabot.Market.Visualization

let deserializeLatest (responseJson: string) : GetLatestCanvasResponse =
    let extra =
        Extra.empty
        |> Extra.withInt64
        |> Extra.withUInt64
        |> Extra.withDecimal

    let result = Decode.Auto.fromString<GetLatestCanvasResponse>(responseJson, extra = extra)

    match result with
    | Ok value -> value
    | Error error -> failwith error
