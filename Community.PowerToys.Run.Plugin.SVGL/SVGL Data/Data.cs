using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wox.Plugin.Logger;


namespace Community.PowerToys.Run.Plugin.SVGL;

public class MyApiClients
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static readonly string pattern = @"library/(.*?)(\.|$)";
    private static readonly Regex SVGRegex = new Regex(pattern, RegexOptions.Compiled);

    public async Task<List<SVGL>> GetSVGFromSource(string query)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(Constants.APIBaseURL + "?search=" + query);
        response.EnsureSuccessStatusCode();
        string data = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new ThemeBaseConverter(), new CategoryBaseConverter() } };
        var parsedData = JsonSerializer.Deserialize<List<SVGL>>(data, options);
        return parsedData!;
    }

    public async Task<List<SVGL>> GetAllSVGs()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(Constants.APIBaseURL);
        response.EnsureSuccessStatusCode();
        string data = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new ThemeBaseConverter(), new CategoryBaseConverter() } };
        var parsedData = JsonSerializer.Deserialize<List<SVGL>>(data, options);
        return parsedData!;

    }

    public async Task<string> GetSVGContent(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException($"URL cannot be empty or empty.", nameof(url));
        }

        Match match = SVGRegex.Match(url);

        if (!match.Success)
        {
            throw new ArgumentException($"The URL does not contain a valid SVG identifier.", nameof(url));
        }

        string extractedSVGName = match.Groups[1].Value;
        string fullURL = $"{Constants.SVGLBaseURL}{extractedSVGName}.svg";
        Log.Info($"Fixed URL: {fullURL}", GetType());

        HttpResponseMessage response = await _httpClient.GetAsync(fullURL);
        response.EnsureSuccessStatusCode();
        string data = await response.Content.ReadAsStringAsync();
        return data;
    }
}