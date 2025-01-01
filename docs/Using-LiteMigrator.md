# How to use LiteMigrator
This guide will step you through adding _LiteMigrator_ to your project from scratch and how to add additional migration scripts. Remember, your scripts are only executed one time. Once they're registered in your database, the system will never execute the script again.

Both **_basic_** and **_advanced_** sample provide the same basic features
1. Creating the migration controller
2. Trigger your migration scripts
3. Creating a migration script

We recommend you place your scripts in a subfolder below your **migration controller** class. In this example, we'll assume your scripts are placed in the subfolder named, ``Scripts``.

## Basic

1. Add **LiteMigrator** project to your solution
2. Create a folder in your solution to hold the scripts
3. Add SQL files as **Embedded Resources**
  * You must use the naming convention, "_YYYYMMDDhhmm-FileName.sql_"
4. Wire-up the controller

```cs
public async Task InstallMigrationsAsync()
{
  // Your EXE/DLL with the scripts
  var resourceAssm = Assembly.GetExecutingAssembly();
  var dbPath = @"C:\TEMP\MyDatabase.db3";
  var migsNamespace = "MyProjNamespace.Scripts";
  
  var liteMig = new LiteMigration(dbPath, resourceAssm, migsNamespace);
  bool = success = await liteMig.MigrateUpAsync();
}
```

## Advanced
### Step 1 - Create the Migration Controller
First, you'll need a migration controller class that kicks off your migration scripts.

```cs
using System;
using System.IO;
using System.Threading.Tasks;
using LiteMigrator;

namespace Xeno.Pos.Client.Business.Migrations
{
  public class LocalMigrations
  {
    private const string DatabaseName = "MY_LOCAL_DATABASE.db3";
    private LiteMigration _liteMigrator;

    public LocalMigrations()
    {
      // Current namespace location with the subfolder, Scripts
      var scriptLocation = GetType().Namespace + ".Scripts";

      // Path to database file
      var databasePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        DatabaseName);

      _liteMigrator = new LiteMigration(databasePath, scriptLocation, DatabaseType.SQLite, Assembly.GetExecutingAssembly().Location);
    }

    /// <summary>Execute all unexecuted migration scripts.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<bool> MigrateUp()
    {
      return await _liteMigrator.MigrateUpAsync();
    }
  }
}
```

### Step 2 - Trigger Migration Scripts

```cs
    private async Task LoadDatabaseMigrations()
    {
      var mig = new LocalMigrations();
      await mig.MigrateUp();
    }
```

### Step 3 - Creating a script
Scripts MUST follow the naming format of ``YYYYMMDDhhmm-<NameOfScript>.sql``. As an example, ``201909150000-BaseDDL.sql`` (_2019-09-15 12:00 AM_)

1. Add a new file to your project
2. Set file as an _**Embedded Resource**_
3. Done!

