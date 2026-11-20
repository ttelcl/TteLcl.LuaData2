open System

open ColorPrint
open CommonTools

let rec run arglist =
  match arglist with
  | "-v" :: rest ->
    verbose <- true
    rest |> run
  | "--help" :: _
  | "-h" :: _ ->
    Usage.usage "*"
    0
  | [] ->
    Usage.usage ""
    0  // program return status code to the operating system; 0 == "OK"
  | "export" :: rest ->
    rest |> AppExport.run
  | "convert" :: rest ->
    rest |> AppConvert.run
  | x :: _ when x.StartsWith('-') ->
    // unusually, we support a default command in this application
    arglist |> AppConvert.run
  | x :: _ ->
    cp $"\frUnknown command:\f0 '\fy{x}\f0'"
    Usage.usage ""
    1

[<EntryPoint>]
let main args =
  try
    args |> Array.toList |> run
  with
  | ex ->
    ex |> ExceptionTool.fancyExceptionPrint verbose
    resetColor ()
    1



