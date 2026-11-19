module Usage

open ColorPrint

/// <summary>
/// Show the usage message. The detail level is controlled by the focus parameter
/// </summary>
/// <param name="focus">
/// If an empty string: show an overview of the app and summaries of its subcommands.
/// If "*": also show details of all subcommands (triggered by passing '-h' on the commandline instead of a subcommand)
/// If a command name: Only show that subcommand's summary and details
/// </param>
let usage focus =
  let showSection section =
    focus = "" || focus = section || focus = "*"
  let showDetail section =
    focus = section || focus = "*"
  if showSection "" then
    cp "\foLua-data to Json converter\f0."
    cp ""
  if showSection "convert" then
    cp "\foluajson2 \f0[\fyconvert\f0] \fg-o \fcfile.json\f0 {\fg-i \fcfile.lua\f0} [\fg-compact\f0|\fg-split\f0|\fg-raw\f0] [\fg-objects\f0]"
    cp "  Convert one or more lua-data files to Json"
  if showDetail "convert" then
    cp "  \fg-o \fcfile.json\f0    The output file name. Optional if there is exactly 1 input."
    cp "  \fg-i \fcfile.lua\f0     An input file defining one or more named globals."
    cp "  \fx\fx\fx                If there are more then they are concatenated into the output pseudo-table"
    cp "  \fg-split\f0\fx          Split tables by key type ('x' for integer indexed, 'k' for string keyed)"
    cp "  \fx\fx\fx                In untypical cases you may also find 'b' (boolean-keyed) and 'f' (float-indexed)"
    cp "  \fg-compact\f0\fx        Like \fg-split\f0, but if there is only one type, it is unwrapped. And if there is"
    cp "  \fx\fx\fx                none, null is returned."
    cp "  \fg-raw\f0\fx            Emit tables in raw form, an array of key-value objects"
    cp "  \fg-objects\f0\fx        Tables that only have only string keys are converted to objects"
    cp "  \fx\fx\fx                (bypassing \fg-raw\f0/\fg-split\f0/\fg-compact\f0)"
    cp ""
  if showSection "" then
    cp "\fyCommon options\f0"
    cp "  \fg-v\f0   Be more verbose. Also affects details of crash messages."
    cp "  \fg-h\f0   Show help message."



