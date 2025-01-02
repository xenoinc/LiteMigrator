# LiteMigrator Change Log

## v0.9.0

Breaking Changes:

* [NEW] Renamed class `LiteMigration` to `Migrator`
* [FIX] BaseAssembly object is used instead of BaseAssemblyFile (string)
  * This fixes loading migrations on Android

## v0.8.0

Constructor and namespace cleanup.

* [Update] Cleaned up constructors.
  * All in order now, removed unused parameters.
  * Flip-flopped 'BaseNamespace' and 'Assembly' order
  * Removed unused constructors
* [Removed] Dropped `DatabaseType` from constructor
