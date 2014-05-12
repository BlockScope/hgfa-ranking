#I @"..\packages\FSharp.Data.2.0.7\lib\net40"

#r @"FSharp.Data.dll"

open System
open System.IO
open System.Linq
open FSharp.Data

[<Literal>]
let sampleCompIndexFile = @"..\..\..\Test\CompIndex.csv"
type CompIndex = CsvProvider<sampleCompIndexFile>

[<Literal>]
let compSchemaFile = @"..\..\..\Test\CompSchema.csv"
let workingFile = @"..\..\..\Test\CorryongOpen2014.csv"
//let workingFile = @"..\..\..\Test\ForbesPreWorlds.csv"

[<Literal>]
let schema : string = "Place=int,Name=string,Gender=string,Nat=string,Glider=string,Class=string,Total=int"

type CompRank = CsvProvider<compSchemaFile, HasHeaders = true, Schema = schema>
  
let comps = CompIndex.Load sampleCompIndexFile
let comp = CompRank.Load workingFile

type SanctionRanking = AAA | AA | A | B | C

let sanctionPoints = function
    | AAA -> 450
    | AA -> 360
    | A -> 288
    | B -> 230
    | C -> 184

sanctionPoints AA

let diminishByFraction startWith (fraction : float) (n : int) =
    Seq.initInfinite (fun _ -> fraction)
    |> Seq.take n
    |> Seq.scan (fun acc x -> acc * x) startWith

diminishByFraction 450.0 0.8 4 |> List.ofSeq
//val it : float list = [450.0; 360.0; 288.0; 230.4; 184.32]

diminishByFraction 450.0 0.8 4 |> Seq.map int |> List.ofSeq
//val it : int list = [450; 360; 288; 230; 184]

type Nat = AUS | NZL | Other of string
    with static member Parse = function | "AUS" -> AUS | "NZL" -> NZL | x -> Other x
type Gender = M | F
    with static member Parse = function | "m" | "M" -> M | _ -> F

type Class = Class1 | Class1K | Other of string
    with static member Parse = function | "1" -> Class1 | "1K" -> Class1K | x -> Other x

type CompPlace =
    {
        Place : int
        Name : string
        Gender : Gender
        Nat : Nat
        Class : Class
        Score : int
    }

let compPlacings = query {
        for r in comp.Rows do
        select
            {
                Place = r.Place
                Name = r.Name
                Gender = Gender.Parse r.Gender
                Nat = Nat.Parse r.Nat
                Class = Class.Parse r.Class
                Score = r.Total
            }
    }

compPlacings |> Array.ofSeq

let groupedPlacings = query {
        for p in compPlacings do
        groupBy p.Score into g
        sortByDescending g.Key
    }

groupedPlacings |> Seq.mapi (fun i g -> (i + 1, g)) |> Array.ofSeq

type PlaceState = (int * int * CompPlace list)
let makePlace (xs : PlaceState) (ys : int * CompPlace list) : PlaceState =
    let xTally, xPlace, _ = xs
    let yTally, ys' = ys
    (xTally + ys'.Length, xPlace + 1, ys')

let ranked =
    groupedPlacings
    |> Seq.map (fun g -> (g.Key, g.Cast<CompPlace>() |> List.ofSeq))
    |> Seq.scan makePlace (0, 0, [])

ranked |> Array.ofSeq

let checkPlacings = query {
        for p in compPlacings do
        sortByDescending p.Score
    }

let expectedVersusPlaced = checkPlacings |> Seq.mapi (fun i x -> i + 1, x.Place) |> Array.ofSeq
expectedVersusPlaced |> Seq.filter (fun (a, b) -> a <> b) |> Array.ofSeq

let fractionate (pilotScores : (string * int) seq) : (string * float) seq =
    let maxScore = (pilotScores |> Seq.map snd) |> Seq.max |> float
    pilotScores |> Seq.map (fun (a, b) -> (a, (float b) / maxScore))

compPlacings |> Seq.map (fun x -> x.Name, x.Score) |> fractionate |> Array.ofSeq