/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-3-17
 * Author:  Damian Suess
 * File:    DummyTable.cs
 * Description:
 *  Simple test table
 */
using System;
using SQLite;

namespace LiteMigrator.SystemTests.TestData;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public class DummyTable
{
  [PrimaryKey]
  [AutoIncrement]
  public int Id { get; set; }

  public string IdGuid { get; set; }

  public DateTime LastSyncDttm { get; set; }

  public string Name { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
