using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Community.PowerToys.Run.Plugin.SVGL.Data;
using Windows.ApplicationModel.DataTransfer;
using Wox.Plugin;
using static System.Net.Mime.MediaTypeNames;

namespace Community.PowerToys.Run.Plugin.SVGL.Utils;

internal static class Utils
{
    private static readonly MyApiClients ApiClient = new();

    private static bool CopyToClipboard(string value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        var dataPackage = new DataPackage();
        dataPackage.SetText(value);
        Clipboard.SetContent(dataPackage);
        Clipboard.Flush();

        return true;
    }

    private static bool CopySvgContent(string svg)
    {
        try
        {
            var content = Task.Run(async () => await ApiClient.GetSvgContent(svg)).Result;
            CopyToClipboard(content);
            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    public static ContextMenuResult CreateCopyMenuItem(string title, string glyph, string content, Key key,
        ModifierKeys modifier = ModifierKeys.None)
    {
        return GetContextMenuResult(new GetContextMenuResult
        {
            Title = title,
            Glyph = glyph,
            AcceleratorKey = key,
            AcceleratorModifiers = modifier,
            CopyContent = content
        });
    }

    public static ContextMenuResult GetContextMenuResult(GetContextMenuResult args)
    {
        return new ContextMenuResult
        {
            PluginName = "SVGL",
            Title = args.Title,
            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
            Glyph = args.Glyph,
            AcceleratorKey = args.AcceleratorKey,
            AcceleratorModifiers = args.AcceleratorModifiers,
            //Action = context => args.CustomAction != null ? args.CustomAction(context) : CopySVGContent(args.CopyContent)
            Action = context => args.CustomAction?.Invoke(context) ?? CopySvgContent(args.CopyContent)
        };
    }

    public static string CapitalizeFirstLetter(string input)
    {
        return string.IsNullOrEmpty(input) ? input : char.ToUpper(input[0]) + input[1..];
    }

    public static bool IsInternetAvailable()
    {
        try
        {
            using var client = new HttpClient();
            using var response = client.GetAsync("https://www.google.com/").Result;
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}