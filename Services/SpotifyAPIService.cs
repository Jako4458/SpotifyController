using SpotifyController.Exceptions;
using SpotifyController.Interfaces;
using SpotifyController.Model;
using SpotifyController.Model.SpotifyAPI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyController.Services
{
    public class SpotifyAPIService: ISpotifyAPIService
    {
        public SpotifyAPIService(HttpClient httpClient, IConfiguration config)
        {
            _clientId = config["SpotifyClient:ID"];
            _clientSecret = config["SpotifyClient:Secret"];
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.spotify.com/v1";

        public static string RedirectUri= "https://localhost/api/user/authorizeSpotify";


        private readonly string _clientId;
        private readonly string _clientSecret;

        // ShowDialog should be true for final/publish build
        public string AuthorizeUser(string session_id, string redirect_uri="/", bool showDialog = false)
        {
            string state = $"session_id={session_id};redirect_uri={redirect_uri}";
            List<string> authScopes = new List<string>()
            {
                "user-read-recently-played",
                "user-read-playback-state",
                "user-modify-playback-state",
                "user-read-currently-playing",
                "user-read-private",
                "user-library-read",
                "playlist-read-collaborative",
                "streaming",
            };

            string scopesAsString = authScopes.Select(scope => scope == authScopes[authScopes.Count-1] ? scope : $"{scope}%20").Aggregate((acc, scope) => $"{acc}{scope}");
            //string authPath = $"https://accounts.spotify.com/authorize?client_id={_clientId}&response_type=code&redirect_uri={RedirectUri}&scope={scopesAsString}&show_dialog={showDialog}";
            string authPath = $"https://accounts.spotify.com/authorize?client_id={_clientId}&response_type=code&redirect_uri={RedirectUri}&scope={scopesAsString}&show_dialog={showDialog}&state={state}";

            return authPath;
        }

        public async Task<(bool, string)> GetToken(SpotifySession session)
        {
            if (session is null)
                throw new SpotifyNotConnectedException();


            var dict = new Dictionary<string, string>();
            dict.Add("client_id", _clientId);
            dict.Add("client_secret", _clientSecret);
            dict.Add("redirect_uri", RedirectUri);

            if (session.SpotifyToken.RefreshToken != null) 
            { 
                dict.Add("grant_type", "refresh_token");
                dict.Add("refresh_token", session.SpotifyToken.RefreshToken);
            } else
            {
                dict.Add("grant_type", "authorization_code");
                dict.Add("code", session.SpotifyToken.Code);
            }

            string url = "https://accounts.spotify.com/api/token";
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(dict) };
            var res = await _httpClient.SendAsync(req);
            string resContent = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                return (false, resContent);

            string resString = await res.Content.ReadAsStringAsync();
            var TokenObject = JsonConvert.DeserializeObject<TokenResponse>(resString);

            session.SpotifyToken.AccessToken = TokenObject.access_token;
            session.SpotifyToken.AccessTokenExpiration = DateTime.Now.AddSeconds(TokenObject.expires_in);

            if (session.SpotifyToken.RefreshToken == null)
                session.SpotifyToken.RefreshToken = TokenObject.refresh_token;

            return (true, resContent);
        }

        private async Task<(bool, T, string)> SendAPIRequest<T>(SpotifySession session, string method, string url, HttpContent content=null)
        {
            if (session is null)
                throw new SpotifyNotConnectedException();

            // if expired and refresh not succesfull return false
            if (session.SpotifyToken.AccessTokenExpiration < DateTime.Now)
            {
                (bool succes, string tokenContent) = await GetToken(session);
                if (!succes)
                    return (false, default, $"Could not get Token: GetToken respone = '{tokenContent}'");
            }

            HttpMethod requestMethod;

            switch (method.ToUpper())
            {
                case "GET":
                    requestMethod = HttpMethod.Get;
                    break;
                case "POST":
                    requestMethod = HttpMethod.Post;
                    break;
                case "PUT":
                    requestMethod = HttpMethod.Put;
                    break;
                case "DELETE":
                    requestMethod = HttpMethod.Delete;
                    break;
                default:
                    return (false, default, $"Invalid HTTP Method! - '{method.ToUpper()}'");
            }

            var request = new HttpRequestMessage(requestMethod, $"{_baseUrl}{url}");
            request.Headers.Add("Authorization", $"Bearer {session.SpotifyToken.AccessToken}");

            // if not GET add content/body
            if (requestMethod != HttpMethod.Get)
                request.Content = content;

            var response = await _httpClient.SendAsync(request);
            var resContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return (false, default, resContent);

            try
            {
                return (true, JsonConvert.DeserializeObject<T>(resContent), resContent);
            }
            catch (JsonReaderException e)
            {
                // for debugging - raw content will still be returned when json deserialization fails
                return (true, default, resContent);
            }
            //return (true, default, resContent);
        }

        /////////////////////////////////////////////////////////////////
        
        public async Task<(bool, Playlist, string)> GetPlaylist(SpotifySession session, string playlistId)
        {
            string url = $"/playlists/{playlistId}";

            var response = await SendAPIRequest<Playlist>(session, "GET", url);
            return response;
        }

         public async Task<(bool, PlaylistTracks, string)> GetPlaylistTracks(SpotifySession session, string playlistId, int offset=0, int limit=20)
        {
            string url = $"/playlists/{playlistId}/tracks?offset={offset}&limit={limit}";

            var response = await SendAPIRequest<PlaylistTracks>(session, "GET", url);
            return response;
        }       

        public async Task<(bool, Track, string)> GetTrack(SpotifySession session, string trackId)
        {
            string url = $"/tracks/{trackId}";

            var response = await SendAPIRequest<Track>(session, "GET", url);
            return response;
        }
        public async Task<(bool, string)> QueueTrack(SpotifySession session, string trackId)
        {
            string url = $"/me/player/queue?uri=spotify%3Atrack%3A{trackId}";

            var response = await SendAPIRequest<string>(session, "POST", url);
            return (response.Item1, response.Item2);
        }

        public async Task<(bool, Search, string)> Search(SpotifySession session, string query)
        {
            string url = "/search?q=track:" + query + "&type=album,artist,playlist,track,show,episode&market=dk";

            var response = await SendAPIRequest<Search>(session, "GET", url);
            return response;
        }

        public async Task<(bool, Playlists, string)> GetCurrentUsersPlaylists(SpotifySession session)
        {
            string url = "/me/playlists";

            //var response = await SendAPIRequest<List<Playlist>>(user, "GET", url);
            var response = await SendAPIRequest<Playlists>(session, "GET", url);
            return response;
        }
        
    }
}
