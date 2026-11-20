using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// Styles for LUA to JSON conversion for LUA tables
/// </summary>
public enum LuaJsonStyle
{
  /// <summary>
  /// Raw form: JSON is an array of key-value records
  /// </summary>
  Raw,

  /// <summary>
  /// Full split form: tables are split by key types into integer indexed ("x"), string keyed ("k"), boolean keyed ("b")
  /// and float indexed ("f") branches of an object. Empty branches are skipped.
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
  Split,

  /// <summary>
  /// Like <see cref="Split"/>, but if there is only one branch only the value of that branch is emitted and
  /// if there are none, null is emitted.
  /// </summary>
  Compact,
}

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
  /// Defines the style of converting LUA tables to JSON.
  /// </summary>
  public LuaJsonStyle Style { get; set; }

  /// <summary>
  /// If <see langword="true"/>, tables for which ALL keys are strings are converted to compact objects instead of
  /// verbose arrays-of-objects. 
  /// </summary>
  public bool AutoObjects { get; set; }
}
