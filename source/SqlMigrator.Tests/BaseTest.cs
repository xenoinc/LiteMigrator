/* Copyright Xeno Innovations, Inc. 2019
 * Date:    2019-9-28
 * Author:  Damian Suess
 * File:    BaseTest.cs
 * Description:
 *  Base methods for test classes. This ensure we keep naming conventions
 */

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xeno.LiteMigrator.SystemTests
{
  [TestClass]
  public class BaseTest
  {
    private string _tempPath = string.Empty;

    public BaseTest()
    {
      //  Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
      var pth = Path.GetTempPath();
      TempDatabasePath = Path.Combine(pth, $"POS-TEST-{System.Guid.NewGuid()}.db3");
    }

    ~BaseTest()
    {
      DeleteDatabase();
    }

    /// <summary>Gets the location as an in-memory database path.</summary>
    /// <value>Location of temp database is in-memory.</value>
    public string InMemoryDatabasePath => ":memory:";

    /// <summary>Gets or sets location to temp database file.</summary>
    /// <value>Location to temp database file.</value>
    public string TempDatabasePath
    {
      get => _tempPath;
      set => _tempPath = value;
    }

    [TestInitialize]
    public virtual void CleanupBeforeTest()
    {
      // Initialize here
    }

    // NOTE: We can't override the base classes Init/Cleanup
    //       because a static can't be virtual or override
    //
    ////[ClassCleanup]
    ////public virtual void ClassCleanup()
    ////{
    ////  if (System.IO.File.Exists(TempDatabasePath))
    ////    System.IO.File.Delete(TempDatabasePath);
    ////}
    ////
    /////// <summary>Initialize test class.</summary>
    /////// <param name="context">Test context.</param>
    /////// <seealso cref="https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2010/ms245248(v=vs.100)?redirectedfrom=MSDN" />
    ////[ClassInitialize]
    ////public virtual void ClassInit(TestContext context)
    ////{
    ////  // randomly generate DB file
    ////  var pth = Path.GetTempPath();
    ////  TempDatabasePath = Path.Combine(pth, $"POS-TEST-{System.Guid.NewGuid()}.db3");
    ////}

    [TestCleanup]
    public virtual void CleanupPostTest()
    {
      // Cleanup here
    }

    public void DeleteDatabase()
    {
      if (System.IO.File.Exists(TempDatabasePath))
        System.IO.File.Delete(TempDatabasePath);
    }
  }
}
