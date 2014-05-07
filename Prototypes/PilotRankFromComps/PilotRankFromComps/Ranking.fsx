#r @"..\packages\FSharp.Data.2.0.7\lib\net40\FSharp.Data.dll"

open System
open System.Linq
open FSharp.Data

[<Literal>]
let sampleCompIndexFile = @"..\..\..\Test\CompIndex.csv"
type CompIndex = CsvProvider<sampleCompIndexFile>

[<Literal>]
let sampleFile = @"..\..\..\Test\ForbesPreWorlds.csv"
type CompRank = CsvProvider<sampleFile>
//  {Headers = Some
//                 [|"Place"; "Name"; "ladder score";
//                   "ladder score pre-devaluation"; ""; "Gender"; "Nat";
//                   "Glider"; "Class"; "Total"|];
//     NumberOfColumns = 10;
//     Quote = '"';
//     Rows = seq
//              [("1", "Rohan Holtkamp", "230", "360", "Pre-Worlds 2012", "M",
//                "Australia", "Airborne Rev 13.5", "", "6628");
//               ("2", "Attila Bertok", "", "", "Pre-Worlds 2012", "M",
//                "Hungary", "Moyes Litespeed S 5", "", "6516");
  
let comps = CompIndex.Load sampleCompIndexFile
let comp = CompRank.Load sampleFile

let maxScore = comp.Rows.Max(fun x -> x.Total)

let maxScore' = query {
        for r in comp.Rows do
        maxBy r.Total
    }

let groupByScore = query {
        for r in comp.Rows do
        groupBy r.Total
    }

