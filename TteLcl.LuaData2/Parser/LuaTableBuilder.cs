using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TteLcl.LuaData2.Types;

namespace TteLcl.LuaData2.Parser;

/// <summary>
/// Helper object for building up a <see cref="LuaTable"/>
/// </summary>
public class LuaTableBuilder
{
  private bool _cloneOnModify;
  private int _lastAutoKey;
  private LuaTable _target;

  /// <summary>
  /// Create a new LuaTableBuilder
  /// </summary>
  public LuaTableBuilder(bool noSort = false)
  {
    _target = new LuaTable();
    _lastAutoKey = 0;
    NoSort=noSort;
  }

  /// <summary>
  /// Get the no-sort flag, that when set, prevents tables from being
  /// sorted when baked.
  /// </summary>
  public bool NoSort { get; }

  /// <summary>
  /// Add the content of the specified file to this builder
  /// </summary>
  /// <param name="filename">
  /// The name of the file to load
  /// </param>
  public void Load(string filename)
  {
    ParserItem.ParseFile(filename, this);
  }

  /// <summary>
  /// Return the built table (and prevent changes to it)
  /// </summary>
  /// <param name="sort">
  /// When true, the entries in the returned table are sorted.
  /// </param>
  /// <remarks>
  /// <para>
  /// Changing this builder after calling this method is allowed,
  /// but changes will not be visible in the table that was
  /// returned
  /// </para>
  /// </remarks>
  public LuaTable Bake(bool sort)
  {
    _cloneOnModify = true;
    if(sort)
    {
      _target.Sort();
    }
    return _target;
  }
  /// <summary>
  /// Return the built table.
  /// Whether or not the resulting table is sorted depends on <see cref="NoSort"/>.
  /// </summary>
  /// <remarks>
  /// Changing this builder after calling this method is allowed,
  /// but changes will not be visible in the table that was
  /// returned - a new target for this builder will be cloned instead
  /// </remarks>
  public LuaTable Bake()
  {
    return Bake(!NoSort);
  }

  /// <summary>
  /// Set an entry to the specified value, or remove an entry
  /// </summary>
  /// <param name="key">
  /// The key to change. When null, an automatically numbered integer
  /// key is generated.
  /// </param>
  /// <param name="value">
  /// The value to set, or null to remove (not LuaNull.Nil!)
  /// </param>
  public void Set(LuaKeyToken? key, LuaToken value)
  {
    if(_cloneOnModify)
    {
      _cloneOnModify = false;
      throw new NotImplementedException("cloning");
      //_target = _target.Clone();
    }
    if(key==null)
    {
      Add(value);
    }
    else
    {
      _target.SetValue(key, value);
    }
  }

  private void Add(LuaToken value)
  {
    // this implementation is flawed, but good enough for now
    var key = new LuaNumber(++_lastAutoKey);
    _target.SetValue(key, value);
  }

}
