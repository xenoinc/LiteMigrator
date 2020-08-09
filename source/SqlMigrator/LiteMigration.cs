/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-15
 * Author:  Damian Suess
 * File:    LiteMigrator.cs
 * Description:
 *  Simple SQL Migration Engine
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Xeno.LiteMigrator.DataObjects;
using Xeno.LiteMigrator.Engines;
using Xeno.LiteMigrator.Factory;
using Xeno.LiteMigrator.Versioning;

namespace Xeno.LiteMigrator
{
  public class LiteMigration
  {
    private const string InMemoryDatabase = ":memory:";
    private string _databasePath = string.Empty;
    private bool _isInitialized = false;
    private IEngine _sqlEngine;

    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="LiteMigration"/> class.
    ///   Assumes in-memory database, current namespace, and using generic SQLite.</summary>
    /// <param name="databaseType">Type of database.</param>
    public LiteMigration()
      : this(InMemoryDatabase, string.Empty, DatabaseType.SQLite)
    {
      // Set to current namespace, it's a something
      BaseNamespace = GetType().Namespace;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="LiteMigration"/> class.
    ///   Assumes in-memory database and using generic SQLite.
    /// </summary>
    /// <param name="baseNamespace">Namespace to scripts.</param>
    public LiteMigration(string baseNamespace)
      : this(InMemoryDatabase, baseNamespace, DatabaseType.SQLite)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="LiteMigration"/> class.
    ///   Assumes in-memory database and the current namespace contains the scripts.
    /// </summary>
    /// <param name="databaseType">Type of database connection.</param>
    public LiteMigration(DatabaseType databaseType)
      : this(InMemoryDatabase, string.Empty, databaseType)
    {
      // Set to current namespace, it's a something
      BaseNamespace = GetType().Namespace;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="LiteMigration"/> class.
    ///   Assumes using SQLite.
    /// </summary>
    /// <param name="databasePath">Path to the SQLite database.</param>
    /// <param name="baseNamespace">Namespace to scripts.</param>
    public LiteMigration(string databasePath, string baseNamespace)
      : this(databasePath, baseNamespace, DatabaseType.SQLite)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="LiteMigration"/> class.
    /// </summary>
    /// <param name="databasePath">Path to the SQLite database.</param>
    /// <param name="baseNamespace">Namespace to scripts.</param>
    /// <param name="databaseType">Type of database connection.</param>
    public LiteMigration(string databasePath, string baseNamespace, DatabaseType databaseType)
    {
      ////RevisionTable = nameof(VersionInfo);  // FUTURE
      // Set to current namespace, it's a something
      DatabasePath = databasePath;
      DatabaseType = databaseType;

      // Create version info table here
      // Initialize().Wait();
      Migrations = new MigrationFactory();
      Migrations.BaseNamespace = baseNamespace;

      switch (databaseType)
      {
        case DatabaseType.SQLiteCipher:
          _sqlEngine = new Engines.SqlitePclEngine();
          break;

        case DatabaseType.SQLite:
        default:
          _sqlEngine = new Engines.SqlitePclEngine();
          break;
      }

      VersionInitializeAsync().Wait();
      _isInitialized = true;
    }

    public string LastError { get; set; }

    #endregion Constructors

    public string BaseNamespace
    {
      get => Migrations.BaseNamespace;
      set
      {
        Migrations.BaseNamespace = value;
      }
    }

    /// <summary>Gets or sets the SQLite Database Path.</summary>
    /// <value>SQLite Database Path.</value>
    /// <remarks>
    ///   Changing DB path post-constructor will cause the VersionInfo
    ///   table to reinitialize. Thus, can slow things down.
    /// </remarks>
    public string DatabasePath
    {
      get => _databasePath;
      set
      {
        if (string.IsNullOrEmpty(_databasePath))
          _databasePath = InMemoryDatabase;

        if (_databasePath == value)
          return;

        _databasePath = value;
        //// Versions.DatabasePath = value;

        // We should reinitialize if our database path has changed
        if (_isInitialized)
          ReinitializeAsync().Wait();
      }
    }

    public DatabaseType DatabaseType { get; set; }

    public MigrationFactory Migrations { get; private set; }

    //// FUTURE: Consider adding this feature one day
    //// <summary>Overrides the name of our version info table</summary>
    //// public string RevisionTable { get; set; }

    ////public VersionFactory Versions { get; private set; }

    #region Migration Scripts

    public void CreateBaseFramework()
    {
      // Check if VersionInfo table exists
      // and create it if not
      throw new NotImplementedException();
    }

    /// <summary>Executes a single migration script.</summary>
    /// <param name="migration">Migration script information.</param>
    /// <returns>True if executed without errors.</returns>
    public async Task<bool> MigrateUpAsync(IMigration migration)
    {
      await Task.Yield();
      throw new NotImplementedException();
    }

    /// <summary>
    ///   Executes all missing migration scripts inside a database transaction
    ///   so it can rollback all upgrades upon failure.
    ///
    ///   OnSuccess: Remove backup / commit changes.
    ///   OnFailure: Rollback upon failure.
    /// </summary>
    /// <returns>True if executed without errors.</returns>
    public async Task<bool> MigrateUpAsync()
    {
      bool hasError = false;
      string errMsg = string.Empty;

      bool initOk = await ReinitializeAsync();

      if (!initOk)
      {
        hasError = true;
        errMsg = "Could not initialize migration VersionInfo table";
      }
      else
      {
        var allMigs = Migrations.GetSortedMigrations();
        var missing = await GetMissingMigrationsAsync();

        // _sqlEngine.Connect(DatabasePath);
        SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);

        foreach (var mig in missing)
        {
          string path = mig.Value.Script;
          string migScript = Migrations.GetMigrationScriptByResource(path);
          var parser = new ParserFactory(migScript);

          // isOk = _sqlEngine.ExecuteMigration(mig, out errMsg);
          // if (!isOk) break;
          await db.RunInTransactionAsync(tran =>
          {
            // Cannot begin a transaction while in one
            // tran.BeginTransaction();
            try
            {
              //// Only executes the 1st statement
              //// int x = tran.Execute(query);

              int cmdCount = 0;
              string lastQuery = string.Empty;
              string currentQuery = string.Empty;

              try
              {
                // Split commands by ';'
                var commands = parser.GetCommands(migScript);

                // Belongs in ParseFactory
                int beginCount = 0;
                string cmdCache = string.Empty;

                // Execute each command
                // TODO: Move into ParseFactory
                foreach (var query in commands)
                {
                  if (!parser.IsCommand(query))
                    continue;

                  parser.Concat(query);   // cmdCache += query;

                  if (parser.CurrentCommand.Contains("CREATE TRIGGER"))
                  {
                    // Make sure every BEGIN has an END before continuing !
                    if (query.Contains("BEGIN"))
                      beginCount++;

                    if (query.Contains("END;"))
                      beginCount--;

                    if (beginCount > 0)
                      continue;
                  }

                  ++cmdCount;

                  // Increment the execution line
                  parser.CountLines(query);
                  currentQuery = parser.CurrentCommand;   // currentQuery = cmdCache;

                  var cmd = tran.CreateCommand(currentQuery);
                  cmd.ExecuteNonQuery();
                  parser.ClearCommand();

                  lastQuery = currentQuery;
                }
              }
              catch (Exception execEx)
              {
                string err = $"Migration failed on Command number: '{cmdCount}'{Environment.NewLine}" +
                             $"* Exception: {execEx.Message}{Environment.NewLine}" +
                             $"* Line: {parser.Lines}{Environment.NewLine}" +
                             $"* Path: '{path}'{Environment.NewLine}" +
                             $"* Query: {currentQuery}{Environment.NewLine}" +
                             $"* Previous Query: {lastQuery}";

                System.Diagnostics.Debug.WriteLine($"[ERROR] [LiteMigration] [{err}]");
                throw new Exception(err);
              }

              var verInfo = new VersionInfo()
              {
                AppliedDttm = DateTime.Now,
                Description = mig.Value.Description,
                VersionNumber = mig.Value.VersionNumber,
              };

              tran.Insert(verInfo);

              AddVersion(mig.Value);

              tran.Commit();
            }
            catch (Exception ex)
            {
              hasError = true;
              errMsg = ex.Message;
              tran.Rollback();

              // TODO: Report which migration script & error message
              LastError = errMsg;
            }
          });

          if (hasError)
            break;
        }

        // _sqlEngine.Close()
        await db.CloseAsync();
      }

      if (hasError)
      {
        // Why did we get an error
        System.Diagnostics.Debug.WriteLine("LiteMigrator - ERROR - " + LastError);
      }

      return !hasError;
    }

    public async Task<bool> ReinitializeAsync()
    {
      // Migrations = new MigrationFactory();

      // Maybe use Lazy loading?
      ////Versions = new VersionFactory();
      ////Versions.Initialize();
      await VersionInitializeAsync();

      return true;
    }

    #endregion Migration Scripts

    #region Version Factory

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
    ///   Read from database the list of installed migration scripts
    /// </summary>
    /// <returns>Sorted list of migration scripts</returns>
    public async Task<SortedDictionary<long, IVersionInfo>> GetInstalledMigrationsAsync()
    {
      SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);
      var list = await db.Table<VersionInfo>().ToListAsync();
      await db.CloseAsync();

      var sorted = new SortedDictionary<long, IVersionInfo>();
      foreach (VersionInfo item in list)
      {
        sorted.Add(item.VersionNumber, item);

        Versions.AddItem(item.VersionNumber);
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

    public async Task VersionInitializeAsync()
    {
      try
      {
        Versions = new Versions();

        SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);
        await db.CreateTableAsync<VersionInfo>();
        await db.CloseAsync();
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine("ERROR: " + ex.Message);
      }
    }

    #endregion Version Factory
  }
}
