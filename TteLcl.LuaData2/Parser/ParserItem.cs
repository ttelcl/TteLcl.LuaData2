using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using TteLcl.LuaData2.Types;
using TteLcl.LuaData2.Lexer;

namespace TteLcl.LuaData2.Parser;

/// <summary>
/// The object that can represent any item that needs to be on the parser stack.
/// The parser 'stack' is not an actual type, but a linked list of these
/// </summary>
public class ParserItem
{
  /// <summary>
  /// Create a new uninitialized ParserItem
  /// </summary>
  /// <param name="stack">
  /// The next item in the stack
  /// </param>
  private ParserItem(ParserItem? stack)
  {
    Next = stack;
    Type = ParserType.Unused;
    StartPosition = SourcePosition.Dummy;
  }

  /// <summary>
  /// Create a new stack
  /// </summary>
  /// <param name="global">
  /// A table builder for the global variable table
  /// </param>
  /// <returns>
  /// A new ParserItem acting as the initial (and final) parser stack
  /// </returns>
  public static ParserItem NewStack(LuaTableBuilder global)
  {
    return new ParserItem(null)
    {
      TableBuilder = global,
      Type = ParserType.TableBuilder,
    };
  }

  /// <summary>
  /// Get or set the parser type
  /// </summary>
  public ParserType Type { get; private set; }

  /// <summary>
  /// Get or set a name (for Name items and NameBuilder items)
  /// </summary>
  public LuaKeyToken? Name { get; private set; }

  /// <summary>
  /// Get or set a table builder (for TableBuilder items)
  /// </summary>
  public LuaTableBuilder? TableBuilder { get; private set; }

  /// <summary>
  /// Get or set a value (for Value items). Note that this can be LuaNil, so the
  /// type is <see cref="LuaToken"/>, not <see cref="LuaValueToken"/>.
  /// </summary>
  public LuaToken? Value { get; private set; }

  /// <summary>
  /// Get or set a start position for the item (for error messages)
  /// </summary>
  public SourcePosition StartPosition { get; private set; }

  /// <summary>
  /// Get the next item in the parser 'stack'
  /// </summary>
  public ParserItem? Next { get; private set; }

  /// <summary>
  /// Get the item two levels above in the stack
  /// </summary>
  public ParserItem? NextNext
  {
    get
    {
      return Next==null ? null : Next.Next;
    }
  }

  /// <summary>
  /// Push a token onto the stack and perform the subsequent processing
  /// </summary>
  /// <param name="token">
  /// The token to push
  /// </param>
  /// <returns>
  /// The new top of the stack
  /// </returns>
  public ParserItem PushToken(LexToken token)
  {
    ParserItem stack = PurePush(token);
    stack = Reduce(token.Start, stack);
    return stack;
  }

  /// <summary>
  /// Parse the LUA data file, loading the variables into
  /// the provided table builder
  /// </summary>
  /// <param name="filename">
  /// The name of the file
  /// </param>
  /// <param name="global">
  /// The table builder to hold the global variables
  /// </param>
  public static void ParseFile(string filename, LuaTableBuilder global)
  {
    ParserItem stack = NewStack(global);
    foreach(LexToken t in LexToken.Enumerate(filename, true))
    {
      stack = stack.PushToken(t);
    }
  }

  /// <summary>
  /// Parse the LUA data file, returning the loaded data as an anonymous
  /// table
  /// </summary>
  /// <param name="filename">
  /// The name of the file
  /// </param>
  /// <returns>
  /// The table with the variables loaded from the file
  /// </returns>
  public static LuaTable ParseFile(string filename)
  {
    var builder = new LuaTableBuilder();
    ParseFile(filename, builder);
    return builder.Bake();
  }

  /// <summary>
  /// Parse the LUA text, loading the variables into
  /// the provided table builder
  /// </summary>
  /// <param name="label">
  /// The label to identify the source in error messages
  /// </param>
  /// <param name="text">
  /// The raw lua text
  /// </param>
  /// <param name="global">
  /// The table builder to hold the global variables
  /// </param>
  public static void ParseString(string? label, string text, LuaTableBuilder global)
  {
    ParserItem stack = NewStack(global);
    foreach(LexToken t in LexToken.EnumerateString(label ?? "text", text, true))
    {
      stack = stack.PushToken(t);
    }
  }

  /// <summary>
  /// Parse the LUA text, loading the variables into
  /// the provided table builder
  /// </summary>
  /// <param name="text">
  /// The raw lua text
  /// </param>
  /// <param name="global">
  /// The table builder to hold the global variables
  /// </param>
  public static void ParseString(string text, LuaTableBuilder global)
  {
    ParseString(null, text, global);
  }

  /// <summary>
  /// Parse the LUA data file, returning the loaded data as an anonymous
  /// table
  /// </summary>
  /// <param name="label">
  /// The label to identify the source in error messages
  /// </param>
  /// <param name="text">
  /// The raw lua text
  /// </param>
  /// <returns>
  /// The table with the variables loaded from the text
  /// </returns>
  public static LuaTable ParseString(string? label, string text)
  {
    var builder = new LuaTableBuilder();
    ParseString(label, text, builder);
    return builder.Bake();
  }

  /// <summary>
  /// Parse the LUA data file, returning the loaded data as an anonymous
  /// table
  /// </summary>
  /// <param name="text">
  /// The raw lua text
  /// </param>
  /// <returns>
  /// The table with the variables loaded from the text
  /// </returns>
  public static LuaTable ParseString(string text)
  {
    return ParseString(null, text);
  }

  private ParserItem PurePush(LexToken token)
  {
    switch(token.Type)
    {
      default:
        throw new NotSupportedException(token.Start.ToString());

      case TokenType.Null:
        //throw new InvalidOperationException(
        //  $"{token.Start}: Internal/design error: not expecting a 'nil' here");
        return PushValue(token.Start, token.AsValue());

      case TokenType.Boolean:
      case TokenType.Number:
      case TokenType.String:
        return PushValue(token.Start, token.AsValue()!);
      
      case TokenType.Whitespace:
      case TokenType.Comment:
        // ignore
        return this;
      
      case TokenType.Identifier:
        Verify(token.Start, ParserType.TableBuilder);
        return PushName(token.Start, new LuaString(token.Value));
      
      case TokenType.TableOpen:
        return PushTableBuilder(token.Start, new LuaTableBuilder());
      
      case TokenType.TableClose:
        Verify(token.Start, ParserType.TableBuilder);
        if(Next==null)
        {
          throw new InvalidOperationException(String.Format(
            "{0}: Syntax error: superfluous '}'",
            token.Start));
        }
        if(TableBuilder == null)
        {
          throw new InvalidOperationException(
            $"{token.Start}: Expecting a table to be under construction when parsing '}}'");
        }
        return Next.PushValue(StartPosition, TableBuilder.Bake());
      
      case TokenType.NameOpen:
        Verify(token.Start, ParserType.TableBuilder);
        return PushNameBuilder(token.Start);
      
      case TokenType.NameClose:
        Verify(token.Start, ParserType.NameBuilder);
        if(Name==null)
        {
          throw new InvalidOperationException(String.Format(
            "{0}: Syntax error: expecting a name between '[' and ']'",
            token.Start));
        }
        if(Name.Type==LuaType.Table)
        {
          throw new InvalidOperationException(String.Format(
            "{0}: Syntax error: a table value cannot be used as name",
            StartPosition));
        }
        if(Next == null)
        {
          throw new InvalidOperationException(
            $"{token.Start}: not expecting a ']' here");
        }
        return Next.PushName(StartPosition, Name);
      
      case TokenType.Equals:
        Verify(token.Start, ParserType.Name);
        return this;
      
      case TokenType.Comma:
      case TokenType.Semicolon:
        Verify(token.Start, ParserType.TableBuilder);
        return this;
      
      case TokenType.Eof:
        Verify(token.Start, ParserType.TableBuilder);
        if(Next!=null)
        {
          throw new InvalidOperationException(String.Format(
            "{0}: Syntax error: unexpected end of file",
            token.Start));
        }
        // do not bake the world table, to allow multiple files to be loaded
        return this;
    }
  }

  private static ParserItem Reduce(SourcePosition? src, ParserItem stack)
  {
    while(stack.Type == ParserType.Value)
    {
      ParserItem value = stack;
      stack = stack.Next!;
      switch(stack.Type)
      {
        case ParserType.NameBuilder:
          if(stack.Name!=null)
          {
            throw new InvalidOperationException(String.Format(
              "{0}: Syntax error: expecting ']'",
              value.StartPosition));
          }
          if(value.Value is not LuaKeyToken keyToken)
          {
            throw new InvalidOperationException(String.Format(
              $"{value.StartPosition}: Syntax error: a '{value.Value!.Type}' value cannot be used as name"));
          }
          stack = stack.Next!.PushNameBuilder(stack.StartPosition, keyToken);
          break;

        case ParserType.TableBuilder:
          stack.TableBuilder!.Set(null, value.Value!);
          break;

        case ParserType.Name:
          ParserItem name = stack;
          stack = stack.Next!;
          if(stack.Type!=ParserType.TableBuilder)
          {
            throw new InvalidOperationException(String.Format(
              "{0}: internal error: item below NAME was not a TABLEBUILDER",
              value.StartPosition));
          }
          stack.TableBuilder!.Set(name.Name, value.Value!);
          break;

        default:
          throw new InvalidOperationException(String.Format(
            "{0}: Syntax error: a value is not expected here",
            value.StartPosition));
      }
    }
    return stack;
  }

  private void Verify(SourcePosition src, ParserType expected)
  {
    if(Type!=expected)
    {
      throw new InvalidOperationException(String.Format(
        "{0}: Syntax error: unexpected token",
        src
        ));
    }
  }

  ParserItem PushTableBuilder(SourcePosition src, LuaTableBuilder builder)
  {
    return new ParserItem(this)
    {
      Type = ParserType.TableBuilder,
      StartPosition = src,
      TableBuilder = builder,
    };
  }

  ParserItem PushValue(SourcePosition src, LuaToken value)
  {
    return new ParserItem(this)
    {
      Type = ParserType.Value,
      StartPosition = src,
      Value = value,
    };
  }

  ParserItem PushName(SourcePosition src, LuaKeyToken name)
  {
    return new ParserItem(this)
    {
      Type = ParserType.Name,
      StartPosition = src,
      Name = name,
    };
  }

  ParserItem PushNameBuilder(SourcePosition src, LuaKeyToken? name)
  {
    return new ParserItem(this)
    {
      Type = ParserType.NameBuilder,
      StartPosition = src,
      Name = name,
    };
  }

  ParserItem PushNameBuilder(SourcePosition src)
  {
    return PushNameBuilder(src, null);
  }
}

