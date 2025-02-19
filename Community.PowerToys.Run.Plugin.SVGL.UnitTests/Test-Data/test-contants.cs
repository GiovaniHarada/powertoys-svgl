namespace Community.PowerToys.Run.Plugin.SVGL.UnitTests.Test_Data;

internal abstract class TestConstants
{
    public static ThemeString ExpectedRouteString(string content)
    {
        return new ThemeString(content);
    }

    public static ThemeString ExpectedWordmarkString(string content)
    {
        return new ThemeString(content);
    }

    public static ThemeObject ExpectedRouteObject = new(new SvgThemes
    {
        Dark = "https://svgl.app/library/vercel_dark.svg",
        Light = "https://svgl.app/library/vercel.svg"
    });

    public static ThemeObject ExpectedWordmarkObject = new(new SvgThemes
    {
        Light = "https://svgl.app/library/vercel_wordmark.svg",
        Dark = "https://svgl.app/library/vercel_wordmark_dark.svg"
    });

    public static CategoryString ExpectedCategoryString(string category)
    {
        return new CategoryString(category);
    }

    public static CategoryArray ExpectedCategoryArray = new(["Hosting", "Vercel"]);

    public static string NoResultFoundMessage = "No SVG Found";

    public static string NoResultFoundSubTitle(string query)
    {
        return $"Could not found {query} SVG";
    }

    public const string RequestLogo = "Request Logo";
    public const string SubmitLogo = "Submit Logo";
}