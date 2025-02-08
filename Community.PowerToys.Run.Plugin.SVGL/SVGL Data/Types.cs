using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Community.PowerToys.Run.Plugin.SVGL;

public class SVGL
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("category")]
    [JsonConverter(typeof(CategoryBaseConverter))]
    public CategoryBase Category { get; set; }

    [JsonPropertyName("route")]
    [JsonConverter(typeof(ThemeBaseConverter))]
    public ThemeBase Route { get; set; }

    [JsonPropertyName("wordmark")]
    [JsonConverter(typeof(ThemeBaseConverter))]
    public ThemeBase Wordmark { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public abstract class CategoryBase
{ }

public class CategoryString : CategoryBase
{
    public string Category { get; set; }

    public CategoryString(string category)
    {
        Category = category;
    }

    public override string ToString() => Category;
}

public class CategoryArray : CategoryBase
{
    public List<string> Categories { get; set; }

    public CategoryArray(List<string> categories)
    {
        Categories = categories;
    }

    public override string ToString() => string.Join(", ", Categories);
}

public class ThemeBase { }

public class ThemeString : ThemeBase
{
    public string Route { get; set; }

    public ThemeString(string route)
    {
        Route = route;
    }

    public override string ToString() => Route;
}

public class ThemeObject : ThemeBase
{
    public SVGThemes Route { get; set; }

    public ThemeObject(SVGThemes route)
    {
        Route = route;
    }

    public override string ToString() => $"Light: {Route.Light}, Dark: {Route.Dark}";
}

public class SVGThemes
{
    [JsonPropertyName("light")]
    public string Light { get; set; }
    [JsonPropertyName("dark")]
    public string Dark { get; set; }
}

public class INavigateToBrowserData
{
    public string Identifier { get; set; }
    public string Search { get; set; }
}
