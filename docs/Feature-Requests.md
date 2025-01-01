# LiteMigrator - Feature Considerations

Below is a list of features under consideration. Some are nice to haves, others may need some more thought. Overall, things must have a "need" before it is adopted.

Template:

```
| Detail  | Description |
|-|-|
| Status  | |
| Date    | yyyy-mm-dd |
| Details | |
| Proposed for | |
```

## Misc Features

* Search for scripts in file system
* Search filters `Extensions = [] {"*.sql" };`
* Fluent style migration script
* Set assembly as a property not a constructor argument
* Chain together assemblies `.ScriptsInAssembly(Assembly.GetExecutingAssembly()).ScriptsInAssembly(...)...`
* Custom name for Version Info table. Default today is, `VersionInfo`.
* Custom logger using Microsoft.Extensions.Logger

### Reorder the Constructors

| Detail  | Description |
|-|-|
| Status  | Testing |
| Date    | 2024-12-31 |
| Details | Constructing an object should be linear and not scattered when adding an argument. |
| Proposed for | v0.8

```cs
// Before (v0.6)
public LiteMigration()
public LiteMigration(string baseNamespace)
public LiteMigration(Assembly assm, string baseNamespace)
public LiteMigration(DatabaseType databaseType)
public LiteMigration(string databasePath, Assembly assm, string baseNamespace)
public LiteMigration(string databasePath, string baseNamespace, DatabaseType databaseType, string baseAssembly = "")

// Model A
public LiteMigration()
public LiteMigration(string baseNamespace)
public LiteMigration(string baseNamespace, Assembly assm)
public LiteMigration(string baseNamespace, Assembly assm, string databasePath)
public LiteMigration(string baseNamespace, Assembly? baseAssembly = null, string databasePath)

// Model B - Assume current assembly
public LiteMigration()  // Default, in-memory db
public LiteMigration(string dbPath)
public LiteMigration(string dbPath, string baseNamespace)
public LiteMigration(string dbPath, string baseNamespace, Assembly? scriptAssembly)
```

### Connect after Constructing

| Detail  | Description |
|-|-|
| Status  | n/a |
| Date    | 2024-12-31 |
| Details | Users may not want to auto-connect to the database. |

* `(..., bool autoConnect = true`)
* `async Task<bool> ConnectAsync();`

Use cases:

```cs
using (var migrator = new Migrator(...))
{
  bool isConnected = migrator.ConnectAsync();
  if (!isConnected)
  {
    _log.Error(migrator.LastError);
  }

  migrator.MigrateUp();
}
```

### Class Name Change

| Detail  | Description |
|-|-|
| Status  | In-progress |
| Date    | 2024-12-31 |
| Details | The project's name is 'LiteMigrator' and the main class is 'LiteMigration' which can be confusing. |

Rule of thumb, the main class shouldn't be named the same as the namespace. The main class' name should be seamlessly integrated integrated.

New name proposals instead of `LiteMigration`

* `Migrator`
* `Migrater`
* `Migration` - No

### Transactions

| Detail  | Description |
|-|-|
| Status  | n/a |
| Date    | 2024-12-31 |
| Details | Ability to optionally perform transactions, commit on success, and rollback in case of an error. |

```cs
using (var migrator = new Migrator(...))
{
  migrator.ConnectAsync();

  // By default, UseTransactions is
  migrator.UseTransactions = true;

  migrator.MigrateUp();

  // Enable on a per-migration basis
  migrator.MigrateUp(useTransaction: true);
}
```
