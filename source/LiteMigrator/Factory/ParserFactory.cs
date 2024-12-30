/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-10-13
 * Author:  Damian Suess
 * File:    ParserFactory.cs
 * Description:
 *  SQL query parser and execution
 *
 * Limitations:
 *  - Every command must end with a semicolon ';'
 *  - BEGIN and END statements must be on their own line
 */

using System.Text.RegularExpressions;

namespace LiteMigrator.Factory
{
  public class ParserFactory
  {
    /// <summary>Current command to be executed.</summary>
    private string _currentCommand;

    /// <summary>The entire script.</summary>
    private string _script;

    public ParserFactory()
        : this(string.Empty)
    {
    }

    public ParserFactory(string script)
    {
      _script = script;
      Reset();
    }

    /// <summary>Gets or sets the parsed command to execute.</summary>
    /// <value>The parsed command to execute.</value>
    public string CurrentCommand
    {
      get => _currentCommand;
      set => _currentCommand = value;
    }

    /// <summary>Gets or sets a value indicating whether to count lines or not.</summary>
    /// <value>Number of lines parsed so far.</value>
    public bool EnableCounter { get; set; } = true;

    /// <summary>Gets number of lines counted so far.</summary>
    /// <value>Current line executed so far.</value>
    public int Lines { get; private set; }

    /// <summary>Gets or sets the main script to execute.</summary>
    /// <value>The main script file contents.</value>
    public string Script
    {
      get => _script;
      set => _script = value;
    }

    /// <summary>Gets total lines in main script value.</summary>
    /// <value>Total lines in script.</value>
    public int TotalLines { get; private set; }

    public void ClearCommand()
    {
      _currentCommand = string.Empty;
    }

    public void Concat(string commandChunk)
    {
      _currentCommand += commandChunk;
    }

    public int CountLines(bool ignore = false)
    {
      return CountLines(_currentCommand, ignore);
    }

    public int CountLines(string script, bool ignore = false)
    {
      int count = 0;
      int len = script.Length;

      for (int i = 0; i != len; ++i)
      {
        switch (script[i])
        {
          case '\r':
            ++count;
            if (i + 1 != len && script[i + 1] == '\n')
              ++i;
            break;

          case '\n':
            // Uncomment below to include all other line break sequences
            // case '\u000A':
            // case '\v':
            // case '\f':
            // case '\u0085':
            // case '\u2028':
            // case '\u2029':
            ++count;
            break;
        }
      }

      if (!ignore && EnableCounter)
        Lines += count;

      return count;
    }

    public string[] GetCommands()
    {
      return GetCommands(_currentCommand);
    }

    public string[] GetCommands(string script)
    {
      // TODO: ignore semicolons in comments
      TotalLines = CountLines(script, ignore: true);
      Lines = 0;

      // If the split chars were ,, ., and ;, I'd try:
      //  Regex.Split(originalString, @"(?<=[.,;])
      //
      // Old:
      //  var commands = script.Split(';');
      string[] commands = Regex.Split(script, @"(?<=[;])");

      return commands;
    }

    public bool IsCommand(string query)
    {
      string clean = Regex.Replace(query, @"\t|\n|\r", string.Empty);

      if (string.IsNullOrWhiteSpace(clean))
        return false;

      if (string.IsNullOrEmpty(clean))
        return false;

      return true;
    }

    public void Reset()
    {
      Lines = 0;
      TotalLines = 0;
      _currentCommand = string.Empty;
    }
  }
}
