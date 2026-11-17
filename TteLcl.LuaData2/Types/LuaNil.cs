using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// Represents the LUA value 'nil'. There is only one instance of this type,
/// <see cref="Nil"/>. Note that this is not a <see cref="LuaKeyToken"/>.
/// </summary>
public sealed class LuaNil: LuaToken
{
  /// <summary>
  /// Create a new <see cref="LuaNil"/>. Private, because there can only be one,
  /// and that is <see cref="Nil"/>.
  /// </summary>
  private LuaNil() : base(LuaType.Nil)
  {
  }

  /// <summary><inheritdoc/></summary>
  /// <remarks>Returns a JSON null.</remarks>
  /// <returns><inheritdoc/></returns>
  public override JToken ToJson(LuaJsonOptions options)
  {
    return JValue.CreateNull();
  }

  /// <summary>
  /// Return the one instance of this type
  /// </summary>
  public static LuaNil Nil { get; } = new LuaNil();

  /// <summary>
  /// Always returns "nil"
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return "nil";
  }
}
