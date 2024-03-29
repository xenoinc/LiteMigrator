﻿/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-10-6
 * Author:  Damian Suess
 * File:    LiteMigratorExecuteTests.cs
 * Description:
 *  Migration execution tests
 */

using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite;
using Xeno.LiteMigrator.DataObjects;

namespace Xeno.LiteMigrator.SystemTests.Specs
{
  /// <summary>LiteMigrator Tests.</summary>
  [TestCategory("Database")]
  [TestClass]
  public class LiteMigratorExecuteTests : BaseTest
  {
    private readonly string _baseNamespace = "Xeno.LiteMigrator.SystemTests.TestData.Scripts";

    public override void CleanupBeforeTest()
    {
      base.CleanupBeforeTest();

      DeleteDatabase();
    }

    /// <summary>
    ///   Performs a migration
    ///   WARNING:
    ///     This is not the right way to test this!!
    ///     We should create a new "test" namespace to pull from.
    /// </summary>
    /// <returns>Task.</returns>
    [TestMethod]
    public async Task InstallMigrationsTestAsync()
    {
      DeleteDatabase();

      var resourceAssm = Assembly.GetExecutingAssembly();
      var mig = new LiteMigration(TempDatabasePath, resourceAssm, _baseNamespace);

      var allMigs = mig.Migrations.GetSortedMigrations();
      var missing = await mig.GetMissingMigrationsAsync();

      Assert.IsTrue(allMigs.Count > 0);
      Assert.IsNotNull(missing);
      Assert.AreEqual(allMigs.Count, missing.Count);

      // Act
      bool success = await mig.MigrateUpAsync();

      // Assert
      Assert.IsTrue(success, mig.LastError);
      missing = await mig.GetMissingMigrationsAsync();
      Assert.AreEqual(0, missing.Count);
    }

    /// <summary>Gets list of migrations not applied yet.</summary>
    /// <returns>Task.</returns>
    [TestMethod]
    public async Task NotAllMigrationsAreInstalledTestAsync()
    {
      // Get not installed scripts.
      // returns sorted list of IMigration with namespace path to resource
      ClearVersionInfo();

      var resourceAssm = Assembly.GetExecutingAssembly();
      var mig = new LiteMigration(TempDatabasePath, resourceAssm, _baseNamespace);

      var allMigs = mig.Migrations.GetSortedMigrations();
      var missing = await mig.GetMissingMigrationsAsync();

      Assert.IsTrue(allMigs.Count > 0);
      Assert.IsNotNull(missing);
      Assert.AreEqual(allMigs.Count, missing.Count);
    }

    private void ClearVersionInfo()
    {
      using (SQLiteConnection db = new SQLiteConnection(TempDatabasePath))
      {
        var columnInfo = db.GetTableInfo(nameof(VersionInfo));
        if (columnInfo.Count > 0)
          db.DeleteAll<VersionInfo>();
      }
    }
  }
}
