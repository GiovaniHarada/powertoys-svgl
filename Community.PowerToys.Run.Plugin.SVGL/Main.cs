using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LazyCache;
using ManagedCommon;
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
        private const string DefaultCacheKey = "SVGL_DefaultResults";
        private const int CacheMinutes = 30;

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
                //}
                var cachedResults = _cache.GetOrAdd(DefaultCacheKey, () => FetchDefaultResults());

                //foreach (var result in cachedResults)
                //{
                //    Log.Info($"Cached result item: Title = {result.Title}, SubTitle = {result.SubTitle}, Entire Data = {result}", GetType());
                //}

                //return _cache.GetOrAdd(DefaultCacheKey, () => FetchDefaultResults(query.Search));
                //return cachedResults;
                foreach (var result in cachedResults)
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
                    //QueryTextDisplay = string.Empty
                });
            }

            return results.Count > 0 ? results : [CreateNoResultsFound()];
        }

        private Result CreateNoResultsFound()
        {
            return new Result
            {
                Title = "No SVGs Available",
                SubTitle = "Could not fetch default SVG list",
                IcoPath = IconPath,
                Score = 0,
                QueryTextDisplay = string.Empty
            };
        }

        // The Delayed Query Class
        public List<Result> Query(Query query, bool isDelayed)
        {
            var results = new List<Result>();
            var search = query.Search;
            var apiClient = new MyApiClients();

            if (isDelayed && !string.IsNullOrEmpty(search))
            {
                Log.Info($"Delayed SVGL Query: Search='{query.Search}'", GetType());
                try
                {
                    var svgs = Task.Run(async () => await apiClient.GetSVGs(search)).Result;
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
            var apiClient = new MyApiClients();
            if (selectedResult?.ContextData is SVGL svg)
            {
                if (svg.Route is ThemeString routeStr)
                {
                    if (svg.Wordmark is ThemeString wordStr)
                    {

                        return new List<ContextMenuResult>
                    {
                        new ContextMenuResult
                        {
                            PluginName = Name,
                            Title = "Copy Logo SVG (Enter)",
                            AcceleratorKey = Key.Enter,
                            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                            Glyph = "\xE8C8", // Copy,
                            Action = _ =>
                            {
                                var content = Task.Run(async () => await apiClient.GetSVGContent(routeStr.Route)).Result;
                                Log.Info($"Copy Logo SVG: {content}", GetType());
                                CopyToClipboard(content);
                                return true;
                            },
                        },
                        new ContextMenuResult{
                            PluginName = Name,
                            Title = "Copy Wordmark SVG (Ctrl + Enter)",
                            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                            Glyph = "\xE8C8", // Copy
                            AcceleratorKey = Key.Enter,
                            AcceleratorModifiers = ModifierKeys.Control,
                            Action = _ =>
                            {
                                var content = Task.Run(async () => await apiClient.GetSVGContent(wordStr.Route)).Result;
                                CopyToClipboard(content);
                                return true;
                            }
                        }
                    };
                    }

                    return new List<ContextMenuResult> { new ContextMenuResult {
                        PluginName = Name,
                        Title = "Copy Logo SVG (Enter)",
                        FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                        Glyph = "\xE8C8", // Copy
                        AcceleratorKey = Key.Enter,
                        Action = _ =>
                        {
                            var content = Task.Run(async () => await apiClient.GetSVGContent(routeStr.Route)).Result;
                            CopyToClipboard(content);
                            return true;
                        }
                    } };
                }

                if (svg.Route is ThemeObject routeObj)
                {
                    if (svg.Wordmark is ThemeObject wordObj)
                    {
                        return new List<ContextMenuResult>
                    {
                        new ContextMenuResult
                        {
                            PluginName = Name,
                            Title = "Copy Light Theme Logo (Enter)",
                            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                            Glyph = "\xE8C8", // Copy
                            AcceleratorKey = Key.Enter,
                            Action = _ =>
                            {
                                var content = Task.Run(async () => await apiClient.GetSVGContent(routeObj.Route.Light)).Result;
                                CopyToClipboard(content);
                                return true;
                            }
                        },
                        new ContextMenuResult {
                            PluginName = Name,
                            Title = "Copy Dark Theme Logo (Ctrl + Enter)",
                            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                            Glyph = "\xE708", // Quiet Hours (Moon)
                            AcceleratorKey = Key.Enter,
                            AcceleratorModifiers = ModifierKeys.Control,
                            Action = _ =>
                            {
                                string content = Task.Run(async () => await apiClient.GetSVGContent(routeObj.Route.Dark)).Result;
                                CopyToClipboard(content);
                                return true;
                            }
                        },
                        new ContextMenuResult {
                            PluginName = Name,
                            Title = "Copy Light Theme Wordmark (Shift + Enter)",
                            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                            Glyph = "\xE8C8", // Copy
                            AcceleratorKey = Key.Enter,
                            AcceleratorModifiers = ModifierKeys.Shift,
                            Action = _ =>
                            {
                                string content = Task.Run(async () => await apiClient.GetSVGContent(wordObj.Route.Light)).Result;
                                CopyToClipboard(content);
                                return true;
                            }
                        },
                        new ContextMenuResult {
                            PluginName = Name,
                            Title = "Copy Light Dark Wordmark (Ctrl + Shift + Enter)",
                            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                            Glyph = "\xE708", // Copy
                            AcceleratorKey = Key.Enter,
                            AcceleratorModifiers = ModifierKeys.Control | ModifierKeys.Shift,
                            Action = _ =>
                            {
                                string content = Task.Run(async () => await apiClient.GetSVGContent(wordObj.Route.Dark)).Result;
                                CopyToClipboard(content);
                                return true;
                            }
                        }
                    };

                    }

                    return new List<ContextMenuResult>
                    {
                        new ContextMenuResult
                        {
                            PluginName = Name,
                            Title = "Copy Light Theme Logo (Enter)",
                            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                            Glyph = "\xE8C8", // Copy
                            AcceleratorKey = Key.Enter,
                            Action = _ =>
                            {
                                var content = Task.Run(async () => await apiClient.GetSVGContent(routeObj.Route.Light)).Result;
                                CopyToClipboard(content);
                                return true;
                            }
                        },
                        new ContextMenuResult {
                            PluginName = Name,
                            Title = "Copy Dark Theme Logo (Ctrl + Enter)",
                            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                            Glyph = "\xE708", // Quiet Hours (Moon)
                            AcceleratorKey = Key.Enter,
                            AcceleratorModifiers = ModifierKeys.Control,
                            Action = _ =>
                            {
                                string content = Task.Run(async () => await apiClient.GetSVGContent(routeObj.Route.Dark)).Result;
                                CopyToClipboard(content);
                                return true;
                            }
                        }
                    };
                }
            }
            //if (selectedResult?.ContextData is ThemeString routeURL && !string.IsNullOrEmpty(routeURL.Route))
            //{
            //    return new List<ContextMenuResult> {
            //        new ContextMenuResult
            //            {
            //        PluginName = Name,
            //        Title = "Copy URL (Enter)",
            //        FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
            //        Glyph = "\xE8C8", // Copy
            //        AcceleratorKey = Key.Enter,
            //        Action = _ => {
            //            var content = Task.Run(async () => await apiClient.GetSVGContent(routeURL.Route)).Result;
            //            Log.Info($"SVG Content: {content}", GetType());
            //            CopyToClipboard(content);
            //            return true;
            //        },
            //    },
            //        new ContextMenuResult
            //            {
            //        PluginName = Name,
            //        Title = "Open in Browser (Ctrl + Enter)",
            //        FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
            //        Glyph = "\xE8A7", // Icon for opening
            //        AcceleratorKey = Key.B,
            //        AcceleratorModifiers = ModifierKeys.Control,
            //        Action = _ =>
            //    {
            //        try
            //        {
            //            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            //            {
            //                FileName = routeURL.Route,
            //                UseShellExecute = true
            //            });
            //            return true;
            //        }
            //        catch (Exception ex)
            //        {
            //            Context.API.ShowMsg("Error", $"Failed to open URL: {ex.Message}");
            //            return false;
            //        }
            //    }
            //        }
            //};
            //}

            //else if (selectedResult?.ContextData is ThemeObject routeObject)
            //{
            //    return new List<ContextMenuResult>
            //    {
            //        new ContextMenuResult {
            //            PluginName = Name,
            //            Title = "Copy Light Theme Icon (Enter)",
            //            AcceleratorKey = Key.Enter,
            //            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
            //            Glyph = "\xE8C8", // Copy
            //            Action = _ => {
            //                var content = Task.Run(async () => await apiClient.GetSVGContent(routeObject.Route.Light)).Result;
            //                Log.Info($"Light Theme SVG Content: {content}", GetType());
            //                CopyToClipboard(content); return true; }
            //        },
            //        new ContextMenuResult
            //        {
            //            PluginName = Name,
            //            Title = "Copy Dark Theme Icon (Ctrl + Enter)",
            //            AcceleratorKey = Key.Enter,
            //            AcceleratorModifiers = ModifierKeys.Control,
            //            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
            //            Glyph = "\xE708", // Quiet Hours
            //            Action = _ => {var content = Task.Run(async () => await apiClient.GetSVGContent(routeObject.Route.Dark)).Result; Log.Info($"Dark Theme SVG Content: {content}", GetType()); CopyToClipboard(content); return true; }
            //        }
            //    };
            //}

            return new List<ContextMenuResult>();
        }

        public static bool CopyToClipboard(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Clipboard.SetText(value);
                return true;
            }
            return false;
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

        // Types for SVGL Response (From SVGL.API.DEMO)
        public class SVGL
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("category")]
            [JsonConverter(typeof(MyApiClients.CategoryBaseConverter))]
            public CategoryBase Category { get; set; }

            [JsonPropertyName("route")]
            [JsonConverter(typeof(MyApiClients.ThemeBaseConverter))]
            public ThemeBase Route { get; set; }

            [JsonPropertyName("wordmark")]
            [JsonConverter(typeof(MyApiClients.ThemeBaseConverter))]
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

        // Old Api Client from SVG.API.Demo Project
        public class MyApiClients
        {
            private static readonly HttpClient _httpClient = new HttpClient();
            public static string SVGLBaseURL = "https://api.svgl.app";

            public class CategoryBaseConverter : JsonConverter<CategoryBase>
            {
                public override CategoryBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                {
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        string category = reader.GetString();
                        return new CategoryString(category);
                    }
                    else if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        var categories = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                        return new CategoryArray(categories);
                    }
                    throw new JsonException("Invalid JSON for CategoryBase");
                }

                public override void Write(Utf8JsonWriter writer, CategoryBase value, JsonSerializerOptions options)
                {
                    switch (value)
                    {
                        case CategoryString categoryString:
                            writer.WriteStringValue(categoryString.Category);
                            break;

                        case CategoryArray categoryArray:
                            JsonSerializer.Serialize(writer, categoryArray.Categories, options);
                            break;

                        default:
                            throw new InvalidOperationException("Unknown CategoryBase type");
                    }
                }
            }

            // ThemeBaseConverter: Used in order to make sure, the Json Deserialization deserialize Route Type/Property Properly, since Route is Discrimanating/Union Type (of string and object).
            public class ThemeBaseConverter : JsonConverter<ThemeBase>
            {
                public override ThemeBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                {
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        // Deserialize as ThemeString
                        string themeString = reader.GetString();
                        return new ThemeString(themeString);
                    }
                    else if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        // Deserialize (Reading) as ThemeObject
                        SVGThemes themeObject = JsonSerializer.Deserialize<SVGThemes>(ref reader, options);
                        return new ThemeObject(themeObject);
                    }

                    throw new JsonException("Invalid JSON for RouteBase");
                }

                public override void Write(Utf8JsonWriter writer, ThemeBase value, JsonSerializerOptions options)
                {
                    switch (value)
                    {
                        case ThemeString routeString:
                            writer.WriteStringValue(routeString.Route);
                            break;

                        case ThemeObject routeObject:
                            JsonSerializer.Serialize(writer, routeObject.Route, options);
                            break;

                        default:
                            throw new InvalidOperationException("Unknown RouteBase type");
                    }
                }
            }

            public async Task<List<SVGL>> GetSVGs(string query)
            {
                HttpResponseMessage response = await _httpClient.GetAsync(SVGLBaseURL + "?search=" + query);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new ThemeBaseConverter(), new CategoryBaseConverter() } };
                var parsedData = JsonSerializer.Deserialize<List<SVGL>>(data, options);
                return parsedData!;
            }

            public async Task<List<SVGL>> GetAllSVGs()
            {
                HttpResponseMessage response = await _httpClient.GetAsync(SVGLBaseURL);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new ThemeBaseConverter(), new CategoryBaseConverter() } };
                var parsedData = JsonSerializer.Deserialize<List<SVGL>>(data, options);
                return parsedData!;

            }

            public async Task<string> GetSVGContent(string url)
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                string data = await response.Content.ReadAsStringAsync();
                return data;
            }
        }


    }

}
