namespace TrailerDownloader.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using TrailerDownloader.Application.Interfaces;
using TrailerDownloader.Domain.Models;

[ApiController]
[Route("api/[controller]")]
public class MovieController(IMovieService movieService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMovies()
    {
        var result = await movieService.GetAllMoviesAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMovie(Guid id)
    {
        var result = await movieService.GetMovieByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost("scan")]
    public async Task<IActionResult> ScanDirectories()
    {
        var result = await movieService.ScanMediaDirectoriesAsync();
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/download-trailer")]
    public async Task<IActionResult> DownloadTrailer(Guid id)
    {
        var result = await movieService.DownloadTrailerAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
