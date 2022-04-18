using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyController.Data;
using SpotifyController.Model;
using SpotifyController.Model.SpotifyAPI;
using SpotifyController.Services;
using Microsoft.AspNetCore.Mvc;
using SpotifyController.Exceptions;

namespace LogenV2React.Controllers
{
    [Route("API/[controller]/[action]")]
    [ApiController]
    public class SpotifyAPIController : ControllerBase
    {

        private SpotifyAPIService _spotifyAPIService;

        public SpotifyAPIController(SpotifyAPIService spotifyAPIService)
        {
            _spotifyAPIService = spotifyAPIService;
        }

        [HttpGet]
        public IActionResult Authorize([FromQuery] string redirect_uri="/")
        {
            return Redirect(_spotifyAPIService.AuthorizeUser(redirect_uri));
        }

        [HttpGet]
        public async Task<IActionResult> AccessToken([FromQuery] string redirect_uri="/")
        {
            User user = UserRepo.TestUser;

            try
            {
                (bool succes, string tokenContent) = await _spotifyAPIService.GetToken(user);
                if (!succes)
                    return NotFound($"Could not get Token for user - Token response: {tokenContent}");
            }
            catch (SpotifyNotConnectedException)
            {
                return Redirect("/API/SpotifyAPI/Authorize");
            }

            return Redirect(redirect_uri);
        }
        
        [HttpPost]
        public async Task<IActionResult> QueueTrack([FromQuery] string trackId, [FromQuery] bool raw=false)
        {
            if (trackId == null)
                return NotFound("No trackId supplied!");

            User user = UserRepo.TestUser;
            bool succesfullyAdded;
            string responseContent;

            try
            {
                (succesfullyAdded, responseContent) = await _spotifyAPIService.QueueTrack(user, trackId);
            }
            catch (SpotifyNotConnectedException)
            {
                return Unauthorized("Not Connected to spotify!");
            }

            if (!succesfullyAdded)
                return NotFound("Song could not be added to queue!");
            if (raw)
                return Ok(responseContent);

            return Ok("Song succesfully added to queue!");
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPlaylists([FromQuery] bool raw=false)
        {
            User user = UserRepo.TestUser;
            
            bool succesfullyAdded;
            Playlists playlists;
            string responseContent;

            try
            {
                (succesfullyAdded, playlists, responseContent) = await _spotifyAPIService.GetCurrentUsersPlaylists(user);
            }
            catch (SpotifyNotConnectedException)
            {
                return Unauthorized(new {Message = "Not Connected to spotify!"});
            }

            if (!succesfullyAdded)
                return NotFound("Playlists could not be found!");
            if (raw)
                return Ok(responseContent);

            return Ok(playlists);
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaylist([FromQuery] string playlistId, [FromQuery] bool raw = false)
        {
            if (playlistId == null)
                return NotFound("No playlistId supplied!");

            User user = UserRepo.TestUser;
            
            bool succesfullyAdded;
            Playlist playlist;
            string responseContent;

            try
            {
                (succesfullyAdded, playlist, responseContent) = await _spotifyAPIService.GetPlaylist(user, playlistId);
            }
            catch (SpotifyNotConnectedException)
            {
                return Unauthorized(new {Message = "Not Connected to spotify!"});
            }

            if (!succesfullyAdded)
                return NotFound("Playlist could not be found!");
            if (raw) 
                return Ok(responseContent);
            
            return Ok(playlist);
        }

        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] bool raw = false)
        {
            if (query == null)
                return NotFound("No search query found");

            User user = UserRepo.TestUser;

            bool succesfullyAdded;
            Search searchResult;
            string responseContent;

            try
            {
                (succesfullyAdded, searchResult, responseContent) = await _spotifyAPIService.Search(user, query);
            }
            catch (SpotifyNotConnectedException)
            {
                return Unauthorized(new { Message = "Not Connected to spotify!" });
            }

            if (!succesfullyAdded)
                return NotFound("Search failed!");
            if (raw)
                return Ok(responseContent);
            
            return Ok(searchResult);
        }

    }
}
