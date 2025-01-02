/* Copyright Xeno Innovations, Inc. 2024
 * Date:    2024-12-29
 * Author:  Damian Suess
 * File:    MigrationInMemoryTests.cs
 * Description:
 *  In-Memory migration tests
 */

using System.Reflection;
using System.Threading.Tasks;

namespace LiteMigrator.SystemTests.Specs;

[TestClass]
public sealed class MigrationInMemoryTests : BaseTest
{
  private const string ScriptNamespace = "LiteMigrator.SystemTests.TestData.Scripts";

  [TestMethod]
  [DataRow(false)]
  [DataRow(true)]
  public async Task GetMigrationScriptsTestAsync(bool useExecutingAssm)
  {
    Assembly? assm = useExecutingAssm ? Assembly.GetExecutingAssembly() : null;

    // Initializes and performs migration.
    var migrator = new Migrator(InMemoryDatabasePath, ScriptNamespace, assm);

    // Find available migration scripts and those not installed
    var allMigrations = migrator.Migrations.GetSortedMigrations();
    var missing = await migrator.GetMissingMigrationsAsync();

    Assert.AreEqual(2, allMigrations.Count);
    Assert.IsNotNull(missing);
    Assert.AreEqual(allMigrations.Count, missing.Count);
  }

  [TestMethod]
  [DataRow(false)]
  [DataRow(true)]
  public async Task InstallMigrationsAsync(bool useExecutingAssm)
  {
    Assembly? assm = useExecutingAssm ? Assembly.GetExecutingAssembly() : null;

    using (var migrator = new Migrator(InMemoryDatabasePath, ScriptNamespace, assm))
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
}
