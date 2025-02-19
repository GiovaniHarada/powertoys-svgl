using System.Collections.Generic;
using Community.PowerToys.Run.Plugin.SVGL.UnitTests.Test_Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.SVGL.UnitTests;

[TestClass]
public class MainTests
{
    private Main _main = null!;
    public TestContext TestContext { get; set; }

    private IDelayedExecutionPlugin _delayedMain;

    [TestInitialize]
    public void TestInitialize()
    {
        _main = new Main();
        _delayedMain = _main;
    }

    [TestMethod]
    public void Query_should_display_all_svgs_on_initial_trigger()
    {
        const int initialResultCount = 15;

        var results = _main.Query(new Query(""));
        Assert.IsNotNull(results);
        Assert.AreEqual(initialResultCount, results.Count);
        Assert.IsInstanceOfType<Result>(results[0]);
    }

    [TestMethod]
    public void Query_should_display_svgs_matching_query()
    {
        TestContext.WriteLine("Starting Query test for 'Google'...");

        var expectedGoogleResult = QueryTestData.GetGoogleRelatedResults();
        var results = _delayedMain.Query(new Query("Google"), true);

        Assert.IsNotNull(results);
        Assert.AreEqual(expectedGoogleResult.Count, results.Count);
        for (var i = 0; i < expectedGoogleResult.Count; i++)
        {
            Assert.AreEqual(expectedGoogleResult[i].Title, results[i].Title, $"Mismatch at index {i} for Title");
            Assert.AreEqual(expectedGoogleResult[i].SubTitle, results[i].SubTitle,
                $"Mismatch at index {i} for SubTitle");
        }

        Assert.IsInstanceOfType<Result>(results[0]);

        // Testing same query but with different/weird casing
        var weirdCasingResults = _delayedMain.Query(new Query("GOoGlE"), true);

        Assert.IsNotNull(weirdCasingResults);
        Assert.AreEqual(expectedGoogleResult.Count, weirdCasingResults.Count);
        for (var i = 0; i < expectedGoogleResult.Count; i++)
        {
            Assert.AreEqual(expectedGoogleResult[i].Title, weirdCasingResults[i].Title,
                $"Mismatch at index {i} for Title");
            Assert.AreEqual(expectedGoogleResult[i].SubTitle, weirdCasingResults[i].SubTitle,
                $"Mismatch at index {i} for SubTitle");
        }

        Assert.IsInstanceOfType<Result>(weirdCasingResults[0]);
        TestContext.WriteLine("Query test for 'Google' Successfully Passed!!✅");


        // Testing with different query.
        TestContext.WriteLine("Starting Query test for 'flare'📢...");

        var expectedFlareResult = QueryTestData.GetCloudflareResults();
        var flareResults = _delayedMain.Query(new Query("flare"), true);

        Assert.IsNotNull(flareResults);
        for (var i = 0; i < expectedFlareResult.Count; i++)
        {
            Assert.AreEqual(expectedFlareResult[i].Title, flareResults[i].Title, $"Mismatch at index {i} for Title");
            Assert.AreEqual(expectedFlareResult[i].SubTitle, flareResults[i].SubTitle,
                $"Mismatch at index {i} for SubTitle");
        }

        Assert.IsInstanceOfType<Result>(flareResults[0]);
        TestContext.WriteLine("Query test for 'flare' Successfully Passed!!✅");
    }


    [TestMethod]
    public void Query_no_result_found()
    {
        // Testing No Result Found.
        TestContext.WriteLine("🔃 Starting No Result Found test...");

        const string query = "lsakdjf";
        var expectedNoResultFound = QueryTestData.GetNoResultsFoundData(query);
        var noResultFound = _delayedMain.Query(new Query(query), true);

        Assert.IsNotNull(expectedNoResultFound);
        Assert.IsNotNull(noResultFound);
        for (var i = 0; i < noResultFound.Count; i++) TestContext.WriteLine($"Result {i}: {noResultFound[i].Title}");
        //Assert.AreEqual(expectedResult.Count, results.Count);
        for (var i = 0; i < expectedNoResultFound.Count; i++)
        {
            Assert.AreEqual(expectedNoResultFound[i].Title, noResultFound[i].Title, $"Mismatch at index {i} for Title");
            Assert.AreEqual(expectedNoResultFound[i].SubTitle, noResultFound[i].SubTitle,
                $"Mismatch at index {i} for SubTitle");
        }

        TestContext.WriteLine("✅ No Result Found test Successfully Passed");
    }

    [TestMethod]
    public void LoadContextMenus_should_display_all_menu_options()
    {
        TestContext.WriteLine($"🔃 Starting Checking Context Menu for all SVG Variants test...");

        var expectedResult = QueryTestData.GetFullContextMenu();
        var result = _main.LoadContextMenus(new Result
        {
            ContextData = QueryTestData.GetFullContextData()
            // CreateBasicSvgl(2, "Vercel",
            // new CategoryArray(["Hosting", "Development"]), new ThemeObject(new SvgThemes
            // {
            //     Light = "vercel.svg",
            //     Dark = "vercel-dark.svg"
            // }), "https://vercel.com",
            // new ThemeObject(new SvgThemes
            // {
            //     Light = "vercel-wordmark.svg",
            //     Dark = "vercel-wordmark-dark.svg"
            // }))
        });

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResult.Count, result.Count);
        for (var i = 0; i < expectedResult.Count; i++)
        {
            Assert.AreEqual(expectedResult[i].Title, result[i].Title, $"Mismatch at index {i} for Title");
            Assert.AreEqual(expectedResult[i].Glyph, result[i].Glyph, $"Mismatch at index {i} for Glyph");
            Assert.AreEqual(expectedResult[i].AcceleratorKey, result[i].AcceleratorKey,
                $"Mismatch at index {i} for AcceleratorKey");
            Assert.AreEqual(expectedResult[i].AcceleratorModifiers, result[i].AcceleratorModifiers,
                $"Mismatch at index {i} for AcceleratorModifiers");
        }

        Assert.IsInstanceOfType<ContextMenuResult>(result[0]);

        TestContext.WriteLine("✅ Display All Menu Options test Successfully Passed");
    }

    [TestMethod]
    public void LoadContextMenu_should_display_default_menu()
    {
        TestContext.WriteLine($"🔃 Starting Test for default Context Menu SVG test...");

        var expectedResult = QueryTestData.GetDefaultContextMenu();
        var result = _main.LoadContextMenus(new Result()
        {
            ContextData = QueryTestData.GetDefaultContextData()
            // CreateBasicSvgl(5, "Google Drive", new CategoryString("Google"),
            // new ThemeString("google-drive.svg"), "https://www.google.com"
            // )
        });

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResult.Count, result.Count);
        for (var i = 0; i < expectedResult.Count; i++)
        {
            Assert.AreEqual(expectedResult[i].Title, result[i].Title, $"Mismatch at index {i} for Title");
            Assert.AreEqual(expectedResult[i].Glyph, result[i].Glyph, $"Mismatch at index {i} for Glyph");
            Assert.AreEqual(expectedResult[i].AcceleratorKey, result[i].AcceleratorKey,
                $"Mismatch at index {i} for AcceleratorKey");
        }

        Assert.IsInstanceOfType<List<ContextMenuResult>>(result);

        TestContext.WriteLine("✅ Display Default Menu test Successfully Passed");
    }

    [TestMethod]
    public void LoadContextMenu_should_display_themed_menu()
    {
        TestContext.WriteLine($"🔃 Starting Test for displaying Themed Context Menu SVG test...");

        var expectedResult = QueryTestData.GetThemedContextMenu();
        var result = _main.LoadContextMenus(new Result
        {
            ContextData = QueryTestData.GetThemedContextData()
            // CreateBasicSvgl(2, "Clerk",
            // new CategoryArray(["Software", "Authentication"]), new ThemeObject(new SvgThemes
            // {
            //     Light = "clerk.svg",
            //     Dark = "clerk-dark.svg"
            // }),
            // "https://clerk.com")
        });

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResult.Count, result.Count);

        for (var i = 0; i < expectedResult.Count; i++)
        {
            Assert.AreEqual(expectedResult[i].Title, result[i].Title, $"Mismatch at index {i} for Title");
            Assert.AreEqual(expectedResult[i].Glyph, result[i].Glyph, $"Mismatch at index {i} for Glyph");
            Assert.AreEqual(expectedResult[i].AcceleratorKey, result[i].AcceleratorKey,
                $"Mismatch at index {i} for AcceleratorKey");
            Assert.AreEqual(expectedResult[i].AcceleratorModifiers, result[i].AcceleratorModifiers,
                $"Mismatch at index {i} for AcceleratorModifiers");
        }

        Assert.IsInstanceOfType<List<ContextMenuResult>>(result);

        TestContext.WriteLine("✅ Display Themed Context Menu test Successfully Passed");
    }

    [TestMethod]
    public void LoadContextMenu_should_three_menu_with_one_icon_two_themed_wordmark()
    {
        TestContext.WriteLine(
            $"🔃 Starting Test for displaying One Icon & Two Themed Wordmark Context Menu SVG test...");

        var expectedResult = QueryTestData.GetThemedWordmarkContextMenu();
        var result = _main.LoadContextMenus(new Result
        {
            ContextData = QueryTestData.GetThemedWordmarkContextData()
            // CreateBasicSvgl(53, "Trpc", new CategoryString("Framework"),
            // new ThemeString("trpc.svg"), "https://trpc.com", new ThemeObject(new SvgThemes
            // {
            //     Light = "trpc_wordmark_light.svg",
            //     Dark = "trpc_wordmark_dark.svg"
            // }))
        });


        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResult.Count, result.Count);

        for (var i = 0; i < expectedResult.Count; i++)
        {
            Assert.AreEqual(expectedResult[i].Title, result[i].Title, $"Mismatch at index {i} for Title");
            Assert.AreEqual(expectedResult[i].Glyph, result[i].Glyph, $"Mismatch at index {i} for Glyph");
            Assert.AreEqual(expectedResult[i].AcceleratorKey, result[i].AcceleratorKey,
                $"Mismatch at index {i} for AcceleratorKey");
            Assert.AreEqual(expectedResult[i].AcceleratorModifiers, result[i].AcceleratorModifiers,
                $"Mismatch at index {i} for AcceleratorModifiers");
        }

        Assert.IsInstanceOfType<List<ContextMenuResult>>(result);

        TestContext.WriteLine("✅ Display One Icon & Two Themed Wordmark Context Menu test Successfully Passed");
    }
}