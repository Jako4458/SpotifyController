using System;
using System.Collections.Generic;
using SpotifyController.Data;
using SpotifyController.Model;
using Microsoft.AspNetCore.Mvc;

namespace SpotifyController.Controllers
{
    [ApiController]
    [Route("API/{controller}/{action}")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public List<User> AllUsers()
        {
            return UserRepo.Users;
        }

        [HttpGet]
        public IActionResult AuthorizeSpotify([FromQuery]string code, [FromQuery]string error, [FromQuery]string state)
        {
            if (error == null && code != null) { }
                UserRepo.TestUser.spotifyAPIData = new APIData(code, state);
    
            if (state != null)
                return Redirect($"/API/SpotifyAPI/AccessToken?redirect_uri={state}");

            return Redirect($"/API/SpotifyAPI/AccessToken");
        }
    }
}
