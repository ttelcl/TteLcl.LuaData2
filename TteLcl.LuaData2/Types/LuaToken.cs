using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// Base class for all LUA values
/// </summary>
public abstract class LuaToken
{
  /// <summary>
  /// Create a new LuaToken
  /// </summary>
  protected LuaToken(LuaType type)
  {
    Type = type;
  }

  /// <summary>
  /// The type of this value
  /// </summary>
  public LuaType Type { get; }

  /// <summary>
  /// "Create" a nil <see cref="LuaToken"/> (always returning <see cref="LuaNil.Nil"/>).
  /// </summary>
  /// <returns></returns>
  public static LuaNil Create() => LuaNil.Nil;

  /// <summary>
  /// "Create" a <see cref="LuaBoolean"/> from the given boolean argument <paramref name="b"/>.
  /// </summary>
  /// <param name="b"></param>
  /// <returns></returns>
  public static LuaBoolean Create(bool b) => b ? LuaBoolean.True : LuaBoolean.False;

  /// <summary>
  /// Create a new <see cref="LuaString"/> from the given string <paramref name="s"/>.
  /// </summary>
  /// <param name="s"></param>
  /// <returns></returns>
  public static LuaString Create(string s) => new LuaString(s);

  /// <summary>
  /// Create a new <see cref="LuaNumber"/> from the given floating point number <paramref name="d"/>.
  /// </summary>
  /// <param name="d"></param>
  /// <returns></returns>
  public static LuaNumber Create(double d) => new LuaNumber(d);

  /// <summary>
  /// Create a new <see cref="LuaNumber"/> from the given integer number <paramref name="l"/>.
  /// </summary>
  /// <param name="l"></param>
  /// <returns></returns>
  public static LuaNumber Create(long l) => new LuaNumber(l);

  /// <summary>
  /// Create a new empty <see cref="LuaTable"/>.
  /// </summary>
  /// <returns></returns>
  public static LuaTable CreateTable() => new LuaTable();
}
