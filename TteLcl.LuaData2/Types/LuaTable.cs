using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// Represents a table in LUA, mapping key <see cref="LuaToken"/>s to a value
/// <see cref="LuaToken"/>.
/// </summary>
/// <remarks>
/// Not all types can be used as key. All <see cref="LuaToken"/> subtypes except 
/// <see cref="LuaNil"/> can be used as value.
/// </remarks>
public class LuaTable: LuaToken
{
  private LuaToken? _falseValue = null;
  private LuaToken? _trueValue = null;
  private readonly Dictionary<string, LuaToken> _stringValues = new Dictionary<string, LuaToken>();
  private readonly Dictionary<double, LuaToken> _realValues = new Dictionary<double, LuaToken>();
  private readonly Dictionary<long, LuaToken> _integerValue = new Dictionary<long, LuaToken>();

  // Implementation note: different key types are stored in different dictionaries.
  // Numeric keyed items may even be stored in two dictionaries: as floating point (always)
  // and as integer (if applicable)

  /// <summary>
  /// Create a new LuaTable
  /// </summary>
  public LuaTable()
    : base(LuaType.Table)
  {
  }

  public LuaToken GetValue(LuaToken key)
  {
    throw new NotImplementedException();
  }

  public void SetValue(LuaToken key, LuaToken value)
  {
    if(value.Type == LuaType.Nil)
    {
      RemoveValue(key);
    }
    else
    {
      switch(key)
      {
        case LuaBoolean lb:
          if(lb.Value)
          {
            _trueValue = value;
          }
          else
          {
            _falseValue = value;
          }
          break;

        // TODO: Handle other types

        default:
          throw new InvalidOperationException(
            $"Type '{key.Type}' is not valid as a table key");
      }
      throw new NotImplementedException();
    }
  }

  private void RemoveValue(LuaToken key)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// Currently always returns the string "table"
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return "table";
  }
}
