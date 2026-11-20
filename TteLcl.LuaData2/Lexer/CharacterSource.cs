using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace TteLcl.LuaData2.Lexer;

/// <summary>
/// An object that wraps a source file and a 'current position' in it,
/// and line number and position.
/// </summary>
public class CharacterSource : IDisposable
{
  private TextReader? _source;
  private bool _hadCR;
  private bool _hadLF;

  /// <summary>
  /// Create a new <see cref="CharacterSource"/>, and position it at the first character
  /// </summary>
  /// <param name="name">
  /// The name to use for this source in error messages
  /// </param>
  /// <param name="source">
  /// The source stream.
  /// </param>
  /// <remarks>
  /// <para>
  /// When using this constructor, Disposing this object is optional
  /// if the source is disposed elsewhere.
  /// </para>
  /// </remarks>
  public CharacterSource(string name, TextReader source)
  {
    _source = source;
    SourceName = name;
    LineNumber = 1;
    LinePosition = 0;
    Next(); // position it at first character!
  }

  /// <summary>
  /// Create a new CharacterSource
  /// </summary>
  /// <param name="fileName">
  /// The name of the source file to open. It is assumed the content is encoded
  /// as UTF-8.
  /// </param>
  /// <remarks>
  /// <para>
  /// When using this constructor, be sure to dispose this object after use.
  /// </para>
  /// </remarks>
  public CharacterSource(string fileName)
    : this(new FileInfo(fileName).FullName, 
           new StreamReader(fileName, Encoding.UTF8, true))
  {
  }

  /// <summary>
  /// Get the name for the source to be used in error messages
  /// </summary>
  public string SourceName { get; private set; }

  /// <summary>
  /// Get the current line number
  /// </summary>
  public int LineNumber { get; private set; }

  /// <summary>
  /// Get the position in the current line
  /// </summary>
  public int LinePosition { get; private set; }

  /// <summary>
  /// Get the current character. This will be (char)0 before the first
  /// call to Next(), as well as at eof.
  /// </summary>
  public char Current { get; private set; }

  /// <summary>
  /// True when the end of the source has been reached.
  /// </summary>
  public bool IsEof { get; private set; }

  /// <summary>
  /// Move to the next character
  /// </summary>
  /// <returns>
  /// True on success, false on EOF
  /// </returns>
  public bool Next()
  {
    if(IsEof)
    {
      return false;
    }
    if(_source==null)
    {
      IsEof = true;
      Current = (char)0;
      return false;
    }
    var ich = _source.Read();
    if(ich<0)
    {
      IsEof = true;
      Current = (char)0;
      return false;
    }
    Current = (char)ich;
    // detect line ends: either as CRLF, CR, or LF
    if(Current=='\n') // certainly a line end. Ignore optional preceding CR
    {
      if(_hadLF) // two line breaks in a row
      {
        LineNumber++;
        LinePosition = 1;
      }
      else
      {
        LinePosition++;
      }
      _hadCR = false;
      _hadLF = true;
    }
    else if(Current=='\r')
    {
      if(_hadLF || _hadCR) // two line breaks in a row
      {
        LineNumber++;
        LinePosition = 1;
      }
      else
      {
        LinePosition++;
      }
      _hadCR = true;
      _hadLF = false;
    }
    else
    {
      if(_hadLF || _hadCR) // first character of a new line
      {
        LineNumber++;
        LinePosition = 1;
      }
      else
      {
        LinePosition++;
      }
      _hadCR = false;
      _hadLF = false;
    }
    return true;
  }

  /// <summary>
  /// Return a string indicating the specified position in this
  /// source (not necessarily the current position)
  /// </summary>
  public string LocationString(int line, int position)
  {
    return String.Format("{0} ({1}:{2})",
      SourceName, line, position);
  }

  /// <summary>
  /// Return a string indicating the current position in this source
  /// </summary>
  public string LocationString()
  {
    return LocationString(LineNumber, LinePosition);
  }

  /// <summary>
  /// Return an object that contains the current position information
  /// </summary>
  public SourcePosition GetPosition()
  {
    return new SourcePosition(this);
  }

  /// <summary>
  /// Dispose the underlying stream.
  /// </summary>
  public void Dispose()
  {
    if(_source!=null)
    {
      _source.Dispose();
      _source = null;
      IsEof = true;
    }
  }
}

