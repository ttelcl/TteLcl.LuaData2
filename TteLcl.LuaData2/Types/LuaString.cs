using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// An immutable <see cref="LuaToken"/> that carries a <see cref="String"/>
/// </summary>
public class LuaString: LuaToken
{
  /// <summary>
  /// Create a new LuaString
  /// </summary>
  public LuaString(string value) : base(LuaType.String)
  {
    Value = value;
  }

  /// <summary>
  /// The string value
  /// </summary>
  public string Value { get; }

}
