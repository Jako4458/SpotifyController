using SpotifyController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyController.Data
{
    static public class UserRepo
    {
        static public User TestUser = new User { Name = "Test" };

        static public Dictionary<string, User> Users = new Dictionary<string, User>();
    }
}
