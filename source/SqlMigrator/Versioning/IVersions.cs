/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-28
 * Author:  Damian Suess
 * File:    IVersions.cs
 * Description:
 *  Version information interface
 */

using System.Collections.Generic;

namespace Xeno.LiteMigrator.Versioning
{
  public interface IVersions
  {
    void AddItem(long version);

    IEnumerable<long> AppliedMigrations();

    bool IsApplied(long version);
  }
}
