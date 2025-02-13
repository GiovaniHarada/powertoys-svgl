using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LazyCache;
using ManagedCommon;
using Microsoft.Extensions.Caching.Memory;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.SVGL
{
    public class Main : IPlugin, IContextMenu, IDisposable, IDelayedExecutionPlugin
    {
        public static string PluginID => "BFF179A472A84D8D9DA6640346C61102";
        public string Name => "SVGL";
        public string Description => "SVGL Description";
        private PluginInitContext Context { get; set; }
        private string IconPath { get; set; }
        private bool Disposed { get; set; }

        private readonly MyApiClients apiClient = new MyApiClients();

        // Caching variables
        private readonly IAppCache _cache = new CachingService();
        private static MemoryCacheEntryOptions cachingOption = new MemoryCacheEntryOptions()
        {
            Priority = CacheItemPriority.NeverRemove
        };
        private const string DefaultCacheKey = "SVGL_AllResults";
        //private const int CacheMinutes = 30;

        /// <summary>
        /// Return a filtered list, based on the given query.
        /// </summary>
        /// <param name="query">The query to filter the list.</param>
        /// <returns>A filtered list, can be empty when nothing was found.</returns>
        public List<Result> Query(Query query)
        {
            Log.Info($"SVGL Query: Search='{query.Search}'", GetType());
            var results = new List<Result>();


            if (string.IsNullOrWhiteSpace(query.Search))
            {
                var cachedResults = _cache.GetOrAdd(DefaultCacheKey, () => FetchDefaultResults(), cachingOption);
                var slicedResults = cachedResults.Slice(0, 15);

                foreach (var result in slicedResults)
                {
                    results.Add(new Result
                    {
                        Title = result.Title,
                        SubTitle = result.SubTitle,
                        IcoPath = result.IcoPath,
                        Score = result.Score,
                        ContextData = result.ContextData,
                    });
                }

                return results;
            }

            return results;
        }

        private List<Result> FetchDefaultResults()
        {
            var results = new List<Result>();
            try
            {
                //var apiClient = new MyApiClients();
                var svgs = apiClient.GetAllSVGs().GetAwaiter().GetResult();

                if (svgs != null)
                {

                    //foreach (var svg in svgs)
                    //{
                    results.AddRange(svgs.Select(svg => new Result
                    {
                        Title = svg.Title,
                        SubTitle = svg.Category.ToString(),
                        IcoPath = IconPath,
                        Score = 100,
                        ContextData = svg,
                    }));
                    //}
                }

                Log.Info($"Fetched {results.Count} SVG results.", GetType());
            }
            catch (Exception ex)
            {
                Log.Error($"Error fetching SVGs: {ex}", GetType());
                results.Add(new Result
                {
                    Title = "Error Fetching SVGs",
                    SubTitle = ex.Message,
                    IcoPath = IconPath,
                    Score = 0,
                });
            }

            return results.Any() ? results : new List<Result> { CreateNoResultsFound() };
        }


        public List<Result> Query(Query query, bool isDelayed)
        {

            var results = new List<Result>();
            var search = query.Search;
            var cachedResult = _cache.Get<List<Result>>(DefaultCacheKey);

            INavigateToBrowserData requestLogoData = new INavigateToBrowserData { Identifier = Constants.RequestLogo, Search = query.Search };
            INavigateToBrowserData submitLogoData = new INavigateToBrowserData { Identifier = Constants.SubmitLogo, Search = query.Search };

            if (isDelayed && !string.IsNullOrEmpty(search))
            {
                Log.Info($"Delayed SVGL Query: Search='{query.Search}'", GetType());
                try
                {
                    if (cachedResult != null)
                    {
                        var filterResult = cachedResult
                                           .Where(r => r.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                                           .OrderByDescending(r => string.Equals(r.Title, search, StringComparison.OrdinalIgnoreCase)) // Exact match first
                                           .ThenByDescending(r => r.Title.StartsWith(search, StringComparison.OrdinalIgnoreCase)) // Then StartsWith Second
                                           .ThenBy(r => r.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase)) // Lastly, ones which contains the search term, earlier index first.
                                           .ToList();

                        if (filterResult.Count == 0)
                        {
                            results.Add(CreateNoResultsFound("No SVG Found", $"Could not found {query.Search} SVG"));
                            results.Add(new Result
                            {
                                Title = Constants.RequestLogo,
                                SubTitle = "Request a Logo on SVGL's Repository",
                                IcoPath = IconPath,
                                ContextData = requestLogoData,
                                Action = _ => OpenURL($"{Constants.RequestLogoURL}+{Utils.CapitalizeFirstLetter(query.Search)}", Constants.RequestLogo)
                            });

                            results.Add(new Result
                            {
                                Title = Constants.SubmitLogo,
                                SubTitle = "Submit a Logo on SVGL's Repository",
                                IcoPath = IconPath,
                                ContextData = submitLogoData,
                                Action = _ => OpenURL(Constants.SubmitLogoURL, Constants.SubmitLogo)
                            });
                        };

                        foreach (var svg in filterResult)
                        {
                            results.Add(new Result
                            {
                                Title = svg.Title,
                                SubTitle = svg.SubTitle,
                                ContextData = svg.ContextData,
                                Score = 100,
                                IcoPath = svg.IcoPath
                            });
                        }
                    }
                    else
                    {
                        var svgs = apiClient.GetSVGFromSource(search).Result;
                        foreach (var svg in svgs)
                        {
                            string routeUrl = svg.Route switch
                            {
                                ThemeString s => s.Route,
                                ThemeObject o => o.Route.Dark,
                                _ => string.Empty
                            };

                            if (string.IsNullOrEmpty(routeUrl)) continue;

                            results.Add(
                                new Result
                                {
                                    Title = svg.Title,
                                    SubTitle = svg.Category.ToString(),
                                    IcoPath = IconPath,
                                    ContextData = svg,
                                    Score = 100
                                });
                        }
                    }
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
            };

            return results;

        }


        // Context Menu Config from each result
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            if (selectedResult?.ContextData is INavigateToBrowserData browserData)
            {
                return browserData.Identifier switch
                {
                    Constants.RequestLogo => HandleNavigateToBrowserDataResult(browserData, Constants.RequestLogo, $"{Constants.RequestLogoURL}+{Utils.CapitalizeFirstLetter(browserData.Search)}"),
                    Constants.SubmitLogo => HandleNavigateToBrowserDataResult(browserData, Constants.SubmitLogo, Constants.SubmitLogoURL),
                    _ => new List<ContextMenuResult>()
                };
            }
            if (selectedResult?.ContextData is SVGL svg)
            {
                return HandleSvgRoutes(svg.Route, svg.Wordmark);
            }

            return new List<ContextMenuResult>();
        }

        private List<ContextMenuResult> HandleSvgRoutes(ThemeBase route, ThemeBase wordmark)
        {
            var results = new List<ContextMenuResult>();

            if (route is ThemeString routeStr)
            {
                results.Add(Utils.CreateCopyMenuItem(Constants.CopySVGLogoMessage, "\xE8C8",
                    routeStr.Route, Key.Enter));
            }
            else if (route is ThemeObject routeObj)
            {
                results.Add(Utils.CreateCopyMenuItem(Constants.CopyLightThemeSVGLogoMessage, "\xE706",
                    routeObj.Route.Light, Key.Enter));
                results.Add(Utils.CreateCopyMenuItem(Constants.CopyDarkThemeSVGLogoMessage, "\xE708",
                    routeObj.Route.Dark, Key.Enter, ModifierKeys.Control));
            }

            if (wordmark is ThemeString wordStr)
            {
                results.Add(Utils.CreateCopyMenuItem(Constants.CopySVGWordmarkMessage, "\xE8D2",
                    wordStr.Route, Key.Enter, ModifierKeys.Control));
            }
            else if (wordmark is ThemeObject wordObj)
            {
                results.Add(Utils.CreateCopyMenuItem(Constants.CopyLightThemeSVGWordmarMessage, "\xE8D2",
                    wordObj.Route.Light, Key.Enter, ModifierKeys.Shift));
                results.Add(Utils.CreateCopyMenuItem(Constants.CopyDarkThemeSVGWordmarMessage, "\xE8D3",
                    wordObj.Route.Dark, Key.Enter, ModifierKeys.Control | ModifierKeys.Shift));
            }

            return results;
        }


        private bool OpenURL(string url, string constantName)
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

        private Result CreateNoResultsFound(string Title = "No SVGs Available", string subTitle = "Could not fetch deafult SVG list", int Score = 100)
        {
            return new Result
            {
                Title = Title,
                SubTitle = subTitle,
                IcoPath = IconPath,
                Score = Score,
                Action = _ =>
                {
                    Context.API.ShowMsg("Info", "No SVG found for your search.");
                    return false;
                }
            };
        }

        private List<ContextMenuResult> HandleNavigateToBrowserDataResult(INavigateToBrowserData data, string constantName, string url)
        {
            return new List<ContextMenuResult>
        {
            Utils.GetContextMenuResult(new IGetContextMenuResult {
                Title = Constants.OpenInBrowserMessage,
                Glyph = "\xE8A7",
                AcceleratorKey = Key.Enter,
                CustomAction = _ => OpenURL(url, constantName)
            })
        };
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

            Log.Info("SVGL plugin initialized successfully", GetType()); // Using Wox.Plugin.Logger
            Log.Info("SVGL PLUGIN LOADED", GetType()); // For debug builds
        }

        /// <summary>
        /// Return a list context menu entries for a given <see cref="Result"/> (shown at the right side of the result).
        /// </summary>
        /// <param name="selectedResult">The <see cref="Result"/> for the list with context menu entries.</param>
        /// <returns>A list context menu entries.</returns>

        /// <inheritdoc/>
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
            if (Disposed || !disposing)
            {
                return;
            }

            if (Context?.API != null)
            {
                Context.API.ThemeChanged -= OnThemeChanged;
            }

            Disposed = true;
        }

        private void UpdateIconPath(Theme theme) => IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? "Images/svgl.light.png" : "Images/svgl.dark.png";

        private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);

    }

}
