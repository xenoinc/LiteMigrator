/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-15
 * Author:  Damian Suess
 * File:    LiteMigrator.cs
 * Description:
 *  Simple SQL Migration Engine
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SQLite;
using Xeno.LiteMigrator.DataObjects;
using Xeno.LiteMigrator.Engines;
using Xeno.LiteMigrator.Factory;
using Xeno.LiteMigrator.Versioning;

namespace Xeno.LiteMigrator
{
  /// <summary>LiteMigration core system.</summary>
  /// <remarks>
  ///  1. Rename class to LiteMigrator, this may conflict with namespace.
  ///  2. Refactor order of constructor properties (making all of them sequential).
  ///  3. Make disposable, exposing the SQLite DB object. - In-Memory tests will fail once connection is closed.
  /// </remarks>
  public partial class LiteMigration : IDisposable
  {
    private const string InMemoryDatabase = ":memory:";

    private SQLiteAsyncConnection _connection;
    private string _databasePath = string.Empty;
    private bool _disposed = false;
    private bool _isInitialized = false;
    private IEngine _sqlEngine;

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
    /// <param name="assm">Resource file with migration scripts.</param>
    /// <param name="baseNamespace">Assembly path to migration scripts.</param>
    public LiteMigration(Assembly assm, string baseNamespace)
      : this(InMemoryDatabase, baseNamespace, DatabaseType.SQLite)
    {
      Migrations.BaseAssemblyFile = assm.Location;
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
    /// <param name="assm">Resource file with migration scripts.</param>
    /// <param name="baseNamespace">Namespace to scripts.</param>
    public LiteMigration(string databasePath, Assembly assm, string baseNamespace)
      : this(databasePath, baseNamespace, DatabaseType.SQLite)
    {
      Migrations.BaseAssemblyFile = assm.Location;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="LiteMigration"/> class.
    /// </summary>
    /// <param name="databasePath">Path to the SQLite database.</param>
    /// <param name="baseNamespace">Namespace to scripts.</param>
    /// <param name="databaseType">Type of database connection.</param>
    /// <param name="baseAssembly">Migration's base assembly name.</param>
    public LiteMigration(string databasePath, string baseNamespace, DatabaseType databaseType, string baseAssembly = "")
    {
      ////RevisionTable = nameof(VersionInfo);  // FUTURE
      // Set to current namespace, it's a something
      DatabasePath = databasePath;
      DatabaseType = databaseType;

      // Create version info table here
      // Initialize().Wait();
      Migrations = new()
      {
        BaseAssemblyFile = baseAssembly,
        BaseNamespace = baseNamespace,
      };

      switch (databaseType)
      {
        case DatabaseType.SQLiteCipher:
          _sqlEngine = new Engines.SqlitePclEngine();

          // In-testing
          //// _sqlEngine = new Engines.SqlcipherEngine();
          break;

        case DatabaseType.SQLite:
        default:
          _sqlEngine = new Engines.SqlitePclEngine();
          break;
      }

      // Connect to database immediately
      if (!Connect(DatabasePath))
        System.Diagnostics.Debug.WriteLine("Failed to create DB");

      // The next operation may begin before this is finished being created
      VersionInitialize();

      //// Task.Run(async () => await VersionInitializeAsync().ConfigureAwait(false));

      _isInitialized = true;
    }

    public string BaseNamespace
    {
      get => Migrations.BaseNamespace;
      set => Migrations.BaseNamespace = value;
    }

    /// <summary>Gets the SQLite connection.</summary>
    public SQLiteAsyncConnection Connection => _connection;

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
        {
          // Should use Reinitialize to check for validations
          //// Task.Run(async () => await ReinitializeAsync().ConfigureAwait(false));
          //// ReinitializeAsync();

          VersionInitialize();
        }
      }
    }

    public DatabaseType DatabaseType { get; set; }

    /// <summary>Gets a value indicating whether is connected to the database.</summary>
    public bool IsConnected
    {
      get
      {
        if (_connection is null)
          return false;
        else
          return true;
      }
    }

    /// <summary>Gets the last known error.</summary>
    public string LastError { get; private set; }

    /// <summary>Gets the last known error.</summary>
    public Exception? LastException { get; private set; }

    public MigrationFactory Migrations { get; private set; }

    /// <summary>Connect to the database.</summary>
    /// <param name="databasePath">Path to the SQLite database or ":memory:".</param>
    /// <returns>True on successful connection.</returns>
    public bool Connect(string databasePath)
    {
      if (IsConnected)
      {
        // Disconnect so we can reconnect
      }

      try
      {
        _connection = new SQLiteAsyncConnection(DatabasePath);
        return true;
      }
      catch (Exception ex)
      {
        LastException = ex;
        LastError = ex.Message;
      }

      return false;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_disposed)
        return;

      // Close database connection.
      _connection?.CloseAsync();
    }

    //// FUTURE: Consider adding this feature one day
    //// <summary>Overrides the name of our version info table</summary>
    //// public string RevisionTable { get; set; }

    ////public VersionFactory Versions { get; private set; }
  }

  /// <summary>LiteMigration, migration scripts.</summary>
  public partial class LiteMigration
  {
    /*
    /// <summary>Executes a single migration script.</summary>
    /// <param name="migration">Migration script information.</param>
    /// <returns>True if executed without errors.</returns>
    [Obsolete("Not implemented.")]
    public async Task<bool> MigrateUpAsync(IMigration migration)
    {
      await Task.Yield();
      throw new NotImplementedException();
    }
    */

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
        //// SQLiteAsyncConnection db = new SQLiteAsyncConnection(DatabasePath);

        foreach (var mig in missing)
        {
          string path = mig.Value.Script;
          string migScript = Migrations.GetMigrationScriptByResource(path);
          var parser = new ParserFactory(migScript);

          // isOk = _sqlEngine.ExecuteMigration(mig, out errMsg);
          // if (!isOk) break;
          await _connection.RunInTransactionAsync(tran =>
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
                LastException = execEx;
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
              LastException = ex;
            }
          });

          if (hasError)
            break;
        }

        // _sqlEngine.Close()
        //// await _connection.CloseAsync();
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
  }

  /// <summary>Version Factory of LiteMigration.</summary>
  public partial class LiteMigration
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
        var list = await _connection.Table<VersionInfo>().ToListAsync();

        // TODO: In-memory DB will erase when closed
        //// await _connection.CloseAsync();

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
        var task = _connection.CreateTableAsync<VersionInfo>();

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
        await _connection.CreateTableAsync<VersionInfo>();

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
}
