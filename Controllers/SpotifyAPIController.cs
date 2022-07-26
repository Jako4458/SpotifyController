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
        public IActionResult Authorize([FromQuery] string session_id, [FromQuery] string redirect_uri="/")
        {
            return Redirect(_spotifyAPIService.AuthorizeUser(session_id, redirect_uri));
        }

        [HttpGet]
        public async Task<IActionResult> AccessToken([FromQuery] string session_id, [FromQuery] string redirect_uri="/")
        {
            if (session_id == null)
                return Unauthorized("Invalid Session!");

            User user;
            bool UserFound = UserRepo.Users.TryGetValue(session_id, out user);
            
            try
            {
                (bool succes, string tokenContent) = await _spotifyAPIService.GetToken(user.SpotifySession);
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
        public async Task<IActionResult> QueueTrack([FromHeader] string SessionId, [FromHeader] string SpotifySessionId, [FromQuery] string trackId, [FromQuery] bool raw=false)
        {
            if (SessionId == null)
                return Unauthorized("Invalid Session!");
            if (trackId == null)
                return NotFound("No trackId supplied!");

            SpotifySession spotifySession;
            bool sessionFound;

            (sessionFound, spotifySession) = SpotifySessionRepo.getPublicSpotifySession(SpotifySessionId);

            if (!sessionFound)
            {
                User user;
                bool userFound = UserRepo.Users.TryGetValue(SessionId, out user);

                if (!userFound)
                    return Unauthorized("Not connected to spotify");
                else if (SpotifySessionId is null || SpotifySessionId == "" || SpotifySessionId == "undefined"  || user.SpotifySession.Id == SpotifySessionId)
                {
                    sessionFound = true;
                    spotifySession = user.SpotifySession;
                }
                else
                    return Unauthorized("User cannot access the requested session");
            }


            bool succesfullyAdded;
            string responseContent;

            try
            {
                (succesfullyAdded, responseContent) = await _spotifyAPIService.QueueTrack(spotifySession, trackId);
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
        public async Task<IActionResult> GetPlaylists([FromHeader] string SessionId, [FromQuery] bool raw=false)
        {
            if (SessionId == null)
                return Unauthorized("Invalid Session!");

            User user;
            bool userFound = UserRepo.Users.TryGetValue(SessionId, out user);

            if (!userFound)
                return Unauthorized("Not connected to spotify");
            
            bool succesfullyAdded;
            Playlists playlists;
            string responseContent;

            try
            {
                (succesfullyAdded, playlists, responseContent) = await _spotifyAPIService.GetCurrentUsersPlaylists(user.SpotifySession);
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
        public async Task<IActionResult> GetPlaylist([FromHeader] string SessionId, [FromQuery] string playlistId, [FromQuery] bool raw = false)
        {
            if (SessionId == null)
                return Unauthorized("Invalid Session!");
            if (playlistId == null)
                return NotFound("No playlistId supplied!");

            User user;
            bool userFound;

            userFound = UserRepo.Users.TryGetValue(SessionId, out user);


            if (!userFound)
                return Unauthorized("Not connected to spotify");
            
            bool succesfullyAdded;
            Playlist playlist;
            string responseContent;

            try
            {
                (succesfullyAdded, playlist, responseContent) = await _spotifyAPIService.GetPlaylist(user.SpotifySession, playlistId);
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

         [HttpGet]
        public async Task<IActionResult> GetPlaylistTracks([FromHeader] string SessionId, [FromQuery] string playlistId, [FromQuery] int offset=0, [FromQuery] int limit=20, [FromQuery] bool raw = false)
        {
            if (SessionId == null)
                return Unauthorized("Invalid Session!");
            if (playlistId == null)
                return NotFound("No playlistId supplied!");

            User user;
            bool userFound;

            userFound = UserRepo.Users.TryGetValue(SessionId, out user);


            if (!userFound)
                return Unauthorized("Not connected to spotify");
            
            bool succesfullyAdded;
            PlaylistTracks playlistTracks;
            string responseContent;

            try
            {
                (succesfullyAdded, playlistTracks, responseContent) = await _spotifyAPIService.GetPlaylistTracks(user.SpotifySession, playlistId, offset, limit);
            }
            catch (SpotifyNotConnectedException)
            {
                return Unauthorized(new {Message = "Not Connected to spotify!"});
            }

            if (!succesfullyAdded)
                return NotFound("Playlist could not be found!");
            if (raw) 
                return Ok(responseContent);
            
            return Ok(playlistTracks);
        }


        public async Task<IActionResult> Search([FromHeader] string SessionId, [FromHeader] string SpotifySessionId, [FromQuery] string query, [FromQuery] bool raw = false)
        {
            if (SessionId == null)
                return Unauthorized("Invalid Session!");
            if (query == null)
                return NotFound("No search query found");
            
            SpotifySession spotifySession;
            bool sessionFound;

            (sessionFound, spotifySession) = SpotifySessionRepo.getPublicSpotifySession(SpotifySessionId);

            if (!sessionFound)
            {
                User user;
                bool userFound = UserRepo.Users.TryGetValue(SessionId, out user);

                if (!userFound)
                    return Unauthorized("Not connected to spotify");
                else if (SpotifySessionId is null || SpotifySessionId == "" || SpotifySessionId == "undefined"  || user.SpotifySession.Id == SpotifySessionId)
                {
                    sessionFound = true;
                    spotifySession = user.SpotifySession;
                }
                else
                    return Unauthorized("User cannot access the requested session");
            }

            bool succesfullyAdded;
            Search searchResult;
            string responseContent;

            try
            {
                (succesfullyAdded, searchResult, responseContent) = await _spotifyAPIService.Search(spotifySession, query);
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
