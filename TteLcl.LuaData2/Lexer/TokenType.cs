using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TteLcl.LuaData2.Lexer;

/// <summary>
/// The various types of tokens
/// </summary>
public enum TokenType
{
  /// <summary>
  /// The token type is not yet known
  /// </summary>
  Unknown,

  /// <summary>
  /// End of file
  /// </summary>
  Eof,

  /// <summary>
  /// A whitespace sequence
  /// </summary>
  Whitespace,

  /// <summary>
  /// A comment
  /// </summary>
  Comment,

  /// <summary>
  /// A string literal (originally enclosed in quotes)
  /// </summary>
  String,

  /// <summary>
  /// An identifier (a letter/digit sequence). Some identifiers
  /// are treated as dedicated token types.
  /// </summary>
  Identifier,

  /// <summary>
  /// A number (integer or real; may have a sign)
  /// </summary>
  Number,

  /// <summary>
  /// A boolean constant ('true' or 'false')
  /// </summary>
  Boolean,

  /// <summary>
  /// The null constant ('nil')
  /// </summary>
  Null,

  /// <summary>
  /// The start-of-table character ('{')
  /// </summary>
  TableOpen,

  /// <summary>
  /// The end-of-table character ('}')
  /// </summary>
  TableClose,

  /// <summary>
  /// The start-of-name character ('[')
  /// </summary>
  NameOpen,

  /// <summary>
  /// The end-of-name character (']')
  /// </summary>
  NameClose,

  /// <summary>
  /// The equals sign ('=')
  /// </summary>
  Equals,

  /// <summary>
  /// The comma (',')
  /// </summary>
  Comma,

  /// <summary>
  /// The semicolon (';')
  /// </summary>
  Semicolon,
}

