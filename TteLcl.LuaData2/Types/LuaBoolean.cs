using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// A <see cref="LuaToken"/> representing a boolean. The only valid values are
/// <see cref="False"/> and <see cref="True"/>
/// </summary>
public class LuaBoolean: LuaToken
{

  /// <summary>
  /// Create a new LuaBoolean
  /// </summary>
  private LuaBoolean(bool value) : base(LuaType.Boolean)
  {
    Value = value;
  }

  /// <summary>
  /// The boolean value
  /// </summary>
  public bool Value { get; }

  /// <summary>
  /// The one instance of <see cref="LuaBoolean"/> that has the value <see langword="false"/>.
  /// </summary>
  public static LuaBoolean False { get; } = new LuaBoolean(false);

  /// <summary>
  /// The one instance of <see cref="LuaBoolean"/> that has the value <see langword="true"/>.
  /// </summary>
  public static LuaBoolean True { get; } = new LuaBoolean(true);

  /// <summary>
  /// Returns either "true" or "false", depending on <see cref="Value"/>.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return Value ? "true" : "false";
  }
}
