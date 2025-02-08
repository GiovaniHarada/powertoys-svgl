using System.Threading.Tasks;
using System.Windows;

namespace Community.PowerToys.Run.Plugin.SVGL;

public class Utils
{
    private static readonly MyApiClients _apiClient = new MyApiClients();

    public static bool CopyToClipboard(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            Clipboard.SetText(value);
            return true;
        }
        return false;
    }

    public static bool CopySVGContent(string svg)
    {
        //var apiClient = new MyApiClients();
        var content = Task.Run(async () => await _apiClient.GetSVGContent(svg)).Result;
        CopyToClipboard(content);
        return true;
    }

}