using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyController.Model
{
    public class SpotifySession
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public SpotifyAPIToken SpotifyToken { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsPublic => (DateTime.Now < EndTime);

        private string _password;
        public string? Password {
            get { return IsPublic ? _password : null; }
            set { _password = value; }
        }

        public string Url { get; set; }
        
    }
}
