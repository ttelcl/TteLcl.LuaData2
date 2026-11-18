using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using TteLcl.LuaData2.Types;

namespace TteLcl.LuaData2.Lexer;

/// <summary>
/// Represents a token. This class acts as immutable. Use the Enumerate
/// method to create a stream of tokens.
/// </summary>
public class Token
{
  /// <summary>
  /// Create a new, uninitialized, Token
  /// </summary>
  private Token()
  {
    Type = TokenType.Unknown;
    Start = SourcePosition.Dummy;
    Value = String.Empty;
  }

  /// <summary>
  /// Create a new token and load it from the provided source
  /// </summary>
  internal Token(CharacterSource source)
    : this()
  {
    Load(source);
  }

  /// <summary>
  /// Enumerate the tokens in the source file
  /// </summary>
  public static IEnumerable<Token> Enumerate(string filename, bool includeEof)
  {
    using var source = new CharacterSource(filename);
    while(!source.IsEof)
    {
      yield return new Token(source);
    }
    if(includeEof)
    {
      yield return new Token(source);
    }
  }

  /// <summary>
  /// Emumerate the token(s) in the specified LUA fragment
  /// </summary>
  public static IEnumerable<Token> EnumerateString(
    string label, string luaText, bool includeEof)
  {
    using var source = new CharacterSource(label, new StringReader(luaText));
    while(!source.IsEof)
    {
      yield return new Token(source);
    }
    if(includeEof)
    {
      yield return new Token(source);
    }
  }

  /// <summary>
  /// The string 'true'
  /// </summary>
  public const string TrueString = "true";

  /// <summary>
  /// The string 'false'
  /// </summary>
  public const string FalseString = "false";

  /// <summary>
  /// The string 'nil'
  /// </summary>
  public const string NilString = "nil";

  /// <summary>
  /// The kind of token
  /// </summary>
  public TokenType Type { get; private set; }

  /// <summary>
  /// The start location of the token
  /// </summary>
  public SourcePosition Start { get; private set; }

  /// <summary>
  /// The string value of the token. For string literals, this is the
  /// token content with the quotes removed. For other token types this
  /// is the token content.
  /// </summary>
  public string Value { get; private set; }

  /// <summary>
  /// Create a new <see cref="LuaToken"/> wrapping the value of this token if appropriate,
  /// or return null for tokens that do not represent a value.
  /// </summary>
  public LuaValueToken? AsValue()
  {
    switch(Type)
    {
      case TokenType.Boolean:
        return LuaToken.Create(Value==TrueString);
      case TokenType.Null:
        throw new InvalidOperationException(
          $"Internal error: 'nil' not supported here");
        //return LuaNil.Nil;
      case TokenType.Number:
        return LuaToken.Create(Double.Parse(Value, CultureInfo.InvariantCulture));
      case TokenType.String:
        return LuaToken.Create(Value);
      default:
        return null;
    }
  }

  /// <summary>
  /// Create a new <see cref="LuaToken"/> wrapping the value of this token if appropriate,
  /// or return null for tokens that do not represent a value in search expressions.
  /// This method differs from <see cref="AsValue"/> in that Identifiers are converted to strings
  /// </summary>
  public LuaToken? AsLiteral()
  {
    switch(Type)
    {
      case TokenType.Boolean:
      case TokenType.Null:
      case TokenType.Number:
      case TokenType.String:
        return AsValue();
      case TokenType.Identifier:
        return LuaToken.Create(Value);
      default:
        return null;
    }
  }

  /// <summary>
  /// Initialize this instance by reading the token from the source
  /// </summary>
  private void Load(CharacterSource source)
  {
    Start = source.GetPosition();
    if(source.IsEof)
    {
      Type = TokenType.Eof;
      return;
    }
    // Handle the single-letter cases:
    switch(source.Current)
    {
      case '{':
        Type = TokenType.TableOpen;
        Value = "{"; // bind to this constant string instead of creating a new string
        source.Next();
        return;
      case '}':
        Type = TokenType.TableClose;
        Value = "}"; // bind to this constant string instead of creating a new string
        source.Next();
        return;
      case '[':
        Type = TokenType.NameOpen;
        Value = "["; // bind to this constant string instead of creating a new string
        source.Next();
        return;
      case ']':
        Type = TokenType.NameClose;
        Value = "]"; // bind to this constant string instead of creating a new string
        source.Next();
        return;
      case '=':
        Type = TokenType.Equals;
        Value = "="; // bind to this constant string instead of creating a new string
        source.Next();
        return;
      case ',':
        Type = TokenType.Comma;
        Value = ","; // bind to this constant string instead of creating a new string
        source.Next();
        return;
      case ';':
        Type = TokenType.Semicolon;
        Value = ";"; // bind to this constant string instead of creating a new string
        source.Next();
        return;
    }
    if(Char.IsWhiteSpace(source.Current))
    {
      LoadWhitespace(source);
      return;
    }
    if(source.Current=='\'' || source.Current=='"')
    {
      LoadString(source);
      return;
    }
    if(Char.IsLetter(source.Current) || source.Current=='_')
    {
      LoadIdentifier(source);
      return;
    }
    if(source.Current=='-')
    {
      LoadCommentOrNegativeNumber(source);
      return;
    }
    if(Char.IsDigit(source.Current))
    {
      LoadNumber(source);
      return;
    }
    throw LexicalError(source,
      String.Format("Unexpected character: {0}", source.Current));
  }

  private void LoadNumber(CharacterSource source)
  {
    var sb = new StringBuilder();
    ContinueLoadingNumber(source, sb);
  }

  private void LoadCommentOrNegativeNumber(CharacterSource source)
  {
    var sb = new StringBuilder();
    sb.Append(source.Current);
    if(!source.Next())
    {
      throw LexicalError(source, "Unexpected end of file");
    }
    if(Char.IsDigit(source.Current))
    {
      ContinueLoadingNumber(source, sb);
    }
    else if(source.Current=='-')
    {
      ContinueLoadingComment(source, sb);
    }
    else
    {
      throw LexicalError(source, "Unexpected character after initial '-'");
    }
  }

  private void ContinueLoadingNumber(CharacterSource source, StringBuilder sb)
  {
    Type = TokenType.Number;
    do
    {
      sb.Append(source.Current);
    } while(source.Next() &&
      (Char.IsDigit(source.Current) || source.Current=='.'));
    // (exponential notation is not handled, and incorrect numbers like
    // 1.2.3 are silently accepted)
    Value = sb.ToString();
  }

  private void ContinueLoadingComment(CharacterSource source, StringBuilder sb)
  {
    Type = TokenType.Comment;
    do
    {
      sb.Append(source.Current);
    } while(source.Next() && source.Current!='\n' && source.Current!='\r');
    Value = sb.ToString();
  }

  private void LoadIdentifier(CharacterSource source)
  {
    Type = TokenType.Identifier;
    var sb = new StringBuilder();
    do
    {
      sb.Append(source.Current);
    } while(source.Next() &&
      (Char.IsLetterOrDigit(source.Current) || source.Current=='_'));
    Value = sb.ToString();

    // Fix special identifiers (also: normalize their values)
    switch(Value)
    {
      case "true":
        Type = TokenType.Boolean;
        Value = TrueString;
        break;
      case "false":
        Type = TokenType.Boolean;
        Value = FalseString;
        break;
      case "nil":
        Type = TokenType.Null;
        Value = NilString;
        break;
    }
  }

  private void LoadString(CharacterSource source)
  {
    Type = TokenType.String;
    var terminator = source.Current;
    source.Next(); // skip opening quote
    var sb = new StringBuilder();
    while(!source.IsEof && source.Current!=terminator)
    {
      if(source.Current=='\n' || source.Current=='\r')
      {
        throw LexicalError(source, "Newline in string constant");
      }
      if(source.Current=='\\')
      {
        if(!source.Next())
        {
          throw LexicalError(source, "EOF in string escape");
        }
        switch(source.Current)
        {
          case 'n':
            sb.Append('\n');
            break;
          case 'r':
            sb.Append('\r');
            break;
          case '\\':
            sb.Append('\\');
            break;
          case 't':
            sb.Append('\t');
            break;
          case '\'':
            sb.Append('\'');
            break;
          case '"':
            sb.Append('"');
            break;
          default:
            throw LexicalError(source, "Unrecognized escape sequence in string");
        }
      }
      else
      {
        sb.Append(source.Current);
      }
      source.Next();
    }
    if(source.IsEof)
    {
      throw LexicalError(source, "Unterminated string");
    }
    Value = sb.ToString();
    source.Next(); // skip terminating quote
  }

  private void LoadWhitespace(CharacterSource source)
  {
    Type = TokenType.Whitespace;
    var sb = new StringBuilder();
    do
    {
      sb.Append(source.Current);
    } while(source.Next() && Char.IsWhiteSpace(source.Current));
    Value = sb.ToString();
  }

  private InvalidOperationException LexicalError(
    CharacterSource source, string message)
  {
    return new InvalidOperationException(
      String.Format("Lexical error at {0}: {1}",
      source.GetPosition(), message));
  }
}
