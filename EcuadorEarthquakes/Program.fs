open System
open System.IO
open FSharp.Data

type Sismos = HtmlProvider<"http://eventos.igepn.edu.ec/eqevents/events2.html">

let headersWriter (tw:TextWriter) (headers:string[] option) =
    match headers with
    | Some(hs) -> tw.Write(String.Join(",", hs))
    | _ -> tw.Write ""

// Longitude and latitude come as "23° N", "1.15° S", "75° W", "34.12° E"
// These must become "23", "-1.15", "-75", "34.12" 
let coordToFloat posSelector (coord : string) =
    let absValue = coord.Substring(0, coord.Length - 3)
    if coord.EndsWith posSelector then absValue else "-" + absValue

let longToFloat = coordToFloat "E" 
let latToFloat = coordToFloat "N" 

let sismWriter (tw:TextWriter) (sism:Sismos.Table1.Row) =
    tw.Write("{0:s},{1:s},{2},{3},{4},{5},{6},{7},{8},{9:s}", 
            sism.``Origin Time UTC``, 
            sism.``Local Time``, 
            sism.Mag, 
            sism.Type, 
            latToFloat sism.Latitude,
            longToFloat sism.Longitude, 
            sism.``Depth (km)``,
            sism.``Region Name``,
            sism.Status,
            sism.``Last Update``)

[<EntryPoint>]
let main argv = 
    Sismos.GetSample().Tables.Table1.Headers |> printfn "%a" headersWriter
    let pageIndexes = "" :: ([1 .. 49] |> List.map Convert.ToString) // the pages are "", "1", "2", ..., "49" 
    for i in pageIndexes do
        let sismos = Sismos.Load("http://eventos.igepn.edu.ec/eqevents/events" + i + ".html")
        for row in sismos.Tables.Table1.Rows do
            row |> printfn "%a" sismWriter
    0
