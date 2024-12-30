/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-10-6
 * Author:  Damian Suess
 * File:    IEngine.cs
 * Description:
 *  LiteMigration Engine implementation
 */

using LiteMigrator.Versioning;

namespace LiteMigrator.Engines;

internal interface IEngine
{
  /// <summary>Gets error message, if any.</summary>
  /// <value>Error message, if any.</value>
  string LastErrorMessage { get; }

  /// <summary>Execute all missing migrations.</summary>
  /// <returns>pass/fail.</returns>
  bool MigrateUp();

  /// <summary>Migrate a specific migration.</summary>
  /// <param name="migration">Migration information.</param>
  /// <returns>pass/fail.</returns>
  bool MigrateUp(IMigration migration);

  /// <summary>Create VersionInfo table if needed.</summary>
  void VersionInitialize();
}
