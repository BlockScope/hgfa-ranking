#r @"..\packages\FSharp.Data.2.0.7\lib\net40\FSharp.Data.dll"

open FSharp.Data

[<Literal>]
let sampleCompIndexFile = @"..\..\..\Test\CompIndex.csv"
type CompIndex = CsvProvider<sampleCompIndexFile>

[<Literal>]
let sampleFile = @"..\..\..\Test\ForbesPreWorlds.csv"
type CompRank = CsvProvider<sampleFile>

let comps = CompIndex.Load sampleCompIndexFile
let forbes2012 = CompRank.Load sampleFile
