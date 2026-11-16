using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// An enumeration representing the various supported Lua Types.
/// </summary>
/// <remarks>
/// Keep in mind that the following LUA types are not supported: userdata, thread, function
/// </remarks>
public enum LuaType
{
  // NOTE! The relative order determines the sort order of RawTable.Sort!

  /// <summary>
  /// A number (can be floating point or integer)
  /// </summary>
  Number,

  /// <summary>
  /// The nil type
  /// </summary>
  Nil,

  /// <summary>
  /// A boolean
  /// </summary>
  Boolean,

  /// <summary>
  /// A string
  /// </summary>
  String,

  /// <summary>
  /// A table
  /// </summary>
  Table,
}

