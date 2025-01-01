/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-28
 * Author:  Damian Suess
 * File:    LiteMigratorVersionInfoTests.cs
 * Description:
 *  LiteMigrator Tests
 *
 * Note:
 * 1. These tests are flaky and need updated. A database may not be created
 *    or closed in-time when the next execution occurs.
 * 2. Cannot always perform in-memory tests ":memory:" because we're creating
 *    multiple connections in these tests.
 */

using System.Threading.Tasks;
using SQLite;
using LiteMigrator;
using LiteMigrator.DataObjects;
using LiteMigrator.Versioning;

namespace LiteMigrator.SystemTests.Specs
{
  /// <summary>LiteMigrator Tests.</summary>
  [TestCategory("Database")]
  [TestClass]
  public class LiteMigratorVersionInfoTests : BaseTest
  {
    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext { get; set; }

    [TestMethod]
    public async Task VersionInfoCanAddInstalledVersionsTestAsync()
    {
      // Arrange
      ClearVersionInfo();

      var mig = new LiteMigration(TempDatabasePath, string.Empty);

      // Act
      SQLiteAsyncConnection db = new SQLiteAsyncConnection(TempDatabasePath);
      await AddVersionInfoForTestsAsync(db, mig);

      // Assert
      var tbl = db.Table<VersionInfo>();
      var count = await tbl.CountAsync();

      Assert.AreEqual(3, count);

      await db.CloseAsync();
    }

    [TestMethod]
    public async Task CanCreateVersionInfoTable_InFile_TestAsync()
    {
      using (var mig = new LiteMigration(TempDatabasePath, string.Empty))
      {
        var columnInfo = await mig.Connection.GetTableInfoAsync(nameof(VersionInfo));

        Assert.AreNotEqual(0, columnInfo.Count, "VersionInfo table was not created.");
      }
    }

    [TestMethod]
    public async Task CanCreateVersionInfoTable_InMemory_TestAsync()
    {
      using (var mig = new LiteMigration())
      {
        var columnInfo = await mig.Connection.GetTableInfoAsync(nameof(VersionInfo));

        Assert.AreNotEqual(0, columnInfo.Count, "VersionInfo table was not created.");
      }
    }

    [TestMethod]
    public async Task VersionInfoCanGetInstalledMigrationsAsync()
    {
      // Arrange
      ClearVersionInfo();
      var mig = new LiteMigration(TempDatabasePath, string.Empty);

      SQLiteAsyncConnection db = new SQLiteAsyncConnection(TempDatabasePath);
      await AddVersionInfoForTestsAsync(db, mig);
      await db.CloseAsync();

      // Act
      var list = await mig.GetInstalledMigrationsAsync();

      // Assert
      Assert.AreEqual(3, list.Count, "Expected 3 registered items");
      Assert.IsTrue(list.ContainsKey(199609081025), "Missing Migration, 'When it all began'");
      Assert.IsTrue(list.ContainsKey(201912312359), "Missing Migration, 'Some update'");
      Assert.IsTrue(list.ContainsKey(202512312359), "Missing Migration, 'LTS update'");
    }

    private async Task AddVersionInfoForTestsAsync(SQLiteAsyncConnection db, LiteMigration mig)
    {
      await mig.RegisterVersionAsync(db, new Migration(199609081025, "When it all began"));
      await mig.RegisterVersionAsync(db, new Migration(201912312359, "Some update"));
      await mig.RegisterVersionAsync(db, new Migration(202512312359, "LTS Update"));
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
