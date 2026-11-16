using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
public class LuaNumber: LuaToken
{
  /// <summary>
  /// Create a new <see cref="LuaNumber"/> from a floating point number
  /// </summary>
  public LuaNumber(double d) : base(LuaType.Number)
  {
    Value = d;
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
  public LuaNumber(long l) : base(LuaType.Number)
  {
    Value = l;
    IntegerValue = l;
  }

  /// <summary>
  /// The floating point value of this <see cref="LuaNumber"/>. Always available.
  /// </summary>
  public double Value { get; }

  /// <summary>
  /// The integer value of this <see cref="LuaNumber"/>, if available.
  /// See also <see cref="TryGetInteger(out long)"/>
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
