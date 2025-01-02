/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-15
 * Author:  Damian Suess
 * File:    Migrator.Version.cs
 * Description:
 *  Simple SQL Migration Versioning Engine
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteMigrator.DataObjects;
using LiteMigrator.Versioning;
using SQLite;

namespace LiteMigrator;

/// <summary>Version Factory of LiteMigration.</summary>
public partial class Migrator
{
  /// <summary>Gets or sets the list of applied migrations.</summary>
  /// <value>The list of applied migrations.</value>
  public Versions Versions { get; set; }

  /////// <summary>
  /////// Bulk register scripts into VersionInfo table
  /////// </summary>
  /////// <param name="migration">Migration script.</param>
  /////// <returns>Task.</returns>
  ////public async Task AddVersionsAsync(List<Migration> migrations)
  ////{
  ////
  ////}

  public void AddVersion(long version)
  {
    if (Versions.IsApplied(version))
      return;

    Versions.AddItem(version);
  }

  public void AddVersion(IMigration migration)
  {
    if (Versions.IsApplied(migration.VersionNumber))
    {
      // TODO: throw new VersionPreviouslyExistsException();
      return;
    }

    // Don't think now is the right time to add the script
    ////migration.AppliedDttm = System.DateTime.Now;
    ////SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);
    ////await db.InsertAsync(migration);
    ////await db.CloseAsync();

    Versions.AddItem(migration.VersionNumber);
  }

  /// <summary>
  ///   Read from database the list of installed migration scripts.
  /// </summary>
  /// <returns>Sorted list of migration scripts.</returns>
  public async Task<SortedDictionary<long, IVersionInfo>> GetInstalledMigrationsAsync()
  {
    var sorted = new SortedDictionary<long, IVersionInfo>();

    try
    {
      //// SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);
      var list = await Connection.Table<VersionInfo>().ToListAsync();

      // TODO: In-memory DB will erase when closed
      //// await Connection.CloseAsync();

      foreach (VersionInfo item in list)
      {
        sorted.Add(item.VersionNumber, item);

        Versions.AddItem(item.VersionNumber);
      }
    }
    catch (SQLiteException ex)
    {
      // Causes of failure:
      //  1) VersionInfo table doesn't exist. This can happen when using the ":memory:" database
      //  2) Database is locked - command already in progress
      System.Diagnostics.Debug.WriteLine("[Error] [GetInstalledMigrationsAsync] " + ex.Message);

      LastError = ex.Message;

      // Consider: Just in-case, lets make sure we have a DB
      // await VersionInitializeAsync();
    }

    return sorted;
  }

  /// <summary>Get list of migrations which have not been applied to our database yet.</summary>
  /// <returns>Sorted dictionary of non-applied migrations.</returns>
  public async Task<SortedDictionary<long, IMigration>> GetMissingMigrationsAsync()
  {
    var deltaList = new SortedDictionary<long, IMigration>();
    var installed = await GetInstalledMigrationsAsync();
    var available = Migrations.GetSortedMigrations();

    foreach (var item in available)
    {
      if (!installed.ContainsKey(item.Key))
        deltaList.Add(item.Key, item.Value);
    }

    return deltaList;
  }

  /// <summary>Registers migration script.</summary>
  /// <param name="db">Database connection.</param>
  /// <param name="migration">Migration information.</param>
  /// <returns>Task.</returns>
  public async Task RegisterVersionAsync(SQLiteAsyncConnection db, IMigration migration)
  {
    migration.AppliedDttm = System.DateTime.Now;

    var verInfo = new VersionInfo()
    {
      AppliedDttm = DateTime.Now,
      Description = migration.Description,
      VersionNumber = migration.VersionNumber,
    };

    await db.InsertAsync(verInfo);

    // Cache up what was saved
    AddVersion(migration);
  }

  public CreateTableResult? VersionInitialize()
  {
    try
    {
      Versions = new Versions();
      var task = Connection.CreateTableAsync<VersionInfo>();

      // Make sure the table is created before moving along
      task.Wait();
      var result = task.Result;

      return result;

      /*
      SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);
      db.CreateTableAsync<VersionInfo>();
      db.CloseAsync();
      */
    }
    catch (Exception ex)
    {
      LastException = ex;
      System.Diagnostics.Debug.WriteLine("[Error] [VersionInitialize] " + ex.Message);

      return null;
    }
  }

  public async Task VersionInitializeAsync()
  {
    try
    {
      Versions = new Versions();
      await Connection.CreateTableAsync<VersionInfo>();

      /*
      SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);
      await db.CreateTableAsync<VersionInfo>();
      await db.CloseAsync();
      */
    }
    catch (Exception ex)
    {
      LastException = ex;
      System.Diagnostics.Debug.WriteLine("[Error] [VersionInitialize] " + ex.Message);
    }
  }
}
