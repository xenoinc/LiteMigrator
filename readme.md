# Lite Migrator for mobile devices

<img align="right" width="90" height="90" src="https://raw.githubusercontent.com/xenoinc/SQLiteMigrator/master/docs/logo.png">LiteMigrator is a tiny cross-platform SQLite migration framework for Xamarin (_.NET Standard_) inspired by [Fluent Migrator](https://github.com/fluentmigrator/fluentmigrator). This framework was built for use with Xamarin 🐒 projects, so it needs to be quick, simple and reliable when managing databases

[![](https://img.shields.io/nuget/v/Xeno.LiteMigrator?color=blue)](https://www.nuget.org/packages/Xeno.LiteMigrator/)
[![LiteMigrator Docs](https://img.shields.io/badge/docs-litemigrator-blue.svg)](https://github.com/xenoinc/LiteMigrator/wiki)

Sponsored by [Xeno Innovations](https://xenoinc.com), this project was made with nerd-love.

**_This project is currently in beta_**

Check out the sample project's source code [LiteMigrator.Sample](https://github.com/xenoinc/LiteMigrator.Sample)

## Use it in your project
Get [LiteMigrator](https://www.nuget.org/packages/Xeno.LiteMigrator) on NuGet today!

Currently, we recommend you add this to your project using Git's submodule so you always get the latest.

### Getting Started
Detailed instructions can be found on the [Using LiteMigrator](https://github.com/xenoinc/SQLiteMigrator/wiki/Using-LiteMigrator) wiki page.

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

## How to Contribute
Give it a test drive and support making LiteMigrator better :)

1. Fork on GitHub
2. Create a branch
3. Code (_and add tests)
4. Create a Pull Request (_PR_) on GitHub
   1. Target the ``develop`` branch and we'll get it merged up to ``master``
   2. Target the ``master`` branch for hotfixes
5. Get the PR merged
6. Welcome to our contributors' list!

## Known Limitations
Please visit the [Known Limitations](https://github.com/xenoinc/SQLiteMigrator/wiki/Known-Limitations) wiki page
