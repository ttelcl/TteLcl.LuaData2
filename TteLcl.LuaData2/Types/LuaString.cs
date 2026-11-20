using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// An immutable <see cref="LuaToken"/> that carries a <see cref="String"/>
/// </summary>
public sealed class LuaString: LuaKeyToken<string>
{
  /// <summary>
  /// Create a new string valued <see cref="LuaToken"/>.
  /// </summary>
  public LuaString(string value) : base(LuaType.String, value)
  {
  }

  /// <summary><inheritdoc/></summary>
  /// <remarks>Returns a JSON representation of the string.</remarks>
  /// <returns><inheritdoc/></returns>
  public override JToken ToJson(LuaJsonOptions options)
  {
    return JValue.CreateString(Value);
  }

  /// <inheritdoc/>
  public override string ToString()
  {
    return Value;
  }
}
