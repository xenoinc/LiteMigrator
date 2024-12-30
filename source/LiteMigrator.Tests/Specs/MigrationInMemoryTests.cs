/* Copyright Xeno Innovations, Inc. 2024
 * Date:    2024-12-29
 * Author:  Damian Suess
 * File:    MigrationInMemoryTests.cs
 * Description:
 *  In-Memory migration tests
 */

using System.Reflection;
using System.Threading.Tasks;
using LiteMigrator;

namespace LiteMigrator.SystemTests.Specs;

[TestClass]
public sealed class MigrationInMemoryTests : BaseTest
{
  private const string ScriptNamespace = "LiteMigrator.SystemTests.TestData.Scripts";

  [TestMethod]
  public async Task GetMigrationScriptsTestAsync()
  {
    // Initializes and performs migration.
    var migrator = new LiteMigration(Assembly.GetExecutingAssembly(), ScriptNamespace);

    // Find available migration scripts and those not installed
    var allMigrations = migrator.Migrations.GetSortedMigrations();
    var missing = await migrator.GetMissingMigrationsAsync();

    Assert.AreEqual(2, allMigrations.Count);
    Assert.IsNotNull(missing);
    Assert.AreEqual(allMigrations.Count, missing.Count);
  }

  [TestMethod]
  public async Task InstallMigrationsAsync()
  {
    using (var migrator = new LiteMigration(Assembly.GetExecutingAssembly(), ScriptNamespace))
    {
      // Act
      bool isSuccess = await migrator.MigrateUpAsync();

      // Double check if any are not installed
      var missing = await migrator.GetMissingMigrationsAsync();

      // Assert
      Assert.IsTrue(isSuccess, migrator.LastError);
      Assert.AreEqual(0, missing.Count);
    }
  }

  [TestCleanup]
  public void TestCleanup()
  {
    // This method is called after each test method.
  }

  [TestInitialize]
  public void TestInit()
  {
    // This method is called before each test method.
  }
}
