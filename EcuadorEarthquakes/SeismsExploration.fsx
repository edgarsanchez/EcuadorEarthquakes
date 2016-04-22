#r "../packages/FSharp.Data.2.2.5/lib/net40/FSharp.Data.dll"

open FSharp.Data
open System

type Seisms = HtmlProvider<"http://eventos.igepn.edu.ec/eqevents/events2.html">

// Longitude and latitude come as "23° N", "1.15° S", "75° W", "34.12° E"
// These must become "23", "-1.15", "-75", "34.12" 
let coordToFloat posSelector (coord : string) =
    let absValue = coord.Substring(0, coord.Length - 3)
    if coord.EndsWith posSelector then absValue else "-" + absValue

let longToFloat = coordToFloat "E" 
let latToFloat = coordToFloat "N" 

let seismToString (seism: Seisms.Table1.Row) =
    String.Format("{0:s},{1:s},{2},{3},{4},{5},{6},{7},{8},{9:s}", 
            seism.``Origin Time UTC``, 
            seism.``Local Time``, 
            seism.Mag, 
            seism.Type, 
            longToFloat seism.Latitude, 
            latToFloat seism.Longitude,
            seism.``Depth (km)``,
            seism.``Region Name``,
            seism.Status,
            seism.``Last Update``)

let headers = String.Join (",", Seisms.GetSample().Tables.Table1.Headers.Value) 


let pageIndexes = "" :: ([1 .. 49] |> List.map Convert.ToString) // the page indexes are "", "1", "2", ..., "49"
let seisms = pageIndexes |> List.collect 
                (fun pageIndex ->
                    let page = Seisms.Load("http://eventos.igepn.edu.ec/eqevents/events" + pageIndex + ".html")
                    page.Tables.Table1.Rows |> Array.map seismToString |> Array.toList)

