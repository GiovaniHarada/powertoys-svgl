using System.Collections.Generic;
using Wox.Plugin;


namespace Community.PowerToys.Run.Plugin.SVGL.UnitTests;
public static class QueryTestData
{
    public static List<Result> GoogleData()
    {
        List<Result> results = new List<Result>{new Result
        {
            Title = "Google",
            SubTitle = TestContants.expectedCategoryString("Google").ToString(),
            IcoPath = "Images\\svgl.dark.png",
            Score = 100,
            ContextData = new SVGL
            {
                Id = 1,
                Title = "Google",
                Category = TestContants.expectedCategoryString("Google"),
                Route = TestContants.expectedRouteString("https://svgl.app/library/google.svg"),
                Wordmark = TestContants.expectedWordmarkString("https://svgl.app/library/google-wordmark.svg"),
                Url = "https://google.com"
            },
            Action = _ => true
        },
        new Result{
            Title = "Google Drive",
            SubTitle = TestContants.expectedCategoryString("Google").ToString(),
            IcoPath = "Images\\svgl.dark.png",
            Score = 99,
            ContextData = new SVGL
            {
                Id = 1,
                Title = "Google",
                Category = TestContants.expectedCategoryString("Google"),
                Route = TestContants.expectedRouteString("https://svgl.app/library/google.svg"),
                Wordmark = TestContants.expectedWordmarkString("https://svgl.app/library/google-wordmark.svg"),
                Url = "https://google.com"
            },
            Action = _ => true
        },
        new Result{
            Title = "Google Idx",
            SubTitle = $"Software, {TestContants.expectedCategoryString("Google").ToString()}",
            IcoPath = "Images\\svgl.dark.png",
            Score = 98,
            ContextData = new SVGL
            {
                Id = 1,
                Title = "Google",
                Category = TestContants.expectedCategoryString("Google"),
                Route = TestContants.expectedRouteString("https://svgl.app/library/google.svg"),
                Wordmark = TestContants.expectedWordmarkString("https://svgl.app/library/google-wordmark.svg"),
                Url = "https://google.com"
            },
            Action = _ => true
        },
        new Result{
            Title = "Google PaLM",
            SubTitle = $"AI, {TestContants.expectedCategoryString("Google").ToString()}",
            IcoPath = "Images\\svgl.dark.png",
            Score = 97,
            ContextData = new SVGL
            {
                Id = 1,
                Title = "Google",
                Category = TestContants.expectedCategoryString("Google"),
                Route = TestContants.expectedRouteString("https://svgl.app/library/google.svg"),
                Wordmark = TestContants.expectedWordmarkString("https://svgl.app/library/google-wordmark.svg"),
                Url = "https://google.com"
            },
            Action = _ => true
        },
        new Result{
            Title = "Google Colaboratory",
            SubTitle = $"{TestContants.expectedCategoryString("Google").ToString()}, Software",
            IcoPath = "Images\\svgl.dark.png",
            Score = 96,
            ContextData = new SVGL
            {
                Id = 1,
                Title = "Google",
                Category = TestContants.expectedCategoryString("Google"),
                Route = TestContants.expectedRouteString("https://svgl.app/library/google.svg"),
                Wordmark = TestContants.expectedWordmarkString("https://svgl.app/library/google-wordmark.svg"),
                Url = "https://google.com"
            },
            Action = _ => true
        }};

        return results;
    }

    public static List<Result> FlareData()
    {
        List<Result> results = new List<Result>{new Result
        {
            Title = "Cloudflare Workers",
            SubTitle = TestContants.expectedCategoryString("Software").ToString(),
            IcoPath = "Images\\svgl.dark.png",
            Score = 100,
            ContextData = new SVGL
            {
                Id = 179,
                Title = "Cloudflare Workers",
                Category = TestContants.expectedCategoryString("Software"),
                Route = TestContants.expectedRouteString("https://svgl.app/library/cloudflare-workers.svg"),
                Url = "https://workers.cloudflare.com/"
            },
            Action = _ => true
        },
        new Result{
            Title = "Cloudflare",
            SubTitle = TestContants.expectedCategoryString("Software").ToString(),
            IcoPath = "Images\\svgl.dark.png",
            Score = 99,
            ContextData = new SVGL
            {
                Id = 180,
                Title = "Cloudflare",
                Category = TestContants.expectedCategoryString("Software"),
                Route = TestContants.expectedRouteString("https://svgl.app/library/cloudflare.svg"),
                Url = "https://www.cloudflare.com/"
            },
            Action = _ => true
        },
       };

        return results;
    }

    public static List<Result> NoResultFoundData(string query)
    {
        List<Result> results = new List<Result>
        {
            new Result
            {
                Title = TestContants.NoResultFoundMessage,
                SubTitle = TestContants.NoResultFoundSubTitle(query),
                IcoPath = "Images\\svgl.dark.png",
                Score = 100,
                Action = _ => false
            },
            new Result
            {
                Title = TestContants.RequestLogo,
                SubTitle = "Request a Logo on SVGL's Repository",
                IcoPath = "Images\\svgl.dark.png",
                Action = _ => true
            },
            new Result
            {
                Title = TestContants.SubmitLogo,
                SubTitle = "Submit a Logo on SVGL's Repository",
                IcoPath = "Images\\svgl.dark.png",
                Action = _ => true
            }
        };
        return results;
    }

    public static List<SVGL> GetSampleSvgs()
    {
        return new List<SVGL>
    {
        // Case 1: Simple case - ThemeString + CategoryString
        new SVGL
        {
            Id = 1,
            Title = "Google",
            Category = new CategoryString("Technology"),
            Route = new ThemeString("google.svg"),
            Wordmark = new ThemeString("google-wordmark.svg"),
            Url = "https://google.com"
        },

        // Case 2: Theme with light/dark + Array of categories
        new SVGL
        {
            Id = 2,
            Title = "Vercel",
            Category = new CategoryArray(["Hosting", "Development"]),
            Route = new ThemeObject(new SVGThemes
            {
                Light = "vercel.svg",
                Dark = "vercel-dark.svg"
            }),
            Wordmark = new ThemeObject(new SVGThemes
            {
                Light = "vercel-wordmark.svg",
                Dark = "vercel-wordmark-dark.svg"
            }),
            Url = "https://vercel.com"
        },

        // Case 3: Mixed theme types + single category
        new SVGL
        {
            Id = 3,
            Title = "GitHub",
            Category = new CategoryString("Development"),
            Route = new ThemeObject(new SVGThemes
            {
                Light = "github.svg",
                Dark = "github-dark.svg"
            }),
            Wordmark = new ThemeString("github-wordmark.svg"),
            Url = "https://github.com"
        }
    };
    }
}
