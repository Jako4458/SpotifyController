﻿using System;
using System.Collections.Generic;
using SpotifyController.Data;
using SpotifyController.Model;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace SpotifyController.Controllers
{
    [ApiController]
    [Route("API/{controller}/{action}")]
    public class UserController : ControllerBase
    {
        //[HttpGet]
        //public List<User> AllUsers()
        //{
        //    return UserRepo.Users;
        //}

        [HttpGet]
        public IActionResult AuthorizeSpotify([FromQuery]string code, [FromQuery]string error, [FromQuery]string state)
        {
           // state is a query string
            string stateAsQueryString = state.Replace(";", "&");
            
            var stateQueries = HttpUtility.ParseQueryString(stateAsQueryString);
            string session_id = stateQueries.Get("session_id");
            string redirect_uri = stateQueries.Get("redirect_uri");

            if (error == null && code != null)
                UserRepo.Users.Add(session_id, new User() { 
                    spotifyAPIData = new APIData(code, stateAsQueryString)
                });
                //UserRepo.TestUser.spotifyAPIData = new APIData(code, stateAsQueryString);
    
            if (stateAsQueryString != null)
                return Redirect($"/API/SpotifyAPI/AccessToken?{stateAsQueryString}");

            return Redirect($"/API/SpotifyAPI/AccessToken");
        }

        [HttpGet]
        public IActionResult GetUser([FromHeader] string SessionId)
        {
            User user;
            bool userFound = UserRepo.Users.TryGetValue(SessionId, out user);

            if (!userFound)
                return NotFound("User Not Found");

            return Ok(user);
        }

        [HttpGet]
        public IActionResult Logout([FromQuery] string session_id)
        {
            User user;
            bool userFound = UserRepo.Users.TryGetValue(session_id, out user);

            if (!userFound)
                return NotFound("User Not Found!");

            UserRepo.Users.Remove(session_id);

            return Redirect("/");
        }
    }
}
