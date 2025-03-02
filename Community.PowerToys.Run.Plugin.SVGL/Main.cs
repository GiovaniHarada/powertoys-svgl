using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using Community.PowerToys.Run.Plugin.SVGL.Data;
using Community.PowerToys.Run.Plugin.SVGL.Utils;
using LazyCache;
using ManagedCommon;
using Microsoft.Extensions.Caching.Memory;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.SVGL;
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
#pragma warning disable CsWinRT1028
public class Main : IPlugin, IContextMenu, IDisposable, IDelayedExecutionPlugin
#pragma warning restore CsWinRT1028
{
    // ReSharper disable once InconsistentNaming
    public static string PluginID => "6401d691-f104-4a1b-9558-087abe8b84a2";
    public string Name => "SVGL";
    public string Description => "Search and copy tech brand logos";
    private PluginInitContext Context { get; set; }
    private string IconPath { get; set; }
    private bool Disposed { get; set; }

    private readonly IMyApiClient _apiClient = new MyApiClients();

    // Caching variables
    private const string DefaultCacheKey = "SVGL_AllResults";
    private readonly CachingService _cache = new();

    private static readonly MemoryCacheEntryOptions CachingOption = new()
    {
        Priority = CacheItemPriority.NeverRemove
    };

    public List<Result> Query(Query query)
    {
        Log.Info($"SVGL Query: Search='{query.Search}'", GetType());
        var results = new List<Result>();

        if (!string.IsNullOrWhiteSpace(query.Search)) return results;

        try
        {
            var cachedResults = _cache.GetOrAdd(DefaultCacheKey, FetchDefaultResults, CachingOption);

            if (cachedResults.Any(r => r.Title == Constants.NoInternet) && Utils.Utils.IsInternetAvailable())
            {
                _cache.Remove(DefaultCacheKey);
                Log.Info("Internet Connection Restored. Cache invalidated", GetType());
                cachedResults = _cache.GetOrAdd(DefaultCacheKey, FetchDefaultResults, CachingOption);
                Log.Info("Cache invalidated successfully: ", GetType());
            }


            if (cachedResults.Any(r => r.Title == Constants.NoInternet))
            {
                results.AddRange(cachedResults.Select(result => new Result
                {
                    Title = result.Title,
                    SubTitle = result.SubTitle,
                    IcoPath = result.IcoPath,
                    Score = result.Score,
                    ContextData = result.ContextData
                }));

                return results;
            }

            var slicedResults = cachedResults[..15];
            results.AddRange(slicedResults.Select(result => new Result
            {
                Title = result.Title,
                SubTitle = result.SubTitle,
                IcoPath = result.IcoPath,
                Score = result.Score,
                ContextData = result.ContextData
            }));


            return results;
        }
        catch (Exception ex)
        {
            results.Add(new Result
            {
                Title = "Error Fetching SVGs",
                SubTitle = ex.Message,
                IcoPath = IconPath,
                Score = 0
            });
        }

        return results;
    }

    public List<Result> Query(Query query, bool isDelayed)
    {
        var results = new List<Result>();
        var search = query.Search;

        var requestLogoData = new NavigateToBrowserData { Identifier = Constants.RequestLogo, Search = query.Search };
        var submitLogoData = new NavigateToBrowserData { Identifier = Constants.SubmitLogo, Search = query.Search };

        if (isDelayed && !string.IsNullOrEmpty(search))
        {
            Log.Info($"Delayed SVGL Query: Search='{query.Search}'", GetType());

            if (search.StartsWith("--"))
            {
                results.Add(new Result
                {
                    Title = "Refresh Cached Data",
                    SubTitle =
                        "Force a cache refresh to retrieve the latest data from the source. Note: The next plugin initialization may take a few milliseconds as the cache is cleared.",
                    IcoPath = IconPath,
                    Score = 100,
                    Action = _ =>
                    {
                        _cache.Remove(DefaultCacheKey);
                        return true;
                    }
                });
                return results;
            }

            try
            {
                var cachedResults = _cache.GetOrAdd(DefaultCacheKey, FetchDefaultResults, CachingOption);

                if (cachedResults.Any(r => r.Title == Constants.NoInternet))
                {
                    results.AddRange(cachedResults.Select(result => new Result
                    {
                        Title = result.Title,
                        SubTitle = result.SubTitle,
                        IcoPath = result.IcoPath,
                        Score = result.Score,
                        ContextData = result.ContextData
                    }));
                    return results;
                }


                var filterResults = cachedResults
                    .Where(r => r.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(r =>
                        string.Equals(r.Title, search, StringComparison.OrdinalIgnoreCase)) // Exact match first
                    .ThenByDescending(r =>
                        r.Title.StartsWith(search, StringComparison.OrdinalIgnoreCase)) // Then StartsWith Second
                    .ThenBy(r =>
                        r.Title.IndexOf(search,
                            StringComparison
                                .OrdinalIgnoreCase)) // Lastly, ones which contains the search term, earlier index first.
                    .ToList();
                //StringMatcher.FuzzySearch(search, r.Title)
                //var filteredResults = cachedResult.Select(r => StringMatcher.FuzzySearch(search, r.Title)).ToList();

                if (filterResults.Count == 0)
                {
                    results.Add(CreateNoResultsFound("No SVG Found", $"Could not found {query.Search} SVG"));
                    results.Add(new Result
                    {
                        Title = Constants.RequestLogo,
                        SubTitle = "Request a Logo on SVGL's Repository",
                        IcoPath = IconPath,
                        ContextData = requestLogoData,
                        Action = _ =>
                            OpenUrl($"{Constants.RequestLogoUrl}+{Utils.Utils.CapitalizeFirstLetter(query.Search)}",
                                Constants.RequestLogo)
                    });

                    results.Add(new Result
                    {
                        Title = Constants.SubmitLogo,
                        SubTitle = "Submit a Logo on SVGL's Repository",
                        IcoPath = IconPath,
                        ContextData = submitLogoData,
                        Action = _ => OpenUrl(Constants.SubmitLogoUrl, Constants.SubmitLogo)
                    });
                }

                results.AddRange(filterResults.Select(svg => new Result
                {
                    Title = svg.Title,
                    SubTitle = svg.SubTitle,
                    ContextData = svg.ContextData,
                    Score = 100,
                    IcoPath = svg.IcoPath
                }));

                // else
                // {
                //     var svgs = _apiClient.GetSVGFromSource(search).Result;
                //     results.AddRange(svgs.Select(svg =>
                //         new Result
                //         {
                //             Title = svg.Title,
                //             SubTitle = svg.Category.ToString(),
                //             IcoPath = IconPath,
                //             ContextData = svg,
                //             Score = 100
                //         }));
                // }
            }
            catch (Exception ex)
            {
                Log.Error($"Error occurred in Query method: {ex}", GetType());
                results.Add(new Result
                {
                    Title = "Error Fetching SVGs",
                    SubTitle = ex.Message,
                    IcoPath = IconPath,
                    Score = 0
                });
            }
        }

        ;

        return results;
    }


    // Context Menu Config from each result
    public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
    {
        return selectedResult?.ContextData switch
        {
            NavigateToBrowserData browserData => browserData.Identifier switch
            {
                Constants.RequestLogo => HandleNavigateToBrowserDataResult(Constants.RequestLogo,
                    $"{Constants.RequestLogoUrl}+{Utils.Utils.CapitalizeFirstLetter(browserData.Search)}"),
                Constants.SubmitLogo => HandleNavigateToBrowserDataResult(Constants.SubmitLogo,
                    Constants.SubmitLogoUrl),
                _ => []
            },
            Svgl svg => HandleSvgRoutes(svg.Route, svg.Wordmark),
            _ => []
        };
    }

    private List<Result> FetchDefaultResults()
    {
        var results = new List<Result>();
        try
        {
            var svgs = _apiClient.GetAllSvGs().GetAwaiter().GetResult();

            if (svgs != null)
            {
                results.AddRange(svgs.Select(svg => new Result
                {
                    Title = svg.Title,
                    SubTitle = svg.Category.ToString(),
                    IcoPath = IconPath,
                    Score = 100,
                    ContextData = svg
                }));
                return results;
            }

            Log.Info($"Fetched {results.Count} SVG results.", GetType());
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"Network Error fetching SVGs: {ex}", GetType());
            results.Add(new Result
            {
                Title = Constants.NoInternet,
                SubTitle = Constants.NoInternetSubTitle,
                IcoPath = IconPath,
                Score = 0
            });
            return results;
        }
        catch (Exception ex)
        {
            Log.Error($"Error fetching SVGs: {ex}", GetType());
            results.Add(new Result
            {
                Title = "Error Fetching SVGs",
                SubTitle = ex.Message,
                IcoPath = IconPath,
                Score = 0
            });
            return results;
        }

        return results.Count != 0 ? results : [CreateNoResultsFound()];
    }

    private static List<ContextMenuResult> HandleSvgRoutes(ThemeBase route, ThemeBase wordmark)
    {
        var results = new List<ContextMenuResult>();

        if (route is ThemeString routeStr)
        {
            results.Add(Utils.Utils.CreateCopyMenuItem(Constants.CopySvgLogoMessage, "\xE8C8",
                routeStr.Route, Key.Enter));
        }
        else if (route is ThemeObject routeObj)
        {
            results.Add(Utils.Utils.CreateCopyMenuItem(Constants.CopyLightThemeSvgLogoMessage, "\xE706",
                routeObj.Route.Light, Key.Enter));
            results.Add(Utils.Utils.CreateCopyMenuItem(Constants.CopyDarkThemeSvgLogoMessage, "\xE708",
                routeObj.Route.Dark, Key.Enter, ModifierKeys.Control));
        }

        if (wordmark is ThemeString wordStr)
        {
            results.Add(Utils.Utils.CreateCopyMenuItem(Constants.CopySvgWordmarkMessage, "\xE8D2",
                wordStr.Route, Key.Enter, ModifierKeys.Shift));
        }
        else if (wordmark is ThemeObject wordObj)
        {
            results.Add(Utils.Utils.CreateCopyMenuItem(Constants.CopyLightThemeSvgWordmarkMessage, "\xE8D2",
                wordObj.Route.Light, Key.Enter, ModifierKeys.Shift));
            results.Add(Utils.Utils.CreateCopyMenuItem(Constants.CopyDarkThemeSvgWordmarkMessage, "\xE8D3",
                wordObj.Route.Dark, Key.Enter, ModifierKeys.Control | ModifierKeys.Shift));
        }

        return results;
    }


    private bool OpenUrl(string url, string constantName)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            return true;
        }
        catch (Exception ex)
        {
            Context.API.ShowMsg("Error", $"Failed to open URL {constantName}: {ex.Message}");
            return false;
        }
    }

    private Result CreateNoResultsFound(string title = "No SVGs Available",
        string subTitle = "Could not fetch default SVG list", int score = 100)
    {
        return new Result
        {
            Title = title,
            SubTitle = subTitle,
            IcoPath = IconPath,
            Score = score,
            Action = _ =>
            {
                Context.API.ShowMsg("Info", "No SVG found for your search.");
                return false;
            }
        };
    }

    private List<ContextMenuResult> HandleNavigateToBrowserDataResult(string constantName, string url)
    {
        return
        [
            Utils.Utils.GetContextMenuResult(new GetContextMenuResult
            {
                Title = Constants.OpenInBrowserMessage,
                Glyph = "\xE8A7",
                AcceleratorKey = Key.Enter,
                CustomAction = _ => OpenUrl(url, constantName)
            })
        ];
    }

    /// <summary>
    /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
    public void Init(PluginInitContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Context.API.ThemeChanged += OnThemeChanged;
        UpdateIconPath(Context.API.GetCurrentTheme());

        Log.Info("SVGL plugin initialized successfully", GetType());
        Log.Info("SVGL PLUGIN LOADED", GetType());
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Wrapper method for <see cref="Dispose()"/> that dispose additional objects and events form the plugin itself.
    /// </summary>
    /// <param name="disposing">Indicate that the plugin is disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (Disposed || !disposing) return;

        if (Context?.API != null) Context.API.ThemeChanged -= OnThemeChanged;

        Disposed = true;
    }

    private void UpdateIconPath(Theme theme)
    {
        IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite
            ? "Images/svgl.light.png"
            : "Images/svgl.dark.png";
    }

    private void OnThemeChanged(Theme currentTheme, Theme newTheme)
    {
        UpdateIconPath(newTheme);
    }
}