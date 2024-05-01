using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrailerDownloaderAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly TmdbService _tmdbService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(TmdbService tmdbService, ILogger<MoviesController> logger)
        {
            _tmdbService = tmdbService;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query parameter is required.");
            }

            try
            {
                var searchResults = await _tmdbService.SearchMoviesAsync(query);
                return Ok(searchResults);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the search request.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("download/{movieId}")]
        public async Task<IActionResult> DownloadTrailer(int movieId)
        {
            if (movieId <= 0)
            {
                return BadRequest("Invalid movie ID.");
            }

            try
            {
                var trailerDownloadUrl = await _tmdbService.GetMovieTrailerDownloadUrlAsync(movieId);
                if (string.IsNullOrEmpty(trailerDownloadUrl))
                {
                    return NotFound("Trailer not found.");
                }

                return Ok(new { MovieId = movieId, TrailerDownloadUrl = trailerDownloadUrl });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the download request.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
