/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-28
 * Author:  Damian Suess
 * File:    Migration.cs
 * Description:
 *  Physical migration file found in project
 */

using System;

namespace Xeno.LiteMigrator.Versioning
{
  public class Migration : IMigration
  {
    public Migration(long version, string description, string scriptPath = "")
    {
      VersionNumber = version;
      Description = description;
      Script = scriptPath;
    }

    public DateTime AppliedDttm { get; set; }

    public string Description { get; set; }

    /// <summary>Gets or sets the path to the script file.</summary>
    /// <value>The path to the script file.</value>
    public string Script { get; set; }

    public long VersionNumber { get; set; }
  }
}
