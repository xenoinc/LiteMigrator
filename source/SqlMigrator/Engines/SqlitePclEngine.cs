/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-10-6
 * Author:  Damian Suess
 * File:    SqlitePclEngine.cs
 * Description:
 *  LiteMigrator using the SQLite-Net-PCL engine
 */

using Xeno.LiteMigrator.Versioning;

namespace Xeno.LiteMigrator.Engines
{
  internal class SqlitePclEngine : IEngine
  {
    //// private MigrationFactory _migrationFactory;

    public SqlitePclEngine()
    {
    }

    ////public SqlitePclEngine(MigrationFactory migrationFactory)
    ////{
    ////  _migrationFactory = migrationFactory;
    ////}

    public string LastErrorMessage { get; private set; }

    /// <summary>Execute all missing migrations.</summary>
    /// <returns>Pass/Fail.</returns>
    public bool MigrateUp()
    {
      // Get list of all migrations not installed
      // Execute each script
      // Return results if failure

      ////var allMigs = _migrationFactory.GetSortedMigrations();
      ////var missing = await GetMissingMigrationsAsync();
      ////
      ////foreach (var mig in missing)
      ////{
      ////}

      throw new System.NotImplementedException();
      return false;
    }

    /// <summary>Execute a specific migration script if it doesn't exist.</summary>
    /// <param name="migration">Migration version.</param>
    /// <returns>Pass/Fail.</returns>
    public bool MigrateUp(IMigration migration)
    {
      // 1. Lookup migration script
      // 2. Execute query
      throw new System.NotImplementedException();
    }

    public void VersionInitialize()
    {
      throw new System.NotImplementedException();
    }

    private bool ExecuteQuery()
    {
      return false;
    }
  }
}
