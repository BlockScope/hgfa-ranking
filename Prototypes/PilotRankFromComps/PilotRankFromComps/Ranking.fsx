#I @"..\packages\FSharp.Data.2.0.7\lib\net40"
//#I @"..\packages\Deedle.0.9.12"

#r @"FSharp.Data.dll"
//#load "Deedle.fsx"

open System
open System.IO
open System.Linq
open FSharp.Data

[<Literal>]
let sampleCompIndexFile = @"..\..\..\Test\CompIndex.csv"
type CompIndex = CsvProvider<sampleCompIndexFile>

[<Literal>]
let sampleCompFile = @"..\..\..\Test\ForbesPreWorlds.csv"
type CompRank = CsvProvider<sampleCompFile>
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

//open Deedle
//let frameFile = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, sampleCompFile))
//let compFrame = Frame.ReadCsv(frameFile, hasHeaders = true, inferTypes = true, inferRows = 0, schema = null, separators = ",", culture = null, maxRows = 0)

type SanctionRanking = AAA | AA | A | B | C

let sanctionPoints = function
    | AAA -> 450
    | AA -> 360
    | A -> 288
    | B -> 230
    | C -> 184

sanctionPoints AA
