/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-15
 * Author:  Damian Suess
 * File:    LiteMigrator.Scripts.cs
 * Description:
 *  Simple SQL Migration Scripts Engine
 */

using System;
using System.Threading.Tasks;
using LiteMigrator.DataObjects;
using LiteMigrator.Factory;

namespace LiteMigrator;

/// <summary>LiteMigration, migration scripts.</summary>
public partial class LiteMigration
{
  /*
  /// <summary>Executes a single migration script.</summary>
  /// <param name="migration">Migration script information.</param>
  /// <returns>True if executed without errors.</returns>
  [Obsolete("Not implemented.")]
  public async Task<bool> MigrateUpAsync(IMigration migration)
  {
    await Task.Yield();
    throw new NotImplementedException();
  }
  */

  /// <summary>
  ///   Executes all missing migration scripts inside a database transaction
  ///   so it can rollback all upgrades upon failure.
  ///
  ///   OnSuccess: Remove backup / commit changes.
  ///   OnFailure: Rollback upon failure.
  /// </summary>
  /// <returns>True if executed without errors.</returns>
  public async Task<bool> MigrateUpAsync()
  {
    bool hasError = false;
    string errMsg = string.Empty;

    bool initOk = await ReinitializeAsync();

    if (!initOk)
    {
      hasError = true;
      errMsg = "Could not initialize migration VersionInfo table";
    }
    else
    {
      var allMigs = Migrations.GetSortedMigrations();
      var missing = await GetMissingMigrationsAsync();

      // _sqlEngine.Connect(DatabasePath);
      //// SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);

      foreach (var mig in missing)
      {
        string path = mig.Value.Script;
        string migScript = Migrations.GetMigrationScriptByResource(path);
        var parser = new ParserFactory(migScript);

        // isOk = _sqlEngine.ExecuteMigration(mig, out errMsg);
        // if (!isOk) break;
        await Connection.RunInTransactionAsync(tran =>
        {
          // Cannot begin a transaction while in one
          // tran.BeginTransaction();
          try
          {
            //// Only executes the 1st statement
            //// int x = tran.Execute(query);

            int cmdCount = 0;
            string lastQuery = string.Empty;
            string currentQuery = string.Empty;

            try
            {
              // Split commands by ';'
              var commands = parser.GetCommands(migScript);

              // Belongs in ParseFactory
              int beginCount = 0;
              string cmdCache = string.Empty;

              // Execute each command
              // TODO: Move into ParseFactory
              foreach (var query in commands)
              {
                if (!parser.IsCommand(query))
                  continue;

                parser.Concat(query);   // cmdCache += query;

                if (parser.CurrentCommand.Contains("CREATE TRIGGER"))
                {
                  // Make sure every BEGIN has an END before continuing !
                  if (query.Contains("BEGIN"))
                    beginCount++;

                  if (query.Contains("END;"))
                    beginCount--;

                  if (beginCount > 0)
                    continue;
                }

                ++cmdCount;

                // Increment the execution line
                parser.CountLines(query);
                currentQuery = parser.CurrentCommand;   // currentQuery = cmdCache;

                var cmd = tran.CreateCommand(currentQuery);
                cmd.ExecuteNonQuery();
                parser.ClearCommand();

                lastQuery = currentQuery;
              }
            }
            catch (Exception execEx)
            {
              string err = $"Migration failed on Command number: '{cmdCount}'{Environment.NewLine}" +
                           $"* Exception: {execEx.Message}{Environment.NewLine}" +
                           $"* Line: {parser.Lines}{Environment.NewLine}" +
                           $"* Path: '{path}'{Environment.NewLine}" +
                           $"* Query: {currentQuery}{Environment.NewLine}" +
                           $"* Previous Query: {lastQuery}";

              System.Diagnostics.Debug.WriteLine($"[ERROR] [LiteMigration] [{err}]");
              LastException = execEx;
              throw new Exception(err);
            }

            var verInfo = new VersionInfo()
            {
              AppliedDttm = DateTime.Now,
              Description = mig.Value.Description,
              VersionNumber = mig.Value.VersionNumber,
            };

            tran.Insert(verInfo);

            AddVersion(mig.Value);

            tran.Commit();
          }
          catch (Exception ex)
          {
            hasError = true;
            errMsg = ex.Message;
            tran.Rollback();

            // TODO: Report which migration script & error message
            LastError = errMsg;
            LastException = ex;
          }
        });

        if (hasError)
          break;
      }

      // _sqlEngine.Close()
      //// await Connection.CloseAsync();
    }

    if (hasError)
    {
      // Why did we get an error
      System.Diagnostics.Debug.WriteLine("LiteMigrator - ERROR - " + LastError);
    }

    return !hasError;
  }

  public async Task<bool> ReinitializeAsync()
  {
    // Migrations = new MigrationFactory();

    // Maybe use Lazy loading?
    ////Versions = new VersionFactory();
    ////Versions.Initialize();

    await VersionInitializeAsync();

    return true;
  }
}
