/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-3-17
 * Author:  Damian Suess
 * File:    DummyTable.cs
 * Description:
 *  Simple test table
 */

using System;
using SQLite;

namespace Xeno.LiteMigrator.SystemTests.TestData
{
  public class DummyTable
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string IdGuid { get; set; }

    public DateTime LastSyncDttm { get; set; }

    public string Name { get; set; }
  }
}
