/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-8-29
 * Author:  Damian Suess
 * File:    Migration.cs
 * Description:
 *
 */

namespace SqlMigrator
{
  public class Migration
  {
    public int Engine { get; set; }

    public string HostName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string AssemblyRoot { get; set; }

    public void Update()
    {
    }
  }
}