module AppConvert

open System
open System.IO

open Newtonsoft.Json

open TteLcl.LuaData2.Parser
open TteLcl.LuaData2.Types

open ColorPrint
open CommonTools

type private Options = {
  OutputName: string
  InputNames: string list
  AutoObjects: bool
  Style: LuaJsonStyle option
}

let private parseArgs args =
  let rec parseMore o args =
    match args with
    | "-v" :: rest ->
      verbose <- true
      rest |> parseMore o
    | "--help" :: _ 
    | "-h" :: _ ->
      None
    | "-o" :: outfile :: rest ->
      rest |> parseMore {o with OutputName = outfile}
    | "-i" :: infile :: rest ->
      rest |> parseMore {o with InputNames = infile :: o.InputNames}
    | "-split" :: rest ->
      rest |> parseMore {o with Style = LuaJsonStyle.Split |> Some}
    | "-compact" :: rest ->
      rest |> parseMore {o with Style = LuaJsonStyle.Compact |> Some}
    | "-raw" :: rest ->
      rest |> parseMore {o with Style = LuaJsonStyle.Raw |> Some}
    | "-no-objects" :: rest ->  // deprecated
      rest |> parseMore {o with AutoObjects = false}
    | "-objects" :: rest ->  // deprecated
      rest |> parseMore {o with AutoObjects = true}
    | [] ->
      let o =
        if o.OutputName |> String.IsNullOrEmpty && o.InputNames.Length = 1 then
          let bareName = o.InputNames.Head |> Path.GetFileName
          let outName = Path.ChangeExtension(bareName, ".json")
          {o with OutputName = outName}
        else
          o
      if o.InputNames |> List.isEmpty then
        cp "\foNo input files (\fg-i\fo) specified\f0."
        None
      elif o.Style.IsNone then
        cp "\foExpecting one of \fg-compact\f0, \fg-split\f0, \fg-raw\f0\f0."
        None
      elif o.OutputName |> String.IsNullOrEmpty then
        cp "\foNo output file (\fg-o\fo) specified\f0."
        None
      else
        {o with InputNames = o.InputNames |> List.rev} |> Some
    | x :: _ ->
      cp $"\foUnrecognized argument \f0'\fy{x}\f0'"
      None
  args |> parseMore {
    OutputName = null
    InputNames = []
    AutoObjects = false
    Style = None
  }

let private runConvert o =
  let builder = new LuaTableBuilder();
  for inputFile in o.InputNames do
    cp $"Loading '\fg{inputFile}\f0'..."
    builder.Load(inputFile)
  let globalTable = builder.Bake()
  let jOptions = new LuaJsonOptions()
  jOptions.KeepReals <- false
  jOptions.AutoObjects <- o.AutoObjects
  jOptions.Style <- o.Style.Value
  let jtoken = globalTable.ToJson(jOptions)
  let json = JsonConvert.SerializeObject(jtoken, Formatting.Indented)
  do
    use w = o.OutputName |> startFile
    json |> w.WriteLine
  o.OutputName |> finishFile
  0

let run args =
  let oo = args |> parseArgs
  match oo with
  | None ->
    cp ""
    Usage.usage "convert"
    1
  | Some o ->
    o |> runConvert
