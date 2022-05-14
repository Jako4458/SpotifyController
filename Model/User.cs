using System;

namespace SpotifyController.Model
{
    public class User
    {
        public string Name { get; set; }

        public SpotifySession SpotifySession { get; set; }
        public SpotifyAPIToken SpotifyToken => SpotifySession.SpotifyToken;
        public bool IsConnectedToSpotify => SpotifyToken != null;
    }
}
