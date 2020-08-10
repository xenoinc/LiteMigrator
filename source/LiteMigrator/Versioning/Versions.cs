/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-28
 * Author:  Damian Suess
 * File:    Versions.cs
 * Description:
 *  Database version information
 */

using System.Collections.Generic;
using System.Linq;

namespace Xeno.LiteMigrator.Versioning
{
  public class Versions : IVersions
  {
    private IList<long> _versions = new List<long>();

    public void AddItem(long version)
    {
      _versions.Add(version);
    }

    public IEnumerable<long> AppliedMigrations()
    {
      return _versions.OrderByDescending(v => v).AsEnumerable();
    }

    public bool IsApplied(long version)
    {
      return _versions.Contains(version);
    }
  }
}
