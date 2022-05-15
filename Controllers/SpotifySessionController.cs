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
    public class SpotifySessionController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetPublicSessions()
        {
            List<SharedSpotifySession> publicSpotifySessions = new List<SharedSpotifySession>();
            
            foreach (SpotifySession session in SpotifySessionRepo.PublicSessions)
            {
                publicSpotifySessions.Add(
                    new SharedSpotifySession()
                    {
                        Id = session.Id,
                        Name = session.Name,
                        HasPassword = session.Password != default,
                        Url = session.Url
                    }
                );

            }

            return Ok(publicSpotifySessions);
        }

    }
}
