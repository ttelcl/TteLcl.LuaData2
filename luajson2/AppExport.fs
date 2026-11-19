module AppExport

open System
open System.IO

open Newtonsoft.Json

open TteLcl.LuaData2.Parser
open TteLcl.LuaData2.Types

open ColorPrint
open CommonTools

type private Options = {
  Style: LuaJsonStyle
  Source: string
  Destination: string
  Overwrite: bool
}

type private Context = {
  ConversionOptions: LuaJsonOptions
  AppOptions: Options
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
    | "-from" :: source :: rest ->
      rest |> parseMore {o with Source = source}
    | "-to" :: destination :: rest ->
      rest |> parseMore {o with Destination = destination}
    | "-split" :: rest ->
      rest |> parseMore {o with Style = LuaJsonStyle.Split}
    | "-compact" :: rest ->
      rest |> parseMore {o with Style = LuaJsonStyle.Compact}
    | "-raw" :: rest ->
      rest |> parseMore {o with Style = LuaJsonStyle.Raw}
    | "-f" :: rest ->
      rest |> parseMore {o with Overwrite = true}
    | [] ->
      let o =
        if o.Destination |> String.IsNullOrEmpty then
          {o with Destination = o.Source}
        else
          o
      if o.Source |> String.IsNullOrEmpty then
        cp "\foNo source directory (\fg-from\fo) specified\f0."
        None
      elif o.Source |> Directory.Exists |> not then
        cp $"\foSource directory (\fy{o.Source}\fo) does not exist\f0."
        None
      else
        o |> Some
    | x :: _ ->
      cp $"\foUnrecognized argument \f0'\fy{x}\f0'"
      None
  args |> parseMore {
    Style = LuaJsonStyle.Compact
    Source = null
    Destination = null
    Overwrite = false
  }

let private exportFile c path (source:FileInfo) destinationPath =
  let sourceFile = source.FullName
  let destinationFile = Path.Combine(destinationPath, source.Name + ".json")
  let destination = new FileInfo(destinationFile)
  let needWrite =
    c.AppOptions.Overwrite ||
    (destination.Exists |> not) ||
    destination.LastWriteTimeUtc < source.LastWriteTimeUtc
  if needWrite then
    cp $"Converting \fy{path}\fo{Path.DirectorySeparatorChar}\fg{destination.Name}\f0."
    let builder = new LuaTableBuilder();
    builder.Load(sourceFile)
    let globalTable = builder.Bake()
    let jtoken = globalTable.ToJson(c.ConversionOptions)
    let json = JsonConvert.SerializeObject(jtoken, Formatting.Indented)
    do
      use w = destinationFile |> startFile
      json |> w.WriteLine
    destinationFile |> finishFile
    true
  else
    false

let rec private exportFolder c path =
  let mutable convertedCount = 0
  let mutable skippedCount = 0
  let o = c.AppOptions
  let source =
    if path |> String.IsNullOrEmpty || path = "." then
      o.Source
    else
      Path.Combine(o.Source, path)
  let destination =
    if path |> String.IsNullOrEmpty || path = "." then
      o.Destination
    else
      Path.Combine(o.Destination, path)
  let sourceInfo = new DirectoryInfo(source)
  let fileInfos = sourceInfo.GetFiles("*.lua")
  if fileInfos.Length > 0 && Directory.Exists(destination) |> not then
    Directory.CreateDirectory(destination) |> ignore
  for fi in fileInfos do
    let converted = exportFile c path fi destination
    if converted then
      convertedCount <- convertedCount + 1
    else
      skippedCount <- skippedCount + 1
  let childInfos = sourceInfo.GetDirectories()
  for ci in childInfos do
    let childPath = Path.Combine(path, ci.Name)
    let nCvt, nSkip = exportFolder c childPath
    convertedCount <- convertedCount + nCvt
    skippedCount <- skippedCount + nSkip
  convertedCount, skippedCount

let private runExport o =
  let jOptions = new LuaJsonOptions()
  jOptions.KeepReals <- false
  jOptions.AutoObjects <- false
  jOptions.Style <- o.Style
  let context = {
    AppOptions = o
    ConversionOptions = jOptions
  }
  let converted, skipped = exportFolder context ""
  cp $"Converted \fb{converted}\f0 files and skipped \fo{skipped}\f0."
  0

let run args =
  let oo = args |> parseArgs
  match oo with
  | None ->
    cp ""
    Usage.usage "export"
    1
  | Some o ->
    o |> runExport
