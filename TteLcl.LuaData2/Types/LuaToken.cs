using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

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
  /// Convert this <see cref="LuaToken"/> to a corresponding <see cref="JToken"/>.
  /// </summary>
  /// <returns></returns>
  public abstract JToken ToJson(LuaJsonOptions options);

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

/// <summary>
/// Base class for all <see cref="LuaToken"/>s that can be used as values in a
/// <see cref="LuaTable"/>.
/// </summary>
/// <remarks>
/// That is: all concrete <see cref="LuaToken"/> subclasses except <see cref="LuaNil"/>.
/// Only <see cref="LuaTable"/> derives directly from this, the rest indirectly via
/// <see cref="LuaKeyToken"/> and then <see cref="LuaKeyToken{T}"/>.
/// </remarks>
public abstract class LuaValueToken: LuaToken
{
  /// <summary>
  /// Create a <see cref="LuaValueToken"/>
  /// </summary>
  /// <param name="type"></param>
  protected LuaValueToken(LuaType type) : base(type)
  {
  }
}

/// <summary>
/// Base class for <see cref="LuaToken"/>s that can be used as key in a <see cref="LuaTable"/>.
/// </summary>
public abstract class LuaKeyToken: LuaValueToken
{
  /// <summary>
  /// Create a <see cref="LuaKeyToken"/>
  /// </summary>
  /// <param name="type"></param>
  protected LuaKeyToken(LuaType type) : base(type)
  {
  }

  /// <summary>
  /// <inheritdoc/>
  /// </summary>
  /// <remarks>
  /// Marked as <see langword="abstract"/> to force subclasses to override <see cref="Object.GetHashCode"/>.
  /// </remarks>
  /// <returns><inheritdoc/></returns>
  public abstract override int GetHashCode();

  /// <summary>
  /// <inheritdoc/>
  /// </summary>
  /// <remarks>
  /// Marked as <see langword="abstract"/> to force subclasses to override <see cref="Object.Equals(object?)"/>.
  /// </remarks>
  /// <returns><inheritdoc/></returns>
  public abstract override bool Equals(object? obj);
}

/// <summary>
/// Base class for <see cref="LuaToken"/>s that can be used as table key and
/// have a simple typed "Value" property of type <typeparamref name="T"/>.
/// </summary>
public abstract class LuaKeyToken<T>: LuaKeyToken, IEquatable<LuaKeyToken<T>>
  where T : IEquatable<T>
{
  /// <summary>
  /// Create a <see cref="LuaKeyToken{T}"/>
  /// </summary>
  /// <param name="type"></param>
  /// <param name="value"></param>
  protected LuaKeyToken(LuaType type, T value) : base(type)
  {
    Value = value;
  }

  /// <summary>
  /// The value of this <see cref="LuaToken"/>
  /// </summary>
  public T Value { get; }

  /// <inheritdoc/>
  public sealed override int GetHashCode()
  {
    return HashCode.Combine(Value);
  }

  /// <inheritdoc/>
  public sealed override bool Equals(object? obj)
  {
    return Equals(obj as LuaKeyToken<T>);
  }

  /// <inheritdoc/>
  public bool Equals(LuaKeyToken<T>? other)
  {
    return other != null ? Value.Equals(other.Value) : false;
  }
}
