using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LazyCache;
using ManagedCommon;
using Microsoft.Extensions.Caching.Memory;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.SVGL
{
    /// <summary>
    /// Main class of this plugin that implement all used interfaces.
    /// </summary>
    public class Main : IPlugin, IContextMenu, IDisposable, IDelayedExecutionPlugin
    {
        /// <summary>
        /// ID of the plugin.
        /// </summary>
        /// e3439fc4-7771-443d-84a6-41083ea80819 -- Generated UUID
        public static string PluginID => "BFF179A472A84D8D9DA6640346C61102";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name => "SVGL";

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description => "SVGL Description";

        private PluginInitContext Context { get; set; }

        private string IconPath { get; set; }

        private bool Disposed { get; set; }



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
            //var search = query.Search;
            //var apiClient = new MyApiClients();


            if (string.IsNullOrWhiteSpace(query.Search))
            {
                //string cacheKey = "SVGL_DEFAULT_RESULTS";
                //var CachedResult = _cache.Get(cacheKey) is List<Result> cached;

                //if (_cache.Get(cacheKey) is List<Result>)
                //{
                //    var cachedResults = _cache.Get(cacheKey) as List<Result>;
                //    return cachedResults;
                //}
                //if (_cache.Get(DefaultCacheKey) is List<Result> cachedResults)
                //{
                //    return cachedResults;
                //}    results.Add(new Result
                results.Add(new Result
                {
                    Title = "Loading...",
                    SubTitle = "Please wait while we fetch the data...",
                    IcoPath = IconPath, // You can use an icon that indicates loading
                });

                var cachedResults = _cache.GetOrAdd(DefaultCacheKey, () => FetchDefaultResults(), cachingOption);
                var slicedResults = cachedResults.Slice(0, 15);

                results.Clear();
                //foreach (var result in cachedResults)
                //{
                //    Log.Info($"Cached result item: Title = {result.Title}, SubTitle = {result.SubTitle}, Entire Data = {result}", GetType());
                //}




                //return _cache.GetOrAdd(DefaultCacheKey, () => FetchDefaultResults(query.Search));
                //return cachedResults;
                foreach (var result in slicedResults)
                {
                    results.Add(new Result
                    {
                        Title = result.Title,
                        SubTitle = result.SubTitle,
                        IcoPath = result.IcoPath,
                        Score = result.Score,
                        ContextData = result.ContextData,
                        //Action = result.Action,
                        //QueryTextDisplay = query.ActionKeyword // Use only the action keyword
                    });
                }

                return results;

                //try
                //{
                //    // Make this truly, async
                //    var svgs = Task.Run(async () => await apiClient.GetSVGsByLimit(10)).Result;
                //    Log.Info($"Fetched {svgs.Count} SVGs", GetType());
                //    foreach (var svg in svgs)
                //    {
                //        string routeUrl = svg.Route switch
                //        {
                //            ThemeString s => s.Route,
                //            ThemeObject o => o.Route.Dark,
                //            _ => string.Empty
                //        };

                //        if (string.IsNullOrEmpty(routeUrl)) continue;

                //        Log.Info($"Added: {svg.Title} | URL: {routeUrl}", GetType());
                //        results.Add(new Result
                //        {
                //            Title = svg.Title,
                //            SubTitle = $"Category {routeUrl} | URL {routeUrl}",
                //            IcoPath = "Images/svgl.light.png",
                //            Score = 50,
                //            ContextData = routeUrl,
                //            ToolTipData = new ToolTipData("Route", $"Link for the Actual SVG: {routeUrl}"),

                //        });

                //    }
                //    // Caching after all the values are inserted in results by looping them over.
                //    if (results.Count > 0)
                //    {
                //        _cache.Add(DefaultCacheKey, results, new CacheItemPolicy
                //        {
                //            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(CacheExpirationInMinutes)
                //        });
                //        Log.Debug($"Cached {results.Count} items", GetType());
                //    }
                //}
                //catch (Exception ex)
                //{
                //    results.Add(new Result
                //    {
                //        Title = "Error Fetching SVGs",
                //        SubTitle = ex.Message,
                //        IcoPath = IconPath,
                //        Score = 0
                //    });
                //}
            }

            return results;
        }

        private List<Result> FetchDefaultResults()
        {
            var results = new List<Result>();
            try
            {
                var apiClient = new MyApiClients();
                var svgs = Task.Run(() => apiClient.GetAllSVGs()).GetAwaiter().GetResult();

                foreach (var svg in svgs)
                {
                    string routeUrl = svg.Route switch
                    {
                        ThemeString s => s.Route,
                        ThemeObject o => o.Route.Dark ?? string.Empty,
                        _ => string.Empty
                    };

                    if (string.IsNullOrWhiteSpace(routeUrl)) continue;

                    results.Add(new Result
                    {
                        Title = svg.Title,
                        SubTitle = svg.Category.ToString(),
                        IcoPath = "Images/svgl.light.png",
                        Score = 100,
                        ContextData = svg,
                    });

                    //if (svg.Route is ThemeString routeStr)
                    //{
                    //    results.Add(new Result
                    //    {
                    //        Title = svg.Title,
                    //        SubTitle = $"Category: {svg.Category} | URL: {routeStr.Route}",
                    //        IcoPath = "Images/svgl.light.png",
                    //        Score = 100,
                    //        ContextData = svg,
                    //        Action = _ => CopyToClipboard(routeStr.Route)
                    //    });
                    //}
                    //else if (svg.Route is ThemeObject routeObj)
                    //{
                    //    results.Add(new Result
                    //    {
                    //        Title = svg.Title,
                    //        SubTitle = $"Category: {svg.Category} | Light URL: {routeObj.Route.Light} | Dark URL: {routeObj.Route.Dark}",
                    //        IcoPath = "Images/svgl.light.png",
                    //        Score = 100,
                    //        ContextData = routeObj,
                    //        Action = _ => CopyToClipboard(routeObj.Route.Dark)
                    //    });
                    //}
                    //results.Add(new Result
                    //{
                    //    Title = svg.Title,
                    //    SubTitle = $"Category: {svg.Category} | URL: {routeUrl}",
                    //    IcoPath = "Images/svgl.light.png",
                    //    Score = 100,
                    //    ContextData = routeUrl,
                    //    //QueryTextDisplay = search,
                    //    Action = _ => CopyToClipboard(routeUrl)
                    //});

                }
                Log.Info($"Result from FetchDefaultTypes Class: {results}", GetType());
            }
            catch (Exception ex)
            {
                results.Add(new Result
                {
                    Title = "Error Fetching SVGs",
                    SubTitle = ex.Message,
                    IcoPath = IconPath,
                    Score = 0,
                });
            }

            return results.Count > 0 ? results : [CreateNoResultsFound()];
        }



        // The Delayed Query Class
        public List<Result> Query(Query query, bool isDelayed)
        {

            var results = new List<Result>();
            var search = query.Search;
            var apiClient = new MyApiClients();
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
                                Title = Constants.RequestLogo, // Fix the ordering of result, currently Request Logo is at top and then No SVG Found, which should be other way around. 
                                SubTitle = "Request a Logo on SVGL's Repository",
                                IcoPath = IconPath,
                                ContextData = requestLogoData,
                                //Action = _ =>
                                //{
                                //    try
                                //    {
                                //        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                //        {
                                //            FileName = $"https://github.com/pheralb/svgl/issues/new?assignees=&labels=request&projects=&template=request-svg.yml&title=%5B%F0%9F%94%94+Request+SVG%5D%3A+{query.Search}",
                                //            UseShellExecute = true
                                //        });
                                //        return true;
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        Context.API.ShowMsg("Error", $"Failed to open Request Log URL: {ex.Message}");
                                //        return false;
                                //    }
                                //}
                            });

                            results.Add(new Result
                            {
                                Title = Constants.SubmitLogo,
                                SubTitle = "Submit a Logo on SVGL's Repository",
                                IcoPath = IconPath,
                                ContextData = submitLogoData,
                                //Action = _ =>
                                //{
                                //    try
                                //    {
                                //        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                //        {
                                //            FileName = "https://github.com/pheralb/svgl#-getting-started",
                                //            UseShellExecute = true
                                //        });
                                //        return true;
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        Context.API.ShowMsg("Error", $"Failed to Open Submit Logo URL: {ex.Message}");
                                //        return false;
                                //    }
                                //}
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
                        var svgs = Task.Run(() => apiClient.GetSVGFromSource(search)).GetAwaiter().GetResult();
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

                            //if (svg.Route is ThemeString routeString)
                            //{
                            //    results.Add(
                            //            new Result
                            //            {
                            //                Title = svg.Title,
                            //                SubTitle = $"Category: {svg.Category?.ToString()} | Actual URL: {routeString.Route}",
                            //                IcoPath = "Images/svgl.light.png",
                            //                Score = 100,
                            //                ContextData = routeString,
                            //                Action = _ => CopyToClipboard(routeString.Route),
                            //            }

                            //    );
                            //};

                            //if (svg.Route is ThemeObject routeObject)
                            //{
                            //    results.Add(
                            //        new Result
                            //        {
                            //            Title = svg.Title,
                            //            SubTitle = $"Category: {svg.Category?.ToString()} | Light URL: {routeObject.Route.Light} | Dark URL: {routeObject.Route.Dark}",
                            //            Score = 100,
                            //            IcoPath = IconPath,
                            //            ContextData = routeObject,
                            //        });
                            //};

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
            }
            ;

            return results;

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
                CustomAction = _ => {
                    try
                    {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo{
                        FileName = url,
                        UseShellExecute = true
                    });
                        return true;
                    } catch (Exception ex)
                    {
                        Context.API.ShowMsg("Error", $"Failed to open URL {constantName}: {ex.Message}");
                        return false;
                    }
                }
            })
        };
        }

        private ContextMenuResult CreateCopyMenuItem(string title, string glyph, string content,
    Key key, ModifierKeys modifiers = ModifierKeys.None)
        {
            return Utils.GetContextMenuResult(new IGetContextMenuResult
            {
                Title = title,
                Glyph = glyph,
                AcceleratorKey = key,
                AcceleratorModifiers = modifiers,
                CopyContent = content
            });
        }

        private List<ContextMenuResult> HandleSvgRoutes(ThemeBase route, ThemeBase wordmark)
        {
            var results = new List<ContextMenuResult>();

            if (route is ThemeString routeStr)
            {
                results.Add(CreateCopyMenuItem(Constants.CopySVGLogoMessage, "\xE8C8",
                    routeStr.Route, Key.Enter));
            }
            else if (route is ThemeObject routeObj)
            {
                results.Add(CreateCopyMenuItem(Constants.CopyLightThemeSVGLogoMessage, "\xE706",
                    routeObj.Route.Light, Key.Enter));
                results.Add(CreateCopyMenuItem(Constants.CopyDarkThemeSVGLogoMessage, "\xE708",
                    routeObj.Route.Dark, Key.Enter, ModifierKeys.Control));
            }

            if (wordmark is ThemeString wordStr)
            {
                results.Add(CreateCopyMenuItem(Constants.CopySVGWordmarkMessage, "\xE8D2",
                    wordStr.Route, Key.Enter, ModifierKeys.Control));
            }
            else if (wordmark is ThemeObject wordObj)
            {
                results.Add(CreateCopyMenuItem(Constants.CopyLightThemeSVGWordmarMessage, "\xE706",
                    wordObj.Route.Light, Key.Enter, ModifierKeys.Shift));
                results.Add(CreateCopyMenuItem(Constants.CopyDarkThemeSVGWordmarMessage, "\xE8D3",
                    wordObj.Route.Dark, Key.Enter, ModifierKeys.Control | ModifierKeys.Shift));
            }

            return results;
        }

        // Context Menu Config from each result
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            var apiClient = new MyApiClients();

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
                if (svg.Route is ThemeString routeStr)
                {
                    if (svg.Wordmark is ThemeString wordStr)
                    {

                        return new List<ContextMenuResult>
                    {   Utils.GetContextMenuResult(new IGetContextMenuResult
                    {
                        Title = Constants.CopySVGLogoMessage,
                        Glyph = "\xE8C8",
                        AcceleratorKey = Key.Enter,
                        CopyContent = routeStr.Route
                    }) ,

                        Utils.GetContextMenuResult (new IGetContextMenuResult
                        {
                            Title = Constants.CopySVGWordmarkMessage,
                            Glyph = "\xE8D2",
                            AcceleratorKey = Key.Enter,
                            AcceleratorModifiers = ModifierKeys.Control,
                            CopyContent = wordStr.Route
                        }),

                    };
                    }
                    else if (svg.Wordmark is ThemeObject wordObj)
                    {
                        return new List<ContextMenuResult> {
                            Utils.GetContextMenuResult(new IGetContextMenuResult{
                                Title = Constants.CopySVGLogoMessage,
                                Glyph = "\xE8C8",
                                AcceleratorKey = Key.Enter,
                                CopyContent = routeStr.Route
                            }),

                        Utils.GetContextMenuResult(new IGetContextMenuResult {
                            Title = Constants.CopyLightThemeSVGWordmarMessage,
                            Glyph = "\xE8D2",
                            AcceleratorKey = Key.Enter,
                            AcceleratorModifiers = ModifierKeys.Shift,
                            CopyContent = wordObj.Route.Light
                        }),

                        Utils.GetContextMenuResult(new IGetContextMenuResult{
                            Title = Constants.CopyDarkThemeSVGWordmarMessage,
                            Glyph = "\xE8D3", // Quiet Hours (Moon)
                            AcceleratorKey = Key.Enter,
                            AcceleratorModifiers = ModifierKeys.Control | ModifierKeys.Shift,
                            CopyContent = wordObj.Route.Dark,
                        }),

                        };
                    }

                    return new List<ContextMenuResult> {
                        Utils.GetContextMenuResult(new IGetContextMenuResult
                        {
                            Title = Constants.CopySVGLogoMessage,
                            Glyph = "\xE8C8",
                            AcceleratorKey = Key.Enter,
                            CopyContent = routeStr.Route
                        }),

                    };
                }

                if (svg.Route is ThemeObject routeObj)
                {
                    if (svg.Wordmark is ThemeObject wordObj)
                    {
                        return new List<ContextMenuResult>
                    {
                            Utils.GetContextMenuResult(new IGetContextMenuResult
                            {
                                Title = Constants.CopyLightThemeSVGLogoMessage,
                                Glyph = "\xE706",// Brightness (Sun)
                                AcceleratorKey = Key.Enter,
                                CopyContent = routeObj.Route.Light
                            }),
                            Utils.GetContextMenuResult(new IGetContextMenuResult{
                                Title = Constants.CopyDarkThemeSVGLogoMessage,
                                Glyph = "\xE708", // Quiet Hours (Moon)
                                AcceleratorKey = Key.Enter,
                                AcceleratorModifiers = ModifierKeys.Control,
                                CopyContent = routeObj.Route.Dark
                            }),
                            Utils.GetContextMenuResult(new IGetContextMenuResult
                            {
                                Title = Constants.CopyLightThemeSVGWordmarMessage,
                                Glyph = "\xE706",
                                AcceleratorKey = Key.Enter,
                                AcceleratorModifiers = ModifierKeys.Shift,
                                CopyContent = wordObj.Route.Light
                            }),
                            Utils.GetContextMenuResult(new IGetContextMenuResult{
                                Title = Constants.CopyDarkThemeSVGLogoMessage,
                                Glyph = "\xE8D3",
                                AcceleratorKey = Key.Enter,
                                AcceleratorModifiers = ModifierKeys.Control | ModifierKeys.Shift,
                                CopyContent = wordObj.Route.Dark
                            })
                    };

                    }
                    else if (svg.Wordmark is ThemeString wordStr)
                    {
                        return new List<ContextMenuResult>
                        {
                             Utils.GetContextMenuResult(new IGetContextMenuResult{
                                Title = Constants.CopyLightThemeSVGLogoMessage,
                                Glyph = "\xE706",// Brightness (Sun)
                                AcceleratorKey = Key.Enter,
                                CopyContent = routeObj.Route.Light
                            }),
                            Utils.GetContextMenuResult(new IGetContextMenuResult{
                                Title = Constants.CopyDarkThemeSVGLogoMessage,
                                Glyph = "\xE708", // Quiet Hours (Moon)
                                AcceleratorKey = Key.Enter,
                                AcceleratorModifiers = ModifierKeys.Control,
                                CopyContent = routeObj.Route.Dark
                            }),
                            Utils.GetContextMenuResult (new IGetContextMenuResult{
                                Title = Constants.CopySVGWordmarkMessage,
                                Glyph = "\xE8D2",
                                AcceleratorKey = Key.Enter,
                                AcceleratorModifiers = ModifierKeys.Control,
                                CopyContent = wordStr.Route
                            }),
                        };
                    }

                    return new List<ContextMenuResult>
                    {
                              Utils.GetContextMenuResult(new IGetContextMenuResult
                            {
                                Title = Constants.CopyLightThemeSVGLogoMessage,
                                Glyph = "\xE706",// Brightness (Sun)
                                AcceleratorKey = Key.Enter,
                                CopyContent = routeObj.Route.Light
                            }),
                            Utils.GetContextMenuResult(new IGetContextMenuResult{
                                Title = Constants.CopyDarkThemeSVGLogoMessage,
                                Glyph = "\xE708", // Quiet Hours (Moon)
                                AcceleratorKey = Key.Enter,
                                AcceleratorModifiers = ModifierKeys.Control,
                                CopyContent = routeObj.Route.Dark
                            }),
                    };
                }
            }


            return new List<ContextMenuResult>();
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
