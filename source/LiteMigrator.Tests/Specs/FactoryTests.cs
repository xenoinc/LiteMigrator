/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-28
 * Author:  Damian Suess
 * File:    LiteMigratorFactoryTests.cs
 * Description:
 *  LiteMigrator Factory Tests
 */

using System.Reflection;

namespace LiteMigrator.SystemTests.Specs;

[TestClass]
public class LiteMigratorFactoryTests : BaseTest
{
  private const string BaseNamespace = "LiteMigrator.SystemTests.TestData.Scripts";
  private const string ScriptFullName = "201909150000-BaseDDL.sql";
  private const string ScriptName = "BaseDDL";
  private const long ScriptRevision = 201909150000;

  [TestMethod]
  [DataRow(false)]
  [DataRow(true)]
  public void GetMigrationScriptByNameTest(bool useExecutingAssm)
  {
    Assembly? assm = useExecutingAssm ? Assembly.GetExecutingAssembly() : null;

    // Arrange
    var migrator = new Migrator(InMemoryDatabasePath, BaseNamespace, assm);

    // Act
    string ns = migrator.Migrations.GetResourceNamed(ScriptName);
    migrator.Migrations.GetMigrationScriptByName(ns, out string? sql);

    // Assert
    Assert.IsNotNull(sql);
    Assert.IsFalse(string.IsNullOrEmpty(ns));
    Assert.IsFalse(string.IsNullOrEmpty(sql));
  }

  [TestMethod]
  [DataRow(false)]
  [DataRow(true)]
  public void GetMigrationScriptTest(bool useExecutingAssm)
  {
    Assembly? assm = useExecutingAssm ? Assembly.GetExecutingAssembly() : null;

    var migrator = new Migrator(InMemoryDatabasePath, BaseNamespace, assm);

    // Sample: "MyProject.Client.Business.Migrations.201909150000-BaseDDL.sql"
    bool success = migrator.Migrations.GetMigrationScriptByName(ScriptFullName, out string? data);

    Assert.IsTrue(success);
    Assert.IsNotNull(data);
    Assert.IsTrue(data.Length > 0, "Could not read migration script");
  }

  [TestMethod]
  [DataRow(false)]
  [DataRow(true)]
  public void GetMigrationScriptVerionTest(bool useExecutingAssm)
  {
    Assembly? assm = useExecutingAssm ? Assembly.GetExecutingAssembly() : null;

    var migrator = new Migrator(InMemoryDatabasePath, BaseNamespace, Assembly.GetExecutingAssembly());

    var results = migrator.Migrations.GetMigrationScriptByVersion(ScriptRevision, out string? sql);

    // Assert
    Assert.IsTrue(results);
    Assert.IsNotNull(sql);
    Assert.IsFalse(string.IsNullOrEmpty(sql));
  }

  [TestMethod]
  [DataRow(false)]
  [DataRow(true)]
  public void GetResourceNamedTest(bool useExecutingAssm)
  {
    Assembly? assm = useExecutingAssm ? Assembly.GetExecutingAssembly() : null;

    // Arrange
    var migrator = new Migrator(InMemoryDatabasePath, BaseNamespace, Assembly.GetExecutingAssembly());

    // Act
    string data = migrator.Migrations.GetResourceNamed(ScriptName);

    // Assert
    Assert.IsNotNull(data);
    Assert.IsTrue(data.Length > 0);
  }

  [TestMethod]
  [DataRow(false)]
  [DataRow(true)]
  public void GetResourcesTests(bool useExecutingAssm)
  {
    Assembly? assm = useExecutingAssm ? Assembly.GetExecutingAssembly() : null;

    // Arrange
    var migrator = new Migrator(InMemoryDatabasePath, BaseNamespace, Assembly.GetExecutingAssembly());

    // Act
    var items = migrator.Migrations.GetResources();

    // Assert
    System.Diagnostics.Debug.Print("======================");
    System.Diagnostics.Debug.Print("===[ GetResourcesTests");
    foreach (var item in items)
    {
      System.Diagnostics.Debug.Print("==> " + item);
    }

    Assert.IsTrue(items.Count > 0, $"Not migration scripts found in namespace, '{BaseNamespace}'");
  }

  [TestMethod]
  [DataRow(false)]
  [DataRow(true)]
  public void GetSortedMigrationsTest(bool useExecutingAssm)
  {
    Assembly? assm = useExecutingAssm ? Assembly.GetExecutingAssembly() : null;

    // Arrange
    long oldVer = 0;
    var migrator = new Migrator(InMemoryDatabasePath, BaseNamespace, Assembly.GetExecutingAssembly());

    // Act
    var items = migrator.Migrations.GetSortedMigrations();

    // Assert
    System.Diagnostics.Debug.Print("==============");
    System.Diagnostics.Debug.Print("===[ GetSortedMigrationsTest");

    bool found = false;

    foreach (var item in items)
    {
      Assert.IsTrue(item.Key > oldVer);
      oldVer = item.Key;

      System.Diagnostics.Debug.Print($"==> version: '{item.Key}' Name: '{item.Value}");
      found = true;
    }

    Assert.IsTrue(found, "No migration scripts found");
  }

  [TestMethod]
  [DataRow(false)]
  [DataRow(true)]
  public void ValidateMigrationNamingConventions(bool useExecutingAssm)
  {
    Assembly? assm = useExecutingAssm ? Assembly.GetExecutingAssembly() : null;

    // Arrange
    long oldVer = 0;
    var migrator = new Migrator(InMemoryDatabasePath, BaseNamespace, Assembly.GetExecutingAssembly());

    // Act
    var items = migrator.Migrations.GetSortedMigrations();

    // Assert
    System.Diagnostics.Debug.Print("==============");
    System.Diagnostics.Debug.Print("===[ GetSortedMigrationsTest");

    bool found = false;

    foreach (var item in items)
    {
      Assert.IsTrue(item.Key > oldVer);
      oldVer = item.Key;

      System.Diagnostics.Debug.Print($"==> version: '{item.Key}' Name: '{item.Value}");
      found = true;
    }

    Assert.IsTrue(found, "No migration scripts found");
  }
}
