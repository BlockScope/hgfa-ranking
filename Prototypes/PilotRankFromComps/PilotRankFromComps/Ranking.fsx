#r @"..\packages\FSharp.Data.2.0.7\lib\net40\FSharp.Data.dll"

open FSharp.Data

[<Literal>]
let sampleFile = @"..\..\..\Test\ForbesPreWorlds.csv"
type CompRank = CsvProvider<sampleFile>

let forbes2012 = CompRank.Load sampleFile
