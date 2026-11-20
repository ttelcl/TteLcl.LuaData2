using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// A <see cref="LuaToken"/> representing a boolean. The only valid values are
/// <see cref="False"/> and <see cref="True"/>
/// </summary>
public sealed class LuaBoolean: LuaKeyToken<bool>
{

  /// <summary>
  /// Create a new LuaBoolean
  /// </summary>
  private LuaBoolean(bool value) : base(LuaType.Boolean, value)
  {
  }

  /// <summary><inheritdoc/></summary>
  /// <remarks>Returns a JSON true or false.</remarks>
  /// <returns><inheritdoc/></returns>
  public override JToken ToJson(LuaJsonOptions options)
  {
    return new JValue(Value);
  }

  /// <summary>
  /// The one instance of <see cref="LuaBoolean"/> that has the value <see langword="false"/>.
  /// </summary>
  public static LuaBoolean False { get; } = new LuaBoolean(false);

  /// <summary>
  /// The one instance of <see cref="LuaBoolean"/> that has the value <see langword="true"/>.
  /// </summary>
  public static LuaBoolean True { get; } = new LuaBoolean(true);

  /// <summary>
  /// Returns either "true" or "false", depending on <see cref="LuaKeyToken{T}.Value"/>.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return Value ? "true" : "false";
  }
}
