/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-29
 * Author:  Damian Suess
 * File:    IMigration.cs
 * Description:
 *  Migration script file interface
 */

using System;

namespace Xeno.LiteMigrator.Versioning
{
  public interface IMigration
  {
    DateTime AppliedDttm { get; set; }

    string Description { get; set; }

    /// <summary>Gets or sets the script's namespace path.</summary>
    /// <value>Script namespace path.</value>
    string Script { get; set; }

    long VersionNumber { get; set; }
  }
}
