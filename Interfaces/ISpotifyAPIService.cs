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

        string AuthorizeUser(string session_id, string redirect_uri, bool showDialog);
        Task<(bool, string)> GetToken(SpotifySession session);
        Task<(bool, Playlist, string)> GetPlaylist(SpotifySession session, string playlistId);
        Task<(bool, Playlists, string)> GetCurrentUsersPlaylists(SpotifySession session);
        Task<(bool, Track, string)> GetTrack(SpotifySession session, string trackId);
        Task<(bool, string)> QueueTrack(SpotifySession session, string trackId);
        Task<(bool, Search, string)> Search(SpotifySession session, string query);
    }
}
