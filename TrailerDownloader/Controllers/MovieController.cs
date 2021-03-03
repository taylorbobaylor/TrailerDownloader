using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using TrailerDownloader.Models;
using TrailerDownloader.Services.Interfaces;

namespace TrailerDownloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
        }

        // GET: api/<MovieController>/info
        [HttpGet]
        [Route("info")]
        public IActionResult GetAllMovieInfo()
        {
            try
            {
                return Ok(_movieService.GetAllMovieInfo());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in MovieInfoController.GetAllMovieInfo");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST api/<MovieController>/trailer
        [HttpPost]
        [Route("trailer")]
        public IActionResult DownloadMovieTrailers([FromBody] Movie[] movies)
        {
            try
            {
                _movieService.DownloadTrailers(movies);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in MovieTrailerController.DownloadMovieTrailers");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST api/<MovieController>/trailer/delete
        [HttpPost]
        [Route("trailer/delete")]
        public IActionResult DeleteAllTrailers(Movie[] movies)
        {
            try
            {
                _movieService.DeleteAllTrailers(movies);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in MovieTrailerController.DeleteAllTrailers");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
