/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-8-30
 * Author:  Damian Suess
 * File:    RawLocalDatabaseTest.cs
 * Description:
 *  Raw local database test without
 */

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LiteMigrator.SystemTests.TestData;

namespace LiteMigrator.SystemTests.Specs.SqliteTests
{
  [TestClass]
  public class RawLocalDatabaseTest
  {
    private readonly string _dbPath = ":memory:";
    private SQLite.SQLiteAsyncConnection _db;

    public RawLocalDatabaseTest()
    {
      // string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
      // var db = new SQLiteConnection(dbPath);
    }

    [TestMethod]
    public async Task FileConnectionTestAsync()
    {
      var guid = System.Guid.NewGuid().ToString();
      string dbPath = $@"C:\temp\{guid}.db";

      if (System.IO.File.Exists(dbPath))
        System.IO.File.Delete(dbPath);

      _db = new SQLite.SQLiteAsyncConnection(dbPath);
      await _db.CreateTableAsync<DummyTable>().ConfigureAwait(false);

      bool exists = System.IO.File.Exists(dbPath);
      Assert.AreEqual(true, exists, "Make sure VS is in Admin mode");

      await _db.CloseAsync();
      if (File.Exists(dbPath))
        File.Delete(dbPath);

      if (File.Exists(dbPath))
        Assert.Fail($"File '{dbPath}' was not deleted");
    }

    [TestMethod("Create table in-memory test. Runs by itself, but not as a group")]
    public async Task MemoryCreateTestAsync()
    {
      // Assemble
      CreateConnection();

      // This table doesn't exist anyways
      var columnInfo = await _db.GetTableInfoAsync("DummyTable");

      // Act
      if (columnInfo is not null && columnInfo.Count == 0)
      {
        // Consider..  <..>(CreateFlags.AllImplicit).Wait();
        _db.CreateTableAsync<DummyTable>().Wait();
      }

      var item = new DummyTable
      {
        // Id = 999,
        IdGuid = "B7B18CA9-38B8-4BD9-B1ED-095FD2E1287B",
        Name = "Item-Test1",
        LastSyncDttm = default(System.DateTime),
      };

      var id = await _db.InsertAsync(item);

      // Assert
      Assert.AreNotEqual(0, id);

      var dummyItem = await GetItemAsync(id);
      Assert.AreEqual(dummyItem.IdGuid, item.IdGuid, $"Incorrect GUID for ItemId {id}");

      CloseConnection();
    }

    [TestMethod]
    public async Task MemoryEmptyTableTestAsync()
    {
      CreateConnection();

      _db.CreateTableAsync<DummyTable>().Wait();

      List<List<string>> items = await _db.QueryAsync<List<string>>("SELECT name FROM sqlite_master WHERE type='table' AND name='DummyTable';");
      Assert.IsNotNull(items);

      // Returns column names and info
      var columnInfo = await _db.GetTableInfoAsync("DummyTable");
      Assert.AreNotEqual(0, columnInfo.Count);

      CloseConnection();
    }

    ////[TestMethod]
    ////public async Task TableInsertTest()
    ////{
    ////  await SaveItemAsync(item);
    ////  var items = await _db.QueryAsync<DummyTable>("SELECT * FROM [DummyTable] WHERE [Done] = 0");
    ////  Assert.AreEqual(1, items.Count);
    ////}

    private void CloseConnection()
    {
      if (_db != null)
        _db.CloseAsync();
    }

    private void CreateConnection()
    {
      _db = new SQLite.SQLiteAsyncConnection(_dbPath);
    }

    private Task<int> DeleteItemAsync(DummyTable item)
    {
      return _db.DeleteAsync(item);
    }

    private Task<DummyTable> GetItemAsync(int id)
    {
      return _db.Table<DummyTable>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();
    }

    private Task<List<DummyTable>> GetItemsAsync()
    {
      return _db.Table<DummyTable>().ToListAsync();
    }

    private Task<List<DummyTable>> GetItemsNotDoneAsync()
    {
      return _db.QueryAsync<DummyTable>("SELECT * FROM [DummyTable] WHERE [Done] = 0");
    }

    private Task<int> SaveItemAsync(DummyTable item)
    {
      if (item.Id != 0)
      {
        return _db.UpdateAsync(item);
      }
      else
      {
        return _db.InsertAsync(item);
      }
    }
  }
}
