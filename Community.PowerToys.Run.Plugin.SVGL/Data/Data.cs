using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Community.PowerToys.Run.Plugin.SVGL.Utils;
using Wox.Plugin.Logger;


namespace Community.PowerToys.Run.Plugin.SVGL.Data;

public interface IMyApiClient
{
    Task<List<Svgl>> GetSvgFromSource(string query);
    Task<List<Svgl>> GetAllSvGs();
    Task<string> GetSvgContent(string url);
}

public class MyApiClients : IMyApiClient
{
    private static readonly HttpClient HttpClient = new();
    private const string Pattern = @"library/(.*?)(\.|$)";
    private static readonly Regex SvgRegex = new(Pattern, RegexOptions.Compiled);

    public async Task<List<Svgl>> GetSvgFromSource(string query)
    {
        var response = await HttpClient.GetAsync(Constants.ApiBaseUrl + "?search=" + query);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, Converters = { new ThemeBaseConverter(), new CategoryBaseConverter() }
        };
        var parsedData = JsonSerializer.Deserialize<List<Svgl>>(data, options);
        return parsedData!;
    }

    public async Task<List<Svgl>> GetAllSvGs()
    {
        var response = await HttpClient.GetAsync(Constants.ApiBaseUrl);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, Converters = { new ThemeBaseConverter(), new CategoryBaseConverter() }
        };
        var parsedData = JsonSerializer.Deserialize<List<Svgl>>(data, options);
        return parsedData!;
    }

    public async Task<string> GetSvgContent(string url)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url), $"URL cannot be empty or empty.");

        var match = SvgRegex.Match(url);

        if (!match.Success)
            throw new ArgumentException($"The URL does not contain a valid SVG identifier.", nameof(url));

        var extractedSvgName = match.Groups[1].Value;
        var fullUrl = $"{Constants.SvglBaseUrl}{extractedSvgName}.svg";
        Log.Info($"Fixed URL: {fullUrl}", GetType());

        var response = await HttpClient.GetAsync(fullUrl);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadAsStringAsync();
        return data;
    }
}