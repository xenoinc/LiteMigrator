/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-28
 * Author:  Damian Suess
 * File:    VersionFactory.cs
 * Description:
 *  Maintains the database's version history
 *
 *  Disabled because it's a pain in the ass to sync
 *  the database path with our main LiteMigrator session in-project
 */

/*
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteMigrator.DataObjects;
using LiteMigrator.Versioning;
using SQLite;

namespace LiteMigrator.Factory
{
  public class VersionFactory
  {
    ////public string DatabasePath { get; set; }
    ////
    ////public Versions Versions { get; set; }
    ////
    ////public async Task AddVersion(long version)
    ////{
    ////  var versionInfo = new VersionInfo();
    ////  SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);
    ////  await db.InsertAsync(versionInfo);
    ////  await db.CloseAsync();
    ////
    ////  Versions.AddItem(version);
    ////}
    ////
    ////public async Task<SortedList<long, IMigration>> GetMigrations()
    ////{
    ////  SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);
    ////  // await db.CreateTableAsync<VersionInfo>();
    ////
    ////  var installed = db.Table<VersionInfo>().ToListAsync();
    ////
    ////  await db.CloseAsync();
    ////
    ////  return new SortedList<long, IMigration>();
    ////}
    ////
    ////public async Task Initialize()
    ////{
    ////  try
    ////  {
    ////    SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);
    ////    await db.CreateTableAsync<VersionInfo>();
    ////    await db.CloseAsync();
    ////  }
    ////  catch (Exception ex)
    ////  {
    ////    System.Diagnostics.Debug.WriteLine("ERROR: " + ex.Message);
    ////  }
    ////}
  }
}
*/
