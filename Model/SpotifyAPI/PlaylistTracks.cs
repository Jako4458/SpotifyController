namespace SpotifyController.Model.SpotifyAPI
{
    public class PlaylistTracks
    {
        public string href { get; set; }
        public PlaylistTrack[] items { get; set; }
        public int limit { get; set; }
        public object next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }

}
