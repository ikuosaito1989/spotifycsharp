using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace spotifycsharp
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var clientId = "";
            var clientSecret = "";
            var token = await new SpotifyApi().GetSpotifyToken(clientId, clientSecret);
        }
    }

    public class SpotifyApi
    {
        private static HttpClient _client = new HttpClient();
        public async Task<string> GetSpotifyToken(string clientId, string clientSecret)
        {
            var clientKey = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            // Create a request.
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
            });
            request.Headers.Add("Authorization", "Basic " + clientKey);
            // Post
            var response = await _client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<dynamic>(json);
            return (string)obj.access_token;
        }
    }
}
