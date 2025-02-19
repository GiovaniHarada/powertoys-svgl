using System.Collections.Generic;
using Community.PowerToys.Run.Plugin.SVGL.UnitTests.Test_Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.SVGL.UnitTests;

[TestClass]
public class MainTests
{
    private Main _main = null!;
    private IDelayedExecutionPlugin _delayedMain;

    [TestInitialize]
    public void Setup()
    {
        _main = new Main();
        _delayedMain = _main;
    }

    [TestMethod]
    [TestCategory("Query")]
    public void InitialQuery_ShouldReturnAllSvgs()
    {
        const int initialResultCount = 15;

        var results = _main.Query(new Query(""));

        Assert.IsNotNull(results);
        Assert.AreEqual(initialResultCount, results.Count);
        Assert.IsInstanceOfType<Result>(results[0]);
    }


    [TestMethod]
    [TestCategory("Query")]
    public void Query_WithValidSearchTerm_ShouldReturnMatchingResult()
    {
        var expectedGoogleResult = QueryTestData.GetGoogleRelatedResults();
        var actualGoogleResult = _delayedMain.Query(new Query("Google"), true);

        AssertQueryResult(expectedGoogleResult, actualGoogleResult, "Google");

        var caseInsensitiveResults = _delayedMain.Query(new Query("GOoGlE"), true);

        AssertQueryResult(expectedGoogleResult, caseInsensitiveResults, "Case Insensitive Google");

        var expectedCloudflareResults = QueryTestData.GetCloudflareResults();
        var actualCloudflareResults = _delayedMain.Query(new Query("flare"), true);

        AssertQueryResult(expectedCloudflareResults, actualCloudflareResults, "Cloudflare");
    }

    [TestMethod]
    [TestCategory("Query")]
    public void Query_WithInvalidSearchTerm_ShouldReturnNoResultFound()
    {
        const string invalidQuery = "lsakdjf";
        var expectedResults = QueryTestData.GetNoResultsFoundData(invalidQuery);
        var actualResults = _delayedMain.Query(new Query(invalidQuery), true);

        AssertQueryResult(expectedResults, actualResults, "No Result");
    }

    [TestMethod]
    [TestCategory("Context Menu")]
    public void LoadContextMenus_WithFullContextData_ShouldReturnAllMenuOptions()
    {
        var expectedResults = QueryTestData.GetFullContextMenu();
        var actualResults = _main.LoadContextMenus(new Result
        {
            ContextData = QueryTestData.GetFullContextData()
        });

        AssertContextMenuResults(expectedResults, actualResults, "Full Context Menu");
    }

    [TestMethod]
    [TestCategory("Context Menu")]
    public void LoadContextMenu_WithDefaultContextData_ShouldReturnDefaultMenuOption()
    {
        var expectedResults = QueryTestData.GetDefaultContextMenu();
        var actualResults = _main.LoadContextMenus(new Result()
        {
            ContextData = QueryTestData.GetDefaultContextData()
        });

        AssertContextMenuResults(expectedResults, actualResults, "Default Context Menu");
    }

    [TestMethod]
    [TestCategory("Context Menu")]
    public void LoadContextMenu_WithThemedContextData_ShouldReturnThemedMenuOptions()
    {
        var expectedResults = QueryTestData.GetThemedContextMenu();
        var actualResults = _main.LoadContextMenus(new Result
        {
            ContextData = QueryTestData.GetThemedContextData()
        });

        AssertContextMenuResults(expectedResults, actualResults, "Themed Context Menu");
    }

    [TestMethod]
    [TestCategory("Context Menu")]
    public void LoadContextMenu_WithThemedWordmarkData_ShouldReturnCombinedMenuOptions()
    {
        var expectedResults = QueryTestData.GetThemedWordmarkContextMenu();
        var actualResults = _main.LoadContextMenus(new Result
        {
            ContextData = QueryTestData.GetThemedWordmarkContextData()
        });

        AssertContextMenuResults(expectedResults, actualResults, "Themed Wordmark Context Menu");
    }


    // Helper Functions
    private static void AssertContextMenuResults(List<ContextMenuResult> expected, List<ContextMenuResult> actual,
        string context)
    {
        Assert.IsNotNull(actual, $"{context}: Result should not be null");
        Assert.AreEqual(expected.Count, actual.Count, $"{context}: Result count mismatch");
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Title, actual[i].Title, $"{context}: Title mismatch");
            Assert.AreEqual(expected[i].Glyph, actual[i].Glyph, $"{context}: Glyph mismatch");
            Assert.AreEqual(expected[i].AcceleratorKey, actual[i].AcceleratorKey,
                $"{context}: Accelerator key mismatch");
            Assert.AreEqual(expected[i].AcceleratorModifiers, actual[i].AcceleratorModifiers,
                $"{context}: Accelerator modifiers mismatch");
        }

        Assert.IsInstanceOfType<List<ContextMenuResult>>(actual, $"{context}: Result type doesn't match");
    }

    private static void AssertQueryResult(List<Result> expected, List<Result> actual, string context)
    {
        Assert.IsNotNull(expected, $"{context}: Result should not be null");
        Assert.AreEqual(expected.Count, actual.Count, $"{context}: Result count mismatch");
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Title, actual[i].Title, $"{context}: Title mismatch");
            Assert.AreEqual(expected[i].SubTitle, actual[i].SubTitle, $"{context}: SubTitle mismatch");
        }

        Assert.IsInstanceOfType<List<Result>>(actual, $"{context}: Result type doesn't match");
    }
}