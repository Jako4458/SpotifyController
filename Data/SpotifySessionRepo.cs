using SpotifyController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyController.Data
{
    static public class SpotifySessionRepo
    {
        static private List<SpotifySession> _publicSessions = new List<SpotifySession>();
        static public (bool, SpotifySession) getPublicSpotifySession(string id)
        {
            foreach (var session in _publicSessions)
            {
                if (session.Id == id)
                {
                    if (!session.IsPublic)
                    {
                        // session expired
                        _publicSessions.Remove(session);
                        return (false, null);
                    }
                    else
                    {
                        return (true, session);
                    }
                }
            }

            // session not found
            return (false, null);
        }

        static public void addPublicSpotifySession(SpotifySession session)
        {
            _publicSessions.Add(session);
        }
    }
}
