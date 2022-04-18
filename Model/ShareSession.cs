using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyController.Model
{
    public class ShareSession
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsPublic { get; set; }
        public string? Password { get; set; }
        public string Url { get; set; }
        
    }
}
