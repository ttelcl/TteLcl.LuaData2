using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// Represents an immutable LUA number.
/// </summary>
/// <remarks>
/// The number can be real or integer. LUA itself doesn't care, but clients may be
/// interested in the distinction, so we use heuristics to distinguish them. All
/// <see cref="LuaNumber"/>s have a <see langword="double"/> value, but some may also
/// have a <see langword="long"/> value, if it has the same value as the double.
/// </remarks>
public sealed class LuaNumber: LuaKeyToken<double>
{
  /// <summary>
  /// Create a new <see cref="LuaNumber"/> from a floating point number
  /// </summary>
  public LuaNumber(double d) : base(LuaType.Number, d)
  {
    IntegerValue = null;
    if(d >= Int64.MinValue && d <= Int64.MaxValue)
    {
      var longValue = (long)d;
      if((double)longValue == d)
      {
        IntegerValue = longValue;
      }
    }
  }

  /// <summary>
  /// Create a new <see cref="LuaNumber"/> from an integer.
  /// </summary>
  public LuaNumber(long l) : base(LuaType.Number, l)
  {
    if((long)Value == l)
    {
      IntegerValue = l;
    }
    else
    {
      // This unusual case happens when a 64 bit integer near maximum or minimum has
      // no exact double representation. For sake of consistency this case is treated as
      // "not representable as integer".
      IntegerValue = null;
    }
  }

  /// <summary><inheritdoc/></summary>
  /// <remarks>Returns a JSON representation of the string.</remarks>
  /// <returns><inheritdoc/></returns>
  public override JToken ToJson(LuaJsonOptions options)
  {
    if(!options.KeepReals && TryGetInteger(out var integer))
    {
      return new JValue(integer);
    }
    else
    {
      return new JValue(Value);
    }
  }

  /// <summary>
  /// The integer value of this <see cref="LuaNumber"/>, if available.
  /// See also <see cref="TryGetInteger(out long)"/> and <see cref="IsInteger"/>
  /// </summary>
  public long? IntegerValue { get; }

  /// <summary>
  /// <see langword="true"/> if this <see cref="LuaNumber"/> is representable as
  /// an integer and <see cref="IntegerValue"/> thus is not <see langword="null"/>.
  /// </summary>
  public bool IsInteger => IntegerValue.HasValue;

  /// <summary>
  /// Test if this <see cref="LuaNumber"/> is representable as an integer and return
  /// <see langword="true"/> if it is and <see langword="false"/> if it isn't.
  /// If <see langword="true"/>, the integer value is returned in <paramref name="integer"/>.
  /// </summary>
  /// <param name="integer"></param>
  /// <returns></returns>
  public bool TryGetInteger(out long integer)
  {
    if(IntegerValue.HasValue)
    {
      integer = IntegerValue.Value;
      return true;
    }
    else
    {
      integer = 0;
      return false;
    }
  }
}
