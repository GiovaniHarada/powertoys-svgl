using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wox.Plugin.Logger;


namespace Community.PowerToys.Run.Plugin.SVGL
{
    public class MyApiClients
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public static string SVGLBaseURL = "https://api.svgl.app";

        public async Task<List<SVGL>> GetSVGFromSource(string query)
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
            string baseURL = "https://svgl.app/library/";
            string pattern = @"library/(.*?)(\.|$)";

            Match match = Regex.Match(url, pattern);

            string extractedSVGName = match.Success ? match.Groups[1].Value : ""; //Todo: Throw Exception, if match.Success Fails
            string fixedURL = baseURL + extractedSVGName + ".svg";
            Log.Info($"Fixed URL: {fixedURL}", GetType());

            HttpResponseMessage response = await _httpClient.GetAsync(fixedURL);
            response.EnsureSuccessStatusCode();
            string data = await response.Content.ReadAsStringAsync();
            return data;
        }
    }

}