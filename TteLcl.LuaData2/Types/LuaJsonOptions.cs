/*
 * (c) 2026  ttelcl / ttelcl
 */

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
  /// If true, tables for which ALL keys are strings are converted to compact objects instead of
  /// verbose arrays-of-objects
  /// </summary>
  public bool AutoObjects { get; set; }

}
