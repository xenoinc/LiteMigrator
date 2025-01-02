# LiteMigrator Change Log

## v0.9.0

Breaking Changes:

* [NEW] Renamed class `LiteMigration` to `Migrator`
* [FIX] MigratorFixture uses `BaseAssembly` object is used instead of `BaseAssemblyFile` (string)
  * This fixes loading migrations on Android
  * Fixes choosing alternate DLL containing migration scripts during AOT.
* [NEW] Calling Assembly is used by default if not provided in the constructor.

## v0.8.0

Constructor and namespace cleanup.

* [Update] Cleaned up constructors.
  * All in order now, removed unused parameters.
  * Flip-flopped 'BaseNamespace' and 'Assembly' order
  * Removed unused constructors
* [Removed] Dropped `DatabaseType` from constructor

## v0.6.x

_Various updates to support .NET MAUI from Xamarin.Forms_
