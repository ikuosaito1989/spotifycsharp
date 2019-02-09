using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using spotifycsharp.model;

namespace spotifycsharp
{
    class Program
    {

        public static async Task Main(string[] args)
        {
            var clientId = "";
            var clientSecret = "";
            var spotify = new SpotifyApi(clientId, clientSecret);
            var searchArtists = await spotify.GetSearchToken("ariana grande");
        }
    }

    public class SpotifyApi
    {
        private static HttpClient _client = new HttpClient();
        private string _accessToken;
        public SpotifyApi(string clientId, string clientSecret)
        {
            _accessToken = GetSpotifyToken(clientId, clientSecret).Result;
        }

        private async Task<string> GetSpotifyToken(string clientId, string clientSecret)
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

        public async Task<SearchArtists> GetSearchToken(string query, string limit = "1", string offset = "0")
        {
            var parame = new Dictionary<string, string>()
            {
                { "q", query },
                { "type", "artist" },
                { "market", "US" },
                { "limit", limit },
                { "offset", offset },
            };
            var parameters = await new FormUrlEncodedContent(parame).ReadAsStringAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/search?{parameters}");
            request.Headers.Add("ContentType", "application/json");
            request.Headers.Add("Authorization", $"Bearer {this._accessToken}");
            var response = await _client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SearchArtists>(json);
        }
    }
}
