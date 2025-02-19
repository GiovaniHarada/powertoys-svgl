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

    public static CategoryString ExpectedCategoryString(string category)
    {
        return new CategoryString(category);
    }

    public const string NoResultFoundMessage = "No SVG Found";

    public static string NoResultFoundSubTitle(string query)
    {
        return $"Could not found {query} SVG";
    }

    public const string RequestLogo = "Request Logo";
    public const string SubmitLogo = "Submit Logo";
}