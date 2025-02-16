using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.SVGL.UnitTests
{
    [TestClass]
    public class MainTests
    {
        private Main _main = null!;
        public TestContext TestContext { get; set; }
        private IDelayedExecutionPlugin _delayedMain = null;

        [TestInitialize]
        public void TestInitialize()
        {
            _main = new Main();
            _delayedMain = (IDelayedExecutionPlugin)_main;
        }

        [TestMethod]
        public void Query_should_display_all_svgs_on_initial_trigger()
        {
            int initialResultCount = 15;

            var results = _main.Query(new(""));
            Assert.IsNotNull(results);
            Assert.AreEqual(initialResultCount, results.Count);
            Assert.IsInstanceOfType(results[0], typeof(Result));
        }

        [TestMethod]
        public void Query_should_display_svgs_matching_query()
        {
            TestContext.WriteLine("Starting Query test for 'Google'...");

            var expectedGoogleResult = QueryTestData.GoogleData();
            var results = _delayedMain.Query(new Query("Google"), true);

            Assert.IsNotNull(results);
            Assert.AreEqual(expectedGoogleResult.Count, results.Count);
            for (int i = 0; i < expectedGoogleResult.Count; i++)
            {
                Assert.AreEqual(expectedGoogleResult[i].Title, results[i].Title, $"Mismatch at index {i} for Title");
                Assert.AreEqual(expectedGoogleResult[i].SubTitle, results[i].SubTitle, $"Mismatch at index {i} for SubTitle");
            }
            Assert.IsInstanceOfType(results[0], typeof(Result));

            // Testing same query but with different/weird casing
            var weirdCasingResults = _delayedMain.Query(new Query("GOoGlE"), true);

            Assert.IsNotNull(weirdCasingResults);
            Assert.AreEqual(expectedGoogleResult.Count, weirdCasingResults.Count);
            for (int i = 0; i < expectedGoogleResult.Count; i++)
            {
                Assert.AreEqual(expectedGoogleResult[i].Title, weirdCasingResults[i].Title, $"Mismatch at index {i} for Title");
                Assert.AreEqual(expectedGoogleResult[i].SubTitle, weirdCasingResults[i].SubTitle, $"Mismatch at index {i} for SubTitle");
            }
            Assert.IsInstanceOfType(weirdCasingResults[0], typeof(Result));
            TestContext.WriteLine("Query test for 'Google' Successfully Passed!!✅");


            // Testing with different query.
            TestContext.WriteLine("Starting Query test for 'flare'📢...");

            var expectedFlareResult = QueryTestData.FlareData();
            var flareResults = _delayedMain.Query(new Query("flare"), true);

            Assert.IsNotNull(flareResults);
            for (int i = 0; i < expectedFlareResult.Count; i++)
            {
                Assert.AreEqual(expectedFlareResult[i].Title, flareResults[i].Title, $"Mismatch at index {i} for Title");
                Assert.AreEqual(expectedFlareResult[i].SubTitle, flareResults[i].SubTitle, $"Mismatch at index {i} for SubTitle");
            }
            Assert.IsInstanceOfType(flareResults[0], typeof(Result));
            TestContext.WriteLine("Query test for 'flare' Successfully Passed!!✅");
        }


        [TestMethod]
        public void Query_no_result_found()
        {
            // Testing No Result Found.
            TestContext.WriteLine("🔃 Starting No Result Found test...");

            string query = "lsakdjf";
            var expectedNoResultFound = QueryTestData.NoResultFoundData(query);
            var noResultFound = _delayedMain.Query(new Query(query), true);

            Assert.IsNotNull(expectedNoResultFound);
            Assert.IsNotNull(noResultFound);
            for (int i = 0; i < noResultFound.Count; i++)
            {
                TestContext.WriteLine($"Result {i}: {noResultFound[i].Title}");
            }
            //Assert.AreEqual(expectedResult.Count, results.Count);
            for (int i = 0; i < expectedNoResultFound.Count; i++)
            {
                Assert.AreEqual(expectedNoResultFound[i].Title, noResultFound[i].Title, $"Mismatch at index {i} for Title");
                Assert.AreEqual(expectedNoResultFound[i].SubTitle, noResultFound[i].SubTitle, $"Mismatch at index {i} for SubTitle");
            }
            TestContext.WriteLine("✅ No Result Found test Successfully Passed");

        }
    }
}




