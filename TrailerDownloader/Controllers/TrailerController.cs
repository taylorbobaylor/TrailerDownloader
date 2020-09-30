using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrailerDownloader.Repositories;

namespace TrailerDownloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrailerController : ControllerBase
    {
        private readonly ITrailerRepository _trailerRepository;

        public TrailerController(ITrailerRepository trailerRepository)
        {
            _trailerRepository = trailerRepository;
        }

        // GET: api/<TrailerController>
        [HttpGet]
        public async Task<IActionResult> GetAllMoviesInfo()
        {
            return Ok(await _trailerRepository.GetAllMoviesInfoAsync());
        }
    }
}
