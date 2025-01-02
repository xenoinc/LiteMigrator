/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-15
 * Author:  Damian Suess
 * File:    Migrator.cs
 * Description:
 *  Simple SQL Migration Engine base class and constructors
 */

using System;
using System.Reflection;
using LiteMigrator.Factory;
using SQLite;

namespace LiteMigrator;

/// <summary>LiteMigration core system.</summary>
/// <remarks>
///  1. Rename class to LiteMigrator, this may conflict with namespace.
///  2. Refactor order of constructor properties (making all of them sequential).
///  3. Make disposable, exposing the SQLite DB object. - In-Memory tests will fail once connection is closed.
/// </remarks>
public partial class Migrator : IDisposable
{
  private const string InMemoryDatabase = ":memory:";

  private SQLiteAsyncConnection _connection;
  private string _databasePath = string.Empty;
  private bool _disposed = false;
  private bool _isInitialized = false;

  /* TODO: Allow user to choose different engines (i.e. SQLite or SQLCypher)
  private IEngine _sqlEngine;
  */

  /// <summary>
  ///   Initializes a new instance of the <see cref="Migrator"/> class using an in-memory database.
  ///   Assumes the current namespace, and using generic SQLite.
  /// </summary>
  public Migrator()
    : this(InMemoryDatabase, string.Empty, null)
  {
    // Set to current namespace, it's a something
    // TODO (2025-01-01): Validate if we should use "" or GetType().Namespace
    // OLD: BaseNamespace = GetType().Namespace;
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="Migrator"/> class.
  ///   Assumes no namespace filter and executing assembly contains the scripts.
  /// </summary>
  /// <param name="databasePath">Path to database.</param>
  public Migrator(string databasePath)
    : this(databasePath, string.Empty, Assembly.GetCallingAssembly())
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="Migrator"/> class.
  ///   Assumes executing assembly contains the scripts.
  /// </summary>
  /// <param name="databasePath">Path to database.</param>
  /// <param name="baseNamespace">Assembly path to migration scripts.</param>
  public Migrator(string databasePath, string baseNamespace)
    : this(databasePath, baseNamespace, Assembly.GetCallingAssembly())
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="Migrator"/> class.
  /// </summary>
  /// <param name="databasePath">Path to the SQLite database.</param>
  /// <param name="baseNamespace">Namespace to scripts.</param>
  /// <param name="databaseType">Type of database connection.</param>
  /// <param name="baseAssembly">Migration's base assembly name.</param>
  public Migrator(string databasePath, string baseNamespace, Assembly baseAssembly = null)
  {
    ////RevisionTable = nameof(VersionInfo);  // FUTURE
    // Set to current namespace, it's a something
    DatabasePath = databasePath;

    Migrations = new()
    {
      BaseAssembly = baseAssembly,  // Consider using if null, Assembly.GetCallingAssembly()
      BaseNamespace = baseNamespace,
    };

    /*
    DatabaseType = databaseType;

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
    */

    // Connect to database immediately
    if (!Connect(DatabasePath))
      System.Diagnostics.Debug.WriteLine("Failed to create DB");

    // Create version info table here
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
  public bool IsConnected => Connection is not null;

  /// <summary>Gets the last known error.</summary>
  public string LastError { get; private set; }

  /// <summary>Gets the last known error.</summary>
  public Exception LastException { get; private set; }

  public MigrationFactory Migrations { get; private set; }

  /// <summary>Connect to the database.</summary>
  /// <param name="databasePath">Path to the SQLite database or ":memory:".</param>
  /// <returns>True on successful connection.</returns>
  public bool Connect(string databasePath)
  {
    if (IsConnected)
    {
      // Disconnect so we can reconnect
      _connection.CloseAsync().Wait();
    }

    try
    {
      // Use default flags: openFlags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex
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
    Connection?.CloseAsync();
  }

  //// FUTURE: Consider adding this feature one day
  //// <summary>Overrides the name of our version info table</summary>
  //// public string RevisionTable { get; set; }

  ////public VersionFactory Versions { get; private set; }
}
