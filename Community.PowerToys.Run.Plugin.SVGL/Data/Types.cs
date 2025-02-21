using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows.Input;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.SVGL.Data;

public class Svgl
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("title")] public string Title { get; init; }

    [JsonPropertyName("category")]
    [JsonConverter(typeof(CategoryBaseConverter))]
    public CategoryBase Category { get; init; }

    [JsonPropertyName("route")]
    [JsonConverter(typeof(ThemeBaseConverter))]
    public ThemeBase Route { get; init; }

    [JsonPropertyName("wordmark")]
    [JsonConverter(typeof(ThemeBaseConverter))]
    public ThemeBase Wordmark { get; init; }

    [JsonPropertyName("url")] public string Url { get; set; }
}

public abstract class CategoryBase
{
}

public class CategoryString(string category) : CategoryBase
{
    public string Category { get; set; } = category;

    public override string ToString()
    {
        return Category;
    }
}

public class CategoryArray(List<string> categories) : CategoryBase
{
    public List<string> Categories { get; set; } = categories;

    // public CategoryArray(List<string> categories)
    // {
    //     Categories = categories;
    // }

    public override string ToString()
    {
        return string.Join(", ", Categories);
    }
}

public class ThemeBase
{
}

public class ThemeString(string route) : ThemeBase
{
    public string Route { get; set; } = route;

    // public ThemeString(string route)
    // {
    //     Route = route;
    // }

    public override string ToString()
    {
        return Route;
    }
}

public class ThemeObject(SvgThemes route) : ThemeBase
{
    public SvgThemes Route { get; set; } = route;

    // public ThemeObject(SvgThemes route)
    // {
    //     Route = route;
    // }

    public override string ToString()
    {
        return $"Light: {Route.Light}, Dark: {Route.Dark}";
    }
}

public class SvgThemes
{
    [JsonPropertyName("light")] public string Light { get; set; }
    [JsonPropertyName("dark")] public string Dark { get; set; }
}

public class NavigateToBrowserData
{
    public string Identifier { get; set; }
    public string Search { get; set; }
}

public class GetContextMenuResult
{
    public string Title { get; set; }
    public string Glyph { get; set; }
    public Key AcceleratorKey { get; set; }
    public ModifierKeys AcceleratorModifiers { get; set; }
    public string CopyContent { get; set; }
    public Func<ActionContext, bool> CustomAction { get; set; }
}