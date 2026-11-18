using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TteLcl.LuaData2.Parser;

/// <summary>
/// Codes for the different types of items on the parser 'stack' (a linked list)
/// </summary>
public enum ParserType
{
  /// <summary>
  /// A stack slot that is not in use
  /// </summary>
  Unused,

  /// <summary>
  /// A stack slot representing a value
  /// </summary>
  Value,

  /// <summary>
  /// A stack slot representing a name for an incomplete table entry
  /// </summary>
  Name,

  /// <summary>
  /// A temporary item to build table values.
  /// </summary>
  TableBuilder,

  /// <summary>
  /// A temporary object for collecting name data
  /// </summary>
  NameBuilder
}
