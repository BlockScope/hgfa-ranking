#I @"../packages/FSharp.Data.2.0.5/lib/net40"
#I @"../packages/Deedle.0.9.12/lib/net40"

#r "Deedle.dll"

open System
open System.IO
open System.Linq
open Deedle

let root = __SOURCE_DIRECTORY__ + "/../../../Test/"
let compFile = root + "ForbesPreWorlds.csv"

File.Exists(compFile);;
let compFrame = Frame.ReadCsv(compFile)
//SEE: https://github.com/BlueMountainCapital/Deedle/issues/163
//System.TypeLoadException: Could not load type 'FSharp.Data.Csv.CsvFile' from assembly 'FSharp.Data.DesignTime, Version=2.0.5.0, Culture=neutral, PublicKeyToken=null'.
//   at Deedle.FrameUtilsModule.readCsv(TextReader reader, FSharpOption`1 hasHeaders, FSharpOption`1 inferTypes, FSharpOption`1 inferRows, FSharpOption`1 schema, String missingValues, FSharpOption`1 separators, FSharpOption`1 culture, FSharpOption`1 maxRows)
//   at Deedle.FSharpFrameExtensions.Frame.ReadCsv.Static(String path, FSharpOption`1 hasHeaders, FSharpOption`1 inferTypes, FSharpOption`1 inferRows, FSharpOption`1 schema, FSharpOption`1 separators, FSharpOption`1 culture, FSharpOption`1 maxRows) in c:\dev\FSharp.DataFrame\src\Deedle\FrameExtensions.fs:line 268
//   at <StartupCode$FSI_0005>.$FSI_0005.main@() in C:\dev\Src\hgfa-ranking\Prototypes\PilotRankFromComps\PilotRankFromComps\Deedle.fsx:line 15