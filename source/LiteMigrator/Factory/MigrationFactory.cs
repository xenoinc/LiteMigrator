/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-28
 * Author:  Damian Suess
 * File:    MigrationFactory.cs
 * Description:
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SQLitePCL;
using Xeno.LiteMigrator.Versioning;

namespace Xeno.LiteMigrator.Factory
{
  public class MigrationFactory
  {
    //// private string _baseAssemblyFile;

    public MigrationFactory()
    {
      BaseNamespace = GetType().Namespace;
      BaseAssemblyFile = string.Empty;
    }

    public MigrationFactory(string baseNamespace, string baseAssemblyFile = "")
    {
      BaseNamespace = baseNamespace;
      BaseAssemblyFile = baseAssemblyFile;
    }

    public string BaseAssemblyFile { get; set; }

    public Assembly BaseAssembly
    {
      get
      {
        var assm = Assembly.GetExecutingAssembly();

        try
        {
          if (!string.IsNullOrEmpty(BaseAssemblyFile))
            assm = Assembly.LoadFile(BaseAssemblyFile);
        }
        catch
        {
          System.Console.WriteLine($"Error loading assembly from file, '{BaseAssemblyFile}'");
        }

        return assm;
      }
    }

    public string BaseNamespace { get; set; }

    /// <summary>Get migration script data using name search.</summary>
    /// <param name="fileName">File name.</param>
    /// <param name="data">SQL script.</param>
    /// <returns>True if file was found.</returns>
    /// <example>
    ///   <![CDATA[isFound = GetMigrationScript("20190915000-BaseDDL.sql", out string sql);]]>
    /// </example>
    public bool GetMigrationScriptByName(string fileName, out string data)
    {
      data = GetMigrationScriptByResource($"{BaseNamespace}.{fileName}");
      return true;
    }

    /// <summary>Get migration script data using the full resource namespace path.</summary>
    /// <param name="resourcePath">Namespace path.</param>
    /// <returns>SQL data for script.</returns>
    public string GetMigrationScriptByResource(string resourcePath)
    {
      string result = string.Empty;
      try
      {
        var assembly = BaseAssembly; //// Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        using (StreamReader reader = new StreamReader(stream))
        {
          result = reader.ReadToEnd();
        }
      }
      catch (Exception ex)
      {
        // throw invalid namespace error?
        System.Diagnostics.Debug.WriteLine("[Error] [GetMigrationScriptByResource] " + ex.Message);
      }

      return result;
    }

    /// <summary>Get migration script data using revision number.</summary>
    /// <param name="revision">Revision Id.</param>
    /// <param name="data">SQL script.</param>
    /// <returns>True if file was found.</returns>
    /// <example>
    /// <code>
    ///   isFound = GetMigrationScript(20190915000, out string sql);
    /// </code>
    /// </example>
    public bool GetMigrationScriptByVersion(long revision, out string data)
    {
      data = string.Empty;
      bool found = false;
      string resName = string.Empty;
      string fileName = string.Empty;

      var assembly = BaseAssembly; //// Assembly.GetExecutingAssembly();
      var items = assembly.GetManifestResourceNames()
                          .Where(name => name.StartsWith(BaseNamespace));

      foreach (var item in items)
      {
        fileName = item.Replace($"{BaseNamespace}.", string.Empty);
        fileName = fileName.Replace($".sql", string.Empty);
        int ndx = fileName.IndexOf("-");

        long rev = Convert.ToInt64(fileName.Substring(0, ndx));

        if (revision == rev)
        {
          resName = item;
          found = true;
          break;
        }
      }

      if (found)
      {
        data = GetMigrationScriptByResource(resName);
        found = !string.IsNullOrEmpty(data);
      }

      return found;
    }

    /// <summary>Get the namespace of the resource ending with the specified name.</summary>
    /// <param name="containingTitle">Part of the file name.</param>
    /// <param name="trimNamespace">Remove 'BaseNamespace' from provided title.</param>
    /// <returns>Full resource name.</returns>
    public string GetResourceNamed(string containingTitle, bool trimNamespace = true)
    {
      var assembly = BaseAssembly; //// Assembly.GetExecutingAssembly();
      var item = assembly.GetManifestResourceNames()
                         .Where(name => name.StartsWith(BaseNamespace))
                         .Single(x => x.Contains(containingTitle));

      if (trimNamespace)
        return item.Replace($"{BaseNamespace}.", string.Empty);
      else
        return item;
    }

    public List<string> GetResources()
    {
      // All loaded assemblies
      //// var assm1 = AppDomain.CurrentDomain.GetAssemblies();

      // Old method:
      //// var assembly = Assembly.GetExecutingAssembly();

      var assembly = BaseAssembly;

      var items = assembly.GetManifestResourceNames()
                          .Where(name => name.StartsWith(BaseNamespace))
                          .ToList();
      items.Sort();

      return items;
    }

    /// <summary>Gets list of available migration scripts in namespace.</summary>
    /// <returns>Sorted dictionary of script namespace paths.</returns>
    /// <remarks>Rename to, GetMigrations().</remarks>
    public SortedDictionary<long, IMigration> GetSortedMigrations()
    {
      //// var assembly = BaseAssembly; //// Assembly.GetExecutingAssembly();
      var items = BaseAssembly
        .GetManifestResourceNames()
        .Where(name => name.StartsWith(BaseNamespace) && name.EndsWith(".sql"));

      // TODO: Need to check if error and report why
      // I.E.
      //  1. Incorrect namespace (first replace doesn't work)
      //  2. Couldn't get the index (from failed replace, or its missing)
      var dict = new SortedDictionary<long, IMigration>();
      foreach (var item in items)
      {
        string fileName = item.Replace($"{BaseNamespace}.", string.Empty);
        fileName = fileName.Replace($".sql", string.Empty);

        // If namespace contains a '-' we'll fail!
        int ndx = fileName.IndexOf("-");
        string baseName = fileName.Substring(0, ndx);

        long revision = Convert.ToInt64(baseName);
        string name = fileName.Substring(ndx + 1);

        dict.Add(revision, new Migration(revision, name, item));
      }

      return dict;
    }
  }
}
