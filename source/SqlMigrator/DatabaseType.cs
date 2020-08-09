/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-15
 * Author:  Damian Suess
 * File:    DatabaseType.cs
 * Description:
 *  DatabaseType
 */

namespace Xeno.LiteMigrator
{
  /// <summary>Database connection type.</summary>
  public enum DatabaseType
  {
    /// <summary>SQLite-Net-PCL</summary>
    SQLite,

    /// <summary>Encrypted SQLite database.</summary>
    SQLiteCipher,

    /////// <summary>Microsoft SQL Server.</summary>
    ////SQLServer,
    ////
    /////// <summary>MySQL.</summary>
    ////MySQL,
  }
}
