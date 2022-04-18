using SpotifyController.Model;
using SpotifyController.Model.SpotifyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpotifyController.Interfaces
{
    interface ISpotifyAPIService
    {

        string AuthorizeUser(string redirect_uri, bool showDialog);
        Task<(bool, string)> GetToken(User user);
        Task<(bool, Playlist, string)> GetPlaylist(User user, string playlistId);
        Task<(bool, Playlists, string)> GetCurrentUsersPlaylists(User user);
        Task<(bool, Track, string)> GetTrack(User user, string trackId);
        Task<(bool, string)> QueueTrack(User user, string trackId);
        Task<(bool, Search, string)> Search(User user, string query);
    }
}
