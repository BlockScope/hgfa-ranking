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
//val compPlacings : seq<CompPlace>

let scoreCounts = query {
        for p in compPlacings do
        groupBy p.Score into g
        sortByDescending g.Key
        select (g.Key, g.Count())
    }
//val scoreCounts : seq<int * int>

let mapScoreToRank =
    scoreCounts
    |> Seq.fold (fun (acc, map) (score, n) ->
        acc + n, Map.add score acc map) (1, Map.empty)
    |> snd
//val mapScoreToRank : Map<int,int> =
//  map
//    [(0, 63); (200, 62); (249, 61); (350, 60); (462, 59); (568, 58); (619, 57);
//     (653, 56); (701, 55); ...]

let checkedPlacings = query {
        for p in compPlacings do
        let expected = p.Place
        let calculated = mapScoreToRank.[p.Score]
        sortByDescending calculated
        select (expected, {p with Place = calculated})
    }
//val checkedPlacings : seq<int * CompPlace>

let namesByRank =
    checkedPlacings
    |> Seq.map (snd >> (fun p -> (p.Place, p.Name)))
    |> Seq.sortBy fst
    |> Array.ofSeq
//val namesByRank : (int * string) [] =
//  [|(1, "Steven Crosby"); (2, "Don Gardner"); (3, "Rory Duncan");
//    (4, "Dick Heffer"); (5, "Stephen Norman"); (6, "Adrian Connor");
//    (7, "Nils Vesk"); (8, "Drewe Waller"); (9, "Shannon Black");
//    (10, "Frank Chetcuti"); (11, "Paul Bissett-Amess"); (12, "Neale Halsall");
//    (13, "Trevor Scott"); (14, "Vic Hare"); (15, "Jason Kath");
//    (16, "Paddy Honey"); (17, "Hughbert Alexander"); (18, "Bradley Elliott");
//    (19, "Steve Docherty"); (20, "Peter Ebeling"); (21, "Andy Schmidt");
//    (22, "Jon Merli"); (23, "Mike Grimes"); (24, "Curtis Greenwood");
//    (25, "Luke Preston"); (26, "James Morgan"); (27, "Dustan Hansen");
//    (28, "Peter Bolton"); (28, "James Wynd"); (30, "Toby Houldsworth");
//    (31, "Mark Stokoe"); (32, "Phillip Knight"); (33, "Will Faulkner");
//    (34, "Simon Plint"); (35, "Mick Lamb"); (36, "Martin Sielaff");
//    (37, "Mark Divito"); (38, "Harrison Rowntree"); (39, "Fred Crous");
//    (40, "Shelley Heinrich"); (41, "Brad Coleman"); (42, "Birgit Svens");
//    (43, "Col Jackson"); (44, "Jonathan Foote"); (45, "Andrew Phillips");
//    (46, "James McGinty"); (47, "James Atkinson"); (48, "Simon West");
//    (49, "Karl Beattie"); (50, "Peter Leach"); (51, "Fred Smeaton");
//    (52, "Paul Crimmings"); (53, "Phil Clarkson"); (54, "Glenn Bachelor");
//    (55, "Nicola Bowskill"); (56, "Robert Larkin"); (57, "John Duffield");
//    (58, "Michael free"); (59, "Ruben Martin"); (60, "Glenn Selmes");
//    (61, "Scott Barrett"); (62, "Tony Masters"); (63, "John Harriott");
//    (63, "David Longman"); (63, "Wesley Kowalski"); (63, "Stuart McElroy")|]

let asExpected : bool =
    checkedPlacings
    |> Seq.map (fun (x, y) -> x, y.Place)
    |> Seq.filter (fun (a, b) -> a <> b)
    |> Seq.isEmpty
//val asExpected : bool = true

let fractionate (pilotScores : (string * int) seq) : (string * float) seq =
    let maxScore = (pilotScores |> Seq.map snd) |> Seq.max |> float
    pilotScores |> Seq.map (fun (a, b) -> (a, (float b) / maxScore))
//val fractionate : pilotScores:seq<string * int> -> seq<string * float>

compPlacings |> Seq.map (fun x -> x.Name, x.Score) |> fractionate |> Array.ofSeq
//val it : (string * float) [] =
//  [|("Steven Crosby", 1.0); ("Don Gardner", 0.9439728353);
//    ("Rory Duncan", 0.9289352413); ("Dick Heffer", 0.9168081494);
//    ("Stephen Norman", 0.9151103565); ("Adrian Connor", 0.9078341014);
//    ("Nils Vesk", 0.903953432); ("Drewe Waller", 0.8872180451);
//    ("Shannon Black", 0.8789716226); ("Frank Chetcuti", 0.8210041232);
//    ("Paul Bissett-Amess", 0.8190637885); ("Neale Halsall", 0.801843318);
//    ("Trevor Scott", 0.7608537473); ("Vic Hare", 0.756730536);
//    ("Jason Kath", 0.7472714043); ("Paddy Honey", 0.726655348);
//    ("Hughbert Alexander", 0.7208343439); ("Bradley Elliott", 0.6715983507);
//    ("Steve Docherty", 0.6601988843); ("Peter Ebeling", 0.6332767402);
//    ("Andy Schmidt", 0.6301236963); ("Jon Merli", 0.6073247635);
//    ("Mike Grimes", 0.6070822217); ("Curtis Greenwood", 0.5951976716);
//    ("Luke Preston", 0.5835556634); ("James Morgan", 0.5830705797);
//    ("Dustan Hansen", 0.5549357264); ("Peter Bolton", 0.5488721805);
//    ("James Wynd", 0.5488721805); ("Toby Houldsworth", 0.528498666);
//    ("Mark Stokoe", 0.5081251516); ("Phillip Knight", 0.4770797963);
//    ("Will Faulkner", 0.4079553723); ("Simon Plint", 0.40237691);
//    ("Mick Lamb", 0.3907349018); ("Martin Sielaff", 0.3820033956);
//    ("Mark Divito", 0.3609022556); ("Harrison Rowntree", 0.3572641281);
//    ("Fred Crous", 0.3524132913); ("Shelley Heinrich", 0.3519282076);
//    ("Brad Coleman", 0.3485326219); ("Birgit Svens", 0.3427116178);
//    ("Col Jackson", 0.3291292748); ("Jonathan Foote", 0.3203977686);
//    ("Andrew Phillips", 0.3155469318); ("James McGinty", 0.3051176328);
//    ("James Atkinson", 0.2908076643); ("Simon West", 0.2704341499);
//    ("Karl Beattie", 0.2321125394); ("Peter Leach", 0.2308998302);
//    ("Fred Smeaton", 0.213194276); ("Paul Crimmings", 0.1930633034);
//    ("Phil Clarkson", 0.1775406258); ("Glenn Bachelor", 0.1731748727);
//    ("Nicola Bowskill", 0.1700218288); ("Robert Larkin", 0.1583798205);
//    ("John Duffield", 0.150133398); ("Michael free", 0.1377637642);
//    ("Ruben Martin", 0.1120543294); ("Glenn Selmes", 0.08488964346);
//    ("Scott Barrett", 0.06039291778); ("Tony Masters", 0.04850836769);
//    ("John Harriott", 0.0); ("David Longman", 0.0); ("Wesley Kowalski", 0.0);
//    ("Stuart McElroy", 0.0)|]