using System.Collections.Generic;
using System.Windows.Input;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.SVGL.UnitTests.Test_Data;

public static class QueryTestData
{
    private static Result CreateBasicResult(string title, string subTitle, int score, Svgl contextData = null)
    {
        return new Result
        {
            Title = title,
            SubTitle = subTitle,
            IcoPath = "Images\\svgl.dark.png",
            Score = score,
            ContextData = contextData,
            Action = _ => true
        };
    }

    private static Result CreateNoActionResult(string title, string subTitle, int score)
    {
        return new Result
        {
            Title = title,
            SubTitle = subTitle,
            IcoPath = "Images\\svgl.dark.png",
            Score = score,
            Action = _ => false
        };
    }

    private static Svgl CreateBasicSvgl(int id, string title, CategoryBase category, ThemeBase route, string url,
        ThemeBase wordmark = null)
    {
        return new Svgl
        {
            Id = id,
            Title = title,
            Category = category,
            Route = route,
            Wordmark = wordmark,
            Url = url
        };
    }

    public static List<Result> GetGoogleRelatedResults()
    {
        var googleContext = CreateBasicSvgl(
            1,
            "Google",
            TestConstants.ExpectedCategoryString("Google"),
            TestConstants.ExpectedRouteString("https://svgl.app/library/google.svg"),
            "https://google.com",
            TestConstants.ExpectedWordmarkString("https://svgl.app/library/google-wordmark.svg")
        );

        return new List<Result>
        {
            CreateBasicResult("Google", TestConstants.ExpectedCategoryString("Google").ToString(), 100, googleContext),
            CreateBasicResult("Google Drive", TestConstants.ExpectedCategoryString("Google").ToString(), 99,
                googleContext),
            CreateBasicResult("Google Idx", $"Software, {TestConstants.ExpectedCategoryString("Google")}", 98,
                googleContext),
            CreateBasicResult("Google PaLM", $"AI, {TestConstants.ExpectedCategoryString("Google")}", 97,
                googleContext),
            CreateBasicResult("Google Colaboratory", $"{TestConstants.ExpectedCategoryString("Google")}, Software", 96,
                googleContext)
        };
    }

    public static List<Result> GetCloudflareResults()
    {
        return new List<Result>
        {
            CreateBasicResult(
                "Cloudflare Workers",
                TestConstants.ExpectedCategoryString("Software").ToString(),
                100,
                CreateBasicSvgl(
                    179,
                    "Cloudflare Workers",
                    TestConstants.ExpectedCategoryString("Software"),
                    TestConstants.ExpectedRouteString("https://svgl.app/library/cloudflare-workers.svg"),
                    "https://workers.cloudflare.com/"
                )
            ),
            CreateBasicResult(
                "Cloudflare",
                TestConstants.ExpectedCategoryString("Software").ToString(),
                99,
                CreateBasicSvgl(
                    180,
                    "Cloudflare",
                    TestConstants.ExpectedCategoryString("Software"),
                    TestConstants.ExpectedRouteString("https://svgl.app/library/cloudflare.svg"),
                    "https://www.cloudflare.com/"
                )
            )
        };
    }

    public static List<Result> GetNoResultsFoundData(string query)
    {
        return
        [
            CreateNoActionResult(TestConstants.NoResultFoundMessage, TestConstants.NoResultFoundSubTitle(query), 100),
            CreateBasicResult(TestConstants.RequestLogo, "Request a Logo on SVGL's Repository", 0),
            CreateBasicResult(TestConstants.SubmitLogo, "Submit a Logo on SVGL's Repository", 0)
        ];
    }

    public static List<Svgl> GetSampleLogos()
    {
        return
        [
            CreateBasicSvgl(
                1,
                "Google",
                new CategoryString("Technology"),
                new ThemeString("google.svg"),
                "https://google.com",
                new ThemeString("google-wordmark.svg")
            ),
            CreateBasicSvgl(
                2,
                "Vercel",
                new CategoryArray(["Hosting", "Development"]),
                new ThemeObject(new SvgThemes
                {
                    Light = "vercel.svg",
                    Dark = "vercel-dark.svg"
                }),
                "https://vercel.com",
                new ThemeObject(new SvgThemes
                {
                    Light = "vercel-wordmark.svg",
                    Dark = "vercel-wordmark-dark.svg"
                })
            ),
            CreateBasicSvgl(
                3,
                "GitHub",
                new CategoryString("Development"),
                new ThemeObject(new SvgThemes
                {
                    Light = "github.svg",
                    Dark = "github-dark.svg"
                }),
                "https://github.com",
                new ThemeString("github-wordmark.svg")
            )
        ];
    }

    private static ContextMenuResult CreateContextMenuResult(
        string title,
        string glyph,
        Key acceleratorKey,
        ModifierKeys modifiers = ModifierKeys.None)
    {
        return new ContextMenuResult
        {
            Title = title,
            Glyph = glyph,
            AcceleratorKey = acceleratorKey,
            AcceleratorModifiers = modifiers
        };
    }

    public static List<ContextMenuResult> GetDefaultContextMenu()
    {
        return
        [
            CreateContextMenuResult(Constants.CopySvgLogoMessage, "\xE8C8", Key.Enter)
        ];
    }

    public static List<ContextMenuResult> GetThemedContextMenu()
    {
        return
        [
            CreateContextMenuResult(Constants.CopyLightThemeSvgLogoMessage, "\xE706", Key.Enter),
            CreateContextMenuResult(Constants.CopyDarkThemeSvgLogoMessage, "\xE708", Key.Enter, ModifierKeys.Control)
        ];
    }

    public static List<ContextMenuResult> GetFullContextMenu()
    {
        return
        [
            CreateContextMenuResult(Constants.CopyLightThemeSvgLogoMessage, "\xE706", Key.Enter),
            CreateContextMenuResult(Constants.CopyDarkThemeSvgLogoMessage, "\xE708", Key.Enter, ModifierKeys.Control),
            CreateContextMenuResult(Constants.CopyLightThemeSvgWordmarkMessage, "\xE8D2", Key.Enter,
                ModifierKeys.Shift),
            CreateContextMenuResult(Constants.CopyDarkThemeSvgWordmarkMessage, "\xE8D3", Key.Enter,
                ModifierKeys.Control | ModifierKeys.Shift)
        ];
    }

    public static List<ContextMenuResult> GetThemedWordmarkContextMenu()
    {
        return
        [
            CreateContextMenuResult(Constants.CopySvgLogoMessage, "\xE8C8", Key.Enter),
            CreateContextMenuResult(Constants.CopyLightThemeSvgWordmarkMessage, "\xE8D2", Key.Enter,
                ModifierKeys.Shift),
            CreateContextMenuResult(Constants.CopyDarkThemeSvgWordmarkMessage, "\xE8D3", Key.Enter,
                ModifierKeys.Control | ModifierKeys.Shift)
        ];
    }

    public static Svgl GetDefaultContextData()
    {
        return CreateBasicSvgl(
            3,
            "GitHub",
            new CategoryString("Development"),
            new ThemeString("github.svg"),
            "https://github.com");
    }

    public static Svgl GetThemedContextData()
    {
        return CreateBasicSvgl(
            449,
            "Clerk",
            new CategoryArray(["Software", "Authentication"]),
            new ThemeObject(new SvgThemes
            {
                Light = "https://svgl.app/library/clerk-light.svg",
                Dark = "https://svgl.app/library/clerk-dark.svg"
            }),
            "https://clerk.com/");
    }

    public static Svgl GetFullContextData()
    {
        return CreateBasicSvgl(
            2,
            "Vercel",
            new CategoryArray(["Hosting", "Development"]),
            new ThemeObject(new SvgThemes
            {
                Light = "vercel.svg",
                Dark = "vercel-dark.svg"
            }),
            "https://vercel.com",
            new ThemeObject(new SvgThemes
            {
                Light = "vercel-wordmark.svg",
                Dark = "vercel-wordmark-dark.svg"
            })
        );
    }

    public static Svgl GetThemedWordmarkContextData()
    {
        return CreateBasicSvgl(
            438,
            "tRPC",
            new CategoryString("Framework"),
            new ThemeString("https://svgl.app/library/trpc.svg"),
            "https://trpc.io/",
            new ThemeObject(new SvgThemes
            {
                Light = "https://svgl.app/library/trpc_wordmark_light.svg",
                Dark = "https://svgl.app/library/trpc_wordmark_dark.svg"
            })
        );
    }
}