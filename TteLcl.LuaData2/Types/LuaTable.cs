using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace TteLcl.LuaData2.Types;

/// <summary>
/// Represents a table in LUA, mapping key <see cref="LuaToken"/>s to a value
/// <see cref="LuaToken"/>.
/// </summary>
/// <remarks>
/// Not all types can be used as key. All <see cref="LuaToken"/> subtypes except 
/// <see cref="LuaNil"/> can be used as value.
/// </remarks>
public sealed class LuaTable: LuaValueToken
{
  private readonly Dictionary<LuaKeyToken, LuaValueToken> _content;
  private readonly Dictionary<long, LuaValueToken> _intMap;

  // Implementation note: all values are stored in _content
  // Additionally, keys that are an exact integer LuaNumber are also stored in _intMap

  /// <summary>
  /// Create a new LuaTable
  /// </summary>
  public LuaTable()
    : base(LuaType.Table)
  {
    _content = new Dictionary<LuaKeyToken, LuaValueToken>();
    _intMap = new Dictionary<long, LuaValueToken>();
  }

  public LuaToken Clone()
  {
    throw new NotImplementedException("Clone() NYI");
  }

  /// <summary><inheritdoc/></summary>
  /// <param name="options"><inheritdoc/></param>
  /// <remarks>
  /// Returns a JSON representation of the table.
  /// ((TO BE DOCUMENTED))
  /// </remarks>
  /// <returns><inheritdoc/></returns>
  public override JToken ToJson(LuaJsonOptions options)
  {
    var objectMode = options.AutoObjects && _content.Keys.All(lkt => lkt is LuaString);
    if(objectMode)
    {
      var jo = new JObject();
      foreach(var (key, value) in _content)
      {
        if(key is not LuaString ls)
        {
          throw new InvalidOperationException(
            $"Internal error - found non-string key '{key}' when there should be none");
        }
        jo[ls.Value] = value.ToJson(options);
      }
      return jo;
    }
    else
    {
      var ja = new JArray();
      foreach(var (key, value) in _content)
      {
        var jo = new JObject();
        var keyToken = key.ToJson(options);
        var valueToken = value.ToJson(options);
        jo["key"] = keyToken.ToString();
        jo["rawkey"] = keyToken;
        if(key is LuaNumber kn && kn.TryGetInteger(out var index))
        {
          jo["index"] = index;
        }
        jo["value"] = valueToken;
        ja.Add(jo);
      }
      return ja;
    }
  }

  /// <summary>
  /// Get, set, or remove the value for <paramref name="key"/> (using LUA semantics,
  /// i.e. a miss returns <see cref="LuaNil.Nil"/>, not <see langword="null"/>).
  /// Setting a key to <see cref="LuaNil.Nil"/> removes it.
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  public LuaToken this[LuaKeyToken key] {
    get => GetValue(key);
    set => SetValue(key, value);
  }

  /// <summary>
  /// Get, set, or remove the value for <paramref name="index"/> (using LUA semantics,
  /// i.e. a miss returns <see cref="LuaNil.Nil"/>, not <see langword="null"/>).
  /// Setting a key to <see cref="LuaNil.Nil"/> removes it.
  /// </summary>
  /// <param name="index"></param>
  /// <returns></returns>
  public LuaToken this[long index] {
    get => GetValue(index);
    set => SetValue(Create(index), value);
  }

  /// Get, set, or remove the value for <paramref name="key"/> (using LUA semantics,
  /// i.e. a miss returns <see cref="LuaNil.Nil"/>, not <see langword="null"/>).
  /// Setting a key to <see cref="LuaNil.Nil"/> removes it.
  public LuaToken this[string key] {
    get => GetValue(Create(key));
    set => SetValue(Create(key), value);
  }

  /// Get, set, or remove the value for <paramref name="key"/> (using LUA semantics,
  /// i.e. a miss returns <see cref="LuaNil.Nil"/>, not <see langword="null"/>).
  /// Setting a key to <see cref="LuaNil.Nil"/> removes it.
  public LuaToken this[bool key] {
    get => GetValue(key ? LuaBoolean.True : LuaBoolean.False);
    set => SetValue(key ? LuaBoolean.True : LuaBoolean.False, value);
  }

  /// <summary>
  /// A read only view on the mapping of keys to values in this <see cref="LuaTable"/>.
  /// No guarantees are made on the sort order (but if you only add entries, never overwrite or
  /// delete, it probably is insertion order)
  /// </summary>
  public IReadOnlyDictionary<LuaKeyToken, LuaValueToken> ValueMap => _content;

  /// <summary>
  /// A read only view on the mapping of keys to values in this <see cref="LuaTable"/> for keys
  /// that are an exact integer.
  /// No guarantees are made on the sort order (but if you only add integer-keyed entries, never
  /// overwrite or delete, it probably is insertion order, which is quite possibly index order).
  /// </summary>
  public IReadOnlyDictionary<long, LuaValueToken> ValueIndex => _intMap;

  /// <summary>
  /// Get the value according to LUA logic: the value if found,
  /// or <see cref="LuaNil.Nil"/> otherwise.
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  public LuaToken GetValue(LuaKeyToken key)
  {
    return _content.TryGetValue(key, out var value) ? value : LuaNil.Nil;
  }

  /// <summary>
  /// Get the value by integer index according to LUA logic: the value if found,
  /// or <see cref="LuaNil.Nil"/> otherwise.
  /// Keep in mind that LUA indexes start at 1, not 0.
  /// </summary>
  /// <param name="index"></param>
  /// <returns></returns>
  public LuaToken GetValue(long index)
  {
    return _intMap.TryGetValue(index, out var value) ? value : LuaNil.Nil;
  }

  /// <summary>
  /// Get the value according to dotnet logic: the value if found,
  /// or <see langword="null"/> otherwise.
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  public LuaValueToken? GetRawValue(LuaKeyToken key)
  {
    return _content.TryGetValue(key, out var value) ? value : null;
  }

  /// <summary>
  /// Get the value by index according to dotnet logic: the value if found,
  /// or <see langword="null"/> otherwise.
  /// </summary>
  /// <param name="index"></param>
  /// <returns></returns>
  public LuaValueToken? GetRawValue(long index)
  {
    return _intMap.TryGetValue(index, out var value) ? value : null;
  }

  /// <summary>
  /// Try to get the value for <paramref name="key"/>, returning
  /// <see langword="true"/> if found (and the value in <paramref name="value"/>),
  /// or returning <see langword="false"/> if not found (and returning <see cref="LuaNil.Nil"/>
  /// in <paramref name="value"/>).
  /// </summary>
  /// <param name="key">
  /// The key to find
  /// </param>
  /// <param name="value">
  /// The value if found, or <see cref="LuaNil.Nil"/> otherwise
  /// </param>
  /// <returns></returns>
  public bool TryGetValue(LuaKeyToken key, out LuaToken value)
  {
    if(_content.TryGetValue(key, out var valueToken))
    {
      value = valueToken;
      return true;
    }
    else
    {
      value = LuaNil.Nil;
      return false;
    }
  }

  /// <summary>
  /// Set or remove the value
  /// </summary>
  /// <param name="key">
  /// The key for the value to set or remove
  /// </param>
  /// <param name="valueToken">
  /// The value to set or <see cref="LuaNil.Nil"/> to remove the existing value.
  /// </param>
  /// <exception cref="InvalidOperationException"></exception>
  public void SetValue(LuaKeyToken key, LuaToken valueToken)
  {
    if(valueToken is LuaValueToken value)
    {
      _content[key] = value;
      if(key is LuaNumber numberKey && numberKey.TryGetInteger(out var integerKey))
      {
        _intMap[integerKey] = value;
      }
    }
    else
    {
      if(valueToken.Type != LuaType.Nil)
      {
        throw new InvalidOperationException(
          $"Internal error values other than 'nil' must derive from LuaValueToken, but got type {valueToken.Type}");
      }
      RemoveValue(key);
    }
  }

  private void RemoveValue(LuaKeyToken key)
  {
    if(key is LuaNumber numKey && numKey.TryGetInteger(out var intKey))
    {
      _intMap.Remove(intKey);
    }
    _content.Remove(key);
  }

  /// <summary>
  /// Currently always returns the string "table"
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return "table";
  }

  /// <summary>
  /// Sort all entries, by key type and value in the order integer - real - boolean - string
  /// </summary>
  /// <exception cref="InvalidOperationException"></exception>
  public void Sort()
  {
    // Split by type into parts, sort each part, then glue everything back together
    var integers = new Dictionary<long, KeyValuePair<LuaKeyToken, LuaValueToken>>();
    var reals = new Dictionary<double, KeyValuePair<LuaKeyToken, LuaValueToken>>();
    var bools = new Dictionary<bool, KeyValuePair<LuaKeyToken, LuaValueToken>>();
    var strings = new Dictionary<string, KeyValuePair<LuaKeyToken, LuaValueToken>>();

    foreach(var kvp in _content)
    {
      var key = kvp.Key;
      switch(key)
      {
        case LuaNumber number:
          {
            if(number.TryGetInteger(out var integer))
            {
              integers[integer] = kvp;
            }
            else
            {
              reals[number.Value] = kvp;
            }
            break;
          }
        case LuaString text:
          strings[text.Value] = kvp;
          break;
        case LuaBoolean boolean:
          bools[boolean.Value] = kvp;
          break;
        default:
          throw new InvalidOperationException(
            $"missing handler for type '{key.Type}'");
      }
    }
    _content.Clear();
    _intMap.Clear();
    foreach(var kkvp in from x in integers orderby x.Key select x)
    {
      _intMap[kkvp.Key] = kkvp.Value.Value;
      _content[kkvp.Value.Key] = kkvp.Value.Value;
    }
    foreach(var kvp in from x in reals orderby x.Key select x.Value)
    {
      _content[kvp.Key] = kvp.Value;
    }
    foreach(var kvp in from x in bools orderby x.Key select x.Value)
    {
      _content[kvp.Key] = kvp.Value;
    }
    foreach(var kvp in from x in strings orderby x.Key select x.Value)
    {
      _content[kvp.Key] = kvp.Value;
    }
  }


}
