using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// Represents the LUA value 'nil'. There is only one instance of this type,
/// <see cref="Nil"/>.
/// </summary>
public class LuaNil: LuaToken
{
  /// <summary>
  /// Create a new <see cref="LuaNil"/>. Private, because there can only be one,
  /// and that is <see cref="Nil"/>.
  /// </summary>
  private LuaNil() : base(LuaType.Nil)
  {
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
