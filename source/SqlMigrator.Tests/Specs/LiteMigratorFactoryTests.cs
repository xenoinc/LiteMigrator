/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-28
 * Author:  Damian Suess
 * File:    LiteMigratorFactoryTests.cs
 * Description:
 *  LiteMigrator Factory Tests
 */

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xeno.LiteMigrator.SystemTests.Specs
{
  [TestClass]
  public class LiteMigratorFactoryTests : BaseTest
  {
    private readonly string _baseNamespace = "Xeno.LiteMigrator.SystemTests.TestData.Scripts";
    private readonly string _script001 = "201909150000-BaseDDL.sql";
    private readonly string _scriptName = "BaseDDL";
    private readonly long _scriptRevision = 201909150000;

    [TestMethod]
    public void GetMigrationScriptNamedTest()
    {
      // Arrange
      var resourceAssm = Assembly.GetExecutingAssembly();
      var migrator = new LiteMigration(resourceAssm, _baseNamespace);

      // Act
      string ns = migrator.Migrations.GetResourceNamed(_scriptName);
      migrator.Migrations.GetMigrationScriptByName(ns, out string sql);

      // Assert
      Assert.IsTrue(!string.IsNullOrEmpty(ns));
      Assert.IsTrue(!string.IsNullOrEmpty(sql));
      Assert.IsNotNull(sql);
    }

    [TestMethod]
    public void GetMigrationScriptTest()
    {
      var assm = Assembly.GetExecutingAssembly();
      var migrator = new LiteMigration(assm, _baseNamespace);

      // Sample: "Xeno.MyProject.Client.Business.Migrations.201909150000-BaseDDL.sql"
      bool success = migrator.Migrations.GetMigrationScriptByName(_script001, out string data);

      Assert.IsTrue(success);
      Assert.IsNotNull(data);
      Assert.IsTrue(data.Length > 0, "Could not read migration script");
    }

    [TestMethod]
    public void GetMigrationScriptVerionTest()
    {
      var resourceAssm = Assembly.GetExecutingAssembly();
      var migrator = new LiteMigration(resourceAssm, _baseNamespace);

      var results = migrator.Migrations.GetMigrationScriptByVersion(_scriptRevision, out string sql);

      // Assert
      Assert.IsTrue(results);
      Assert.IsTrue(!string.IsNullOrEmpty(sql));
      Assert.IsNotNull(sql);
    }

    [TestMethod]
    public void GetResourceNamedTest()
    {
      // Arrange
      var resourceAssm = Assembly.GetExecutingAssembly();
      var migrator = new LiteMigration(resourceAssm, _baseNamespace);

      // Act
      string data = migrator.Migrations.GetResourceNamed(_scriptName);

      // Assert
      Assert.IsNotNull(data);
      Assert.IsTrue(data.Length > 0);
    }

    [TestMethod]
    public void GetResourcesTests()
    {
      // Arrange
      var migrator = new LiteMigration(Assembly.GetExecutingAssembly(), _baseNamespace);

      // Act
      var items = migrator.Migrations.GetResources();

      // Assert
      System.Diagnostics.Debug.Print("======================");
      System.Diagnostics.Debug.Print("===[ GetResourcesTests");
      foreach (var item in items)
      {
        System.Diagnostics.Debug.Print("==> " + item);
      }

      Assert.IsTrue(items.Count > 0, $"Not migration scripts found in namespace, '{_baseNamespace}'");
    }

    [TestMethod]
    public void GetSortedMigrationsTest()
    {
      // Arrange
      long oldVer = 0;
      var migrator = new LiteMigration(Assembly.GetExecutingAssembly(), _baseNamespace);

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
    public void ValidateMigrationNamingConventions()
    {
      // Arrange
      long oldVer = 0;
      var migrator = new LiteMigration(Assembly.GetExecutingAssembly(), _baseNamespace);

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
}
