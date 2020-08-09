# How to use LiteMigrator
This guide will step you through adding _LiteMigrator_ to your project from scratch and how to add additional migration scripts. Remember, your scripts are only executed one time. Once they're registered in your database, the system will never execute the script again.

1. Creating the migration controller
2. Trigger your migration scripts
3. Creating a migration script

We recommend you place your scripts in a subfolder below your **migration controller** class. In this example, we'll assume your scripts are placed in the subfolder named, ``Scripts``.

## Step 1 - Create the Migration Controller
First, you'll need a migration controller class that kicks off your migration scripts.

```cs
/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-15
 * Author:  Damian Suess
 * File:    LocalMigrations.cs
 * Description:
 *  Local helpers for LiteMigrator Engine
 */

using System;
using System.IO;
using System.Threading.Tasks;
using Xeno.LiteMigrator;

namespace Xeno.Pos.Client.Business.Migrations
{
  public class LocalMigrations
  {
    private const string DatabaseName = "MY_LOCAL_DATABASE.db3";
    private LiteMigration _liteMigrator;

    public LocalMigrations()
    {
      // Namespace location to our scripts
      var scriptLocation = GetType().Namespace + ".Scripts";

      // Path to database file
      var databasePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        DatabaseName);

      _liteMigrator = new LiteMigration(databasePath, scriptLocation, DatabaseType.SQLite);
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

## Step 2 - Trigger Migration Scripts

```cs
    private async Task LoadDatabaseMigrations()
    {
      var mig = new LocalMigrations();
      await mig.MigrateUp();
    }
```

## Step 3 - Creating a script
Scripts MUST follow the naming format of ``YYYYMMDDhhmm-<NameOfScript>.sql``. As an example, ``201909150000-BaseDDL.sql`` (_2019-09-15 12:00 AM_)

1. Add a new file to your project
2. Set file as an _**Embedded Resource**_
3. Done!
