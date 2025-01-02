# SQLite Lite Migrator for cross-platform .NET

<!-- <img align="right" width="90" height="90" src="https://raw.githubusercontent.com/SuessLabs/LiteMigrator/master/docs/logo.png" /> -->
[![](https://raw.githubusercontent.com/SuessLabs/LiteMigrator/master/docs/logo.png)]()

LiteMigrator is a tiny cross-platform SQLite migration framework for .NET applications. This library was built for use with .NET MAUI 🐒, Avalonia, and Uno projects. So it needs to be quick, simple, and reliable when managing databases.

**LiteMigrator** takes a "_migrate-up_" approach. In most app deployments, we usually move forward and rarely downgrade (_consider app stores_). This helps keep the library small and nimble. Previously known as, _SQLiteMigrator_.

[![](https://img.shields.io/nuget/v/LiteMigrator?color=blue)](https://www.nuget.org/packages/LiteMigrator/)
[![LiteMigrator Docs](https://img.shields.io/badge/docs-litemigrator-blue.svg)](https://github.com/SuessLabs/LiteMigrator/wiki)

> Breaking Change from 0.6.x to now:
>
> Namespace changed from `Xeno.LiteMigrator` to `LiteMigrator`

Brought to you with nerd-love by [Suess Labs](https://suesslabs.com), sponsored by [Xeno Innovations](https://xenoinc.com).

## Supported Platforms

Check out the sample project's source code [LiteMigrator.Sample](https://github.com/SuessLabs/LiteMigrator.Sample)

| Platform | Status |
|----------|--------|
| Windows  | Yes
| Linux    | Yes
| Android  | Yes
| iOS      | Yes

Contribute today and get your platform supported 👍

## How to use it

Get [LiteMigrator](https://www.nuget.org/packages/LiteMigrator) on NuGet today!

## Getting Started
Detailed instructions can be found on the [Using LiteMigrator](https://github.com/SuessLabs/LiteMigrator/wiki/Using-LiteMigrator) wiki page.

1. Add **LiteMigrator** project to your solution
2. Create a folder in your solution to hold the scripts
3. Add SQL files as **Embedded Resources**
   * You must use the naming convention, "_YYYYMMDDhhmm-FileName.sql_"
4. Wire-up the controller


### Use Case 1

```cs
  var dbPath = Path.Combine(FileSystem.AppDataDirectory, "LiteMigrator.db3");
  var scriptNamespace = "MyProject.Namespace.Scripts";

  using var migrator = new Migrator(dbPath, scriptNamespace);

  // List of all embedded script files
  var embeddedMigrations = migrator.Migrations.GetSortedMigrations();

  // List of scripts installed
  var installed = await migrator.GetInstalledMigrationsAsync();

  // List of scripts waiting to be installed
  var notInstalled = await migrator.GetMissingMigrationsAsync();

  // Install migration scripts
  bool isSuccessful = await migrator.MigrateUpAsync();
```

### Use Case 2 - Class Constructor

```cs
public async Task InstallMigrationsAsync()
{
  // Your EXE/DLL with the scripts
  var dbPath = @"C:\TEMP\MyDatabase.db3";
  var migsNamespace = "MyProjNamespace.Scripts";

  var migrator = new Migrator(dbPath, migsNamespace);
  bool = success = await migrator.MigrateUpAsync();

  // Required after v0.6
  migrator.Dispose();
}
```

## How to Contribute

Give it a test drive and support making LiteMigrator better :)

1. Fork on GitHub
2. Create a branch
3. Code (_and add tests_)
4. Create a Pull Request (_PR_) on GitHub
   1. Target the ``develop`` branch and we'll get it merged up to ``master``
   2. Target the ``master`` branch for hotfixes
5. Get the PR merged
6. Welcome to our contributors' list!

This project could use your assistance to crush any limitations.

Please visit the [Known Limitations](https://github.com/SuessLabs/LiteMigrator/wiki/Known-Limitations) wiki page
