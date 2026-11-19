using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// Describes options for converting <see cref="LuaToken"/>s to <see cref="JToken"/>.
/// Unusual for my coding style, this class is mutable.
/// </summary>
public class LuaJsonOptions
{
  /// <summary>
  /// Create a new <see cref="LuaJsonOptions"/> with default values.
  /// </summary>
  public LuaJsonOptions()
  {
  }

  /// <summary>
  /// When <see langword="true"/>, all <see cref="LuaNumber"/>s are always converted to <see cref="Double"/>.
  /// If <see langword="false"/>, <see cref="LuaNumber"/>s that can be represented as <see cref="Int64"/> are
  /// converted to that, the ones not representable as such are still converted to <see cref="Double"/>.
  /// </summary>
  public bool KeepReals { get; set; }

  /// <summary>
  /// If <see langword="true"/>, tables for which ALL keys are strings are converted to compact objects instead of
  /// verbose arrays-of-objects. 
  /// </summary>
  public bool AutoObjects { get; set; }

  /// <summary>
  /// If <see langword="true"/>, the JSON representation of LUA tables splits them by type.
  /// If <see langword="false"/>, the raw model is kept.
  /// </summary>
  /// <remarks>
  /// The split model is an object (instead of the array in the raw model) which may contain the following 4 fixed fields
  /// (empty ones are omitted).
  /// <list type="bullet">
  /// <item><term>"x"</term> table items that are indexed by an integer, provided as an array of index-value pairs. Common</item>
  /// <item><term>"k"</term> table items that are indexed by a string, as an object. Common</item>
  /// <item><term>"b"</term> table items that are indexed by a boolean, as an object. Very uncommon.</item>
  /// <item><term>"f"</term> table items that are indexed by a non-integer number. Very uncommon. A design flaw, really...</item>
  /// </list>
  /// </remarks>
  public bool Split { get; set; }
}
