namespace Community.PowerToys.Run.Plugin.SVGL.UnitTests.Test_Data
{
    internal class TestContants
    {
        public static ThemeString expectedRouteString(string content)
        {
            return new ThemeString(content);
        }

        public static ThemeString expectedWordmarkString(string content)
        {
            return new ThemeString(content);
        }

        public static ThemeObject expectedRouteObject = new ThemeObject(new SVGThemes
        {
            Dark = "https://svgl.app/library/vercel_dark.svg",
            Light = "https://svgl.app/library/vercel.svg"
        });
        public static ThemeObject expectedWordmarkObject = new ThemeObject(new SVGThemes
        {
            Light = "https://svgl.app/library/vercel_wordmark.svg",
            Dark = "https://svgl.app/library/vercel_wordmark_dark.svg",
        });

        public static CategoryString expectedCategoryString(string category)
        {
            return new CategoryString(category);
        }

        public static CategoryArray expectedCategoryArray = new CategoryArray(["Hosting", "Vercel"]);

        public static string NoResultFoundMessage = "No SVG Found";
        public static string NoResultFoundSubTitle(string query)
        {
            return $"Could not found {query} SVG";
        }

        public const string RequestLogo = "Request Logo";
        public const string SubmitLogo = "Submit Logo";

    }
}
