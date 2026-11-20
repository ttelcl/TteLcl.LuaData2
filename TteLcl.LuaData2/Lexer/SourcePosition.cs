using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TteLcl.LuaData2.Lexer;

/// <summary>
/// A struct to capture a position in a CharacterSource for error messages.
/// </summary>
public struct SourcePosition
{
  /// <summary>
  /// Create a new SourcePosition
  /// </summary>
  public SourcePosition(CharacterSource source)
  {
    Source = source.SourceName;
    Line = source.LineNumber;
    Position = source.LinePosition;
  }

  /// <summary>
  /// Create a fake SourcePosition
  /// </summary>
  private SourcePosition(string source)
  {
    Source = source;
    Line = 0;
    Position = 0;
  }

  /// <summary>
  /// Get a dummy source position for use in situations where there is no source
  /// </summary>
  public static SourcePosition Dummy = new SourcePosition("[global]");

  /// <summary>
  /// Get the name of the source object for the position
  /// </summary>
  public string Source { get; private set; }

  /// <summary>
  /// Get the line number
  /// </summary>
  public int Line { get; private set; }

  /// <summary>
  /// Get the character position in the line
  /// </summary>
  public int Position { get; private set; }

  /// <summary>
  /// Format the information in this object
  /// </summary>
  public override string ToString()
  {
    return String.Format("{0} ({1}:{2})",
      Source, Line, Position);
  }
}
