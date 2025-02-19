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

        // TestLogHelper.WriteEndTest($"{context} Results Validation");
    }

    [TestMethod]
    [TestCategory("Query")]
    public void Query_WithValidSearchTerm_ShouldReturnMatchingResult()
    {
        // TestContext.WriteLine("Starting Query test for 'Google'...");

        var expectedGoogleResult = QueryTestData.GetGoogleRelatedResults();
        var actualGoogleResult = _delayedMain.Query(new Query("Google"), true);

        AssertQueryResult(expectedGoogleResult, actualGoogleResult, "Google");

        // Assert.IsNotNull(actualGoogleResult);
        // Assert.AreEqual(expectedGoogleResult.Count, actualGoogleResult.Count);
        // for (var i = 0; i < expectedGoogleResult.Count; i++)
        // {
        //     Assert.AreEqual(expectedGoogleResult[i].Title, actualGoogleResult[i].Title,
        //         $"Mismatch at index {i} for Title");
        //     Assert.AreEqual(expectedGoogleResult[i].SubTitle, actualGoogleResult[i].SubTitle,
        //         $"Mismatch at index {i} for SubTitle");
        // }
        //
        // Assert.IsInstanceOfType<List<Result>>(actualGoogleResult);

        // Testing same query but with different/weird casing
        var caseInsensitiveResults = _delayedMain.Query(new Query("GOoGlE"), true);

        AssertQueryResult(expectedGoogleResult, caseInsensitiveResults, "Case Insensitive Google");
        // Assert.IsNotNull(caseInsensitiveResults );
        // Assert.AreEqual(expectedGoogleResult.Count, caseInsensitiveResults .Count);
        // for (var i = 0; i < expectedGoogleResult.Count; i++)
        // {
        //     Assert.AreEqual(expectedGoogleResult[i].Title, caseInsensitiveResults [i].Title,
        //         $"Mismatch at index {i} for Title");
        //     Assert.AreEqual(expectedGoogleResult[i].SubTitle, caseInsensitiveResults [i].SubTitle,
        //         $"Mismatch at index {i} for SubTitle");
        // }
        //
        // Assert.IsInstanceOfType<Result>(caseInsensitiveResults [0]);
        // TestContext.WriteLine("Query test for 'Google' Successfully Passed!!✅");


        // Testing with different query.
        // TestContext.WriteLine("Starting Query test for 'flare'📢...");

        var expectedCloudflareResults = QueryTestData.GetCloudflareResults();
        var actualCloudflareResults = _delayedMain.Query(new Query("flare"), true);

        AssertQueryResult(expectedCloudflareResults, actualCloudflareResults, "Cloudflare");
        // Assert.IsNotNull(actualCloudflareResults);
        // for (var i = 0; i < expectedCloudflareResults.Count; i++)
        // {
        //     Assert.AreEqual(expectedCloudflareResults[i].Title, actualCloudflareResults[i].Title, $"Mismatch at index {i} for Title");
        //     Assert.AreEqual(expectedCloudflareResults[i].SubTitle, actualCloudflareResults[i].SubTitle,
        //         $"Mismatch at index {i} for SubTitle");
        // }
        //
        // Assert.IsInstanceOfType<Result>(actualCloudflareResults[0]);
        // TestContext.WriteLine("Query test for 'flare' Successfully Passed!!✅");
    }


    [TestMethod]
    [TestCategory("Query")]
    public void Query_WithInvalidSearchTerm_ShouldReturnNoResultFound()
    {
        // Testing No Result Found.

        // TestContext.WriteLine("🔃 Starting No Result Found test...");

        const string invalidQuery = "lsakdjf";
        var expectedResults = QueryTestData.GetNoResultsFoundData(invalidQuery);
        var actualResults = _delayedMain.Query(new Query(invalidQuery), true);

        AssertQueryResult(expectedResults, actualResults, "No Result");
        // Assert.IsNotNull(noResultFound);
        // Assert.AreEqual(expectedNoResultFound.Count, noResultFound.Count);
        //
        // for (var i = 0; i < expectedNoResultFound.Count; i++)
        // {
        //     Assert.AreEqual(expectedNoResultFound[i].Title, noResultFound[i].Title, $"Mismatch at index {i} for Title");
        //     Assert.AreEqual(expectedNoResultFound[i].SubTitle, noResultFound[i].SubTitle,
        //         $"Mismatch at index {i} for SubTitle");
        // }

        // TestContext.WriteLine("✅ No Result Found test Successfully Passed");
    }

    private static void AssertContextMenuResults(List<ContextMenuResult> expected, List<ContextMenuResult> actual,
        string context)
    {
        // Logger
        // TestLogHelper.WriteStartTest($"Validating: {context}");


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

        // TestLogHelper.WriteEndTest($"{context} Validation");
    }

    [TestMethod]
    [TestCategory("Context Menu")]
    public void LoadContextMenus_WithFullContextData_ShouldReturnAllMenuOptions()
    {
        // TestContext.WriteLine($"🔃 Starting Checking Context Menu for all SVG Variants test...");

        var expectedResults = QueryTestData.GetFullContextMenu();
        var actualResults = _main.LoadContextMenus(new Result
        {
            ContextData = QueryTestData.GetFullContextData()
        });

        AssertContextMenuResults(expectedResults, actualResults, "Full Context Menu");
        // Assert.IsNotNull(result);
        // Assert.AreEqual(expectedResult.Count, result.Count);
        // for (var i = 0; i < expectedResult.Count; i++)
        // {
        //     Assert.AreEqual(expectedResult[i].Title, result[i].Title, $"Mismatch at index {i} for Title");
        //     Assert.AreEqual(expectedResult[i].Glyph, result[i].Glyph, $"Mismatch at index {i} for Glyph");
        //     Assert.AreEqual(expectedResult[i].AcceleratorKey, result[i].AcceleratorKey,
        //         $"Mismatch at index {i} for AcceleratorKey");
        //     Assert.AreEqual(expectedResult[i].AcceleratorModifiers, result[i].AcceleratorModifiers,
        //         $"Mismatch at index {i} for AcceleratorModifiers");
        // }
        //
        // Assert.IsInstanceOfType<ContextMenuResult>(result[0]);

        // TestContext.WriteLine("✅ Display All Menu Options test Successfully Passed");
    }

    [TestMethod]
    [TestCategory("Context Menu")]
    public void LoadContextMenu_WithDefaultContextData_ShouldReturnDefaultMenuOption()
    {
        // TestContext.WriteLine($"🔃 Starting Test for default Context Menu SVG test...");

        var expectedResults = QueryTestData.GetDefaultContextMenu();
        var actualResults = _main.LoadContextMenus(new Result()
        {
            ContextData = QueryTestData.GetDefaultContextData()
        });

        AssertContextMenuResults(expectedResults, actualResults, "Default Context Menu");
        // Assert.IsNotNull(actualResults);
        // Assert.AreEqual(expectedResults.Count, actualResults.Count);
        // for (var i = 0; i < expectedResults.Count; i++)
        // {
        //     Assert.AreEqual(expectedResults[i].Title, actualResults[i].Title, $"Mismatch at index {i} for Title");
        //     Assert.AreEqual(expectedResults[i].Glyph, actualResults[i].Glyph, $"Mismatch at index {i} for Glyph");
        //     Assert.AreEqual(expectedResults[i].AcceleratorKey, actualResults[i].AcceleratorKey,
        //         $"Mismatch at index {i} for AcceleratorKey");
        // }
        //
        // Assert.IsInstanceOfType<List<ContextMenuResult>>(actualResults);

        // TestContext.WriteLine("✅ Display Default Menu test Successfully Passed");
    }

    [TestMethod]
    [TestCategory("Context Menu")]
    public void LoadContextMenu_WithThemedContextData_ShouldReturnThemedMenuOptions()
    {
        // TestContext.WriteLine($"🔃 Starting Test for displaying Themed Context Menu SVG test...");

        var expectedResults = QueryTestData.GetThemedContextMenu();
        var actualResults = _main.LoadContextMenus(new Result
        {
            ContextData = QueryTestData.GetThemedContextData()
        });

        AssertContextMenuResults(expectedResults, actualResults, "Themed Context Menu");

        // Assert.IsNotNull(actualResults);
        // Assert.AreEqual(expectedResults.Count, actualResults.Count);
        //
        // for (var i = 0; i < expectedResults.Count; i++)
        // {
        //     Assert.AreEqual(expectedResults[i].Title, actualResults[i].Title, $"Mismatch at index {i} for Title");
        //     Assert.AreEqual(expectedResults[i].Glyph, actualResults[i].Glyph, $"Mismatch at index {i} for Glyph");
        //     Assert.AreEqual(expectedResults[i].AcceleratorKey, actualResults[i].AcceleratorKey,
        //         $"Mismatch at index {i} for AcceleratorKey");
        //     Assert.AreEqual(expectedResults[i].AcceleratorModifiers, actualResults[i].AcceleratorModifiers,
        //         $"Mismatch at index {i} for AcceleratorModifiers");
        // }
        //
        // Assert.IsInstanceOfType<List<ContextMenuResult>>(actualResults);

        // TestContext.WriteLine("✅ Display Themed Context Menu test Successfully Passed");
    }

    [TestMethod]
    [TestCategory("Context Menu")]
    public void LoadContextMenu_WithThemedWordmarkData_ShouldReturnCombinedMenuOptions()
    {
        // TestContext.WriteLine(
        //     $"🔃 Starting Test for displaying One Icon & Two Themed Wordmark Context Menu SVG test...");

        var expectedResults = QueryTestData.GetThemedWordmarkContextMenu();
        var actualResults = _main.LoadContextMenus(new Result
        {
            ContextData = QueryTestData.GetThemedWordmarkContextData()
        });

        AssertContextMenuResults(expectedResults, actualResults, "Themed Wordmark Context Menu");
        // Assert.IsNotNull(actualResults);
        // Assert.AreEqual(expectedResults.Count, actualResults.Count);
        //
        // for (var i = 0; i < expectedResults.Count; i++)
        // {
        //     Assert.AreEqual(expectedResults[i].Title, actualResults[i].Title, $"Mismatch at index {i} for Title");
        //     Assert.AreEqual(expectedResults[i].Glyph, actualResults[i].Glyph, $"Mismatch at index {i} for Glyph");
        //     Assert.AreEqual(expectedResults[i].AcceleratorKey, actualResults[i].AcceleratorKey,
        //         $"Mismatch at index {i} for AcceleratorKey");
        //     Assert.AreEqual(expectedResults[i].AcceleratorModifiers, actualResults[i].AcceleratorModifiers,
        //         $"Mismatch at index {i} for AcceleratorModifiers");
        // }
        //
        // Assert.IsInstanceOfType<List<ContextMenuResult>>(actualResults);

        // TestContext.WriteLine("✅ Display One Icon & Two Themed Wordmark Context Menu test Successfully Passed");
    }
}