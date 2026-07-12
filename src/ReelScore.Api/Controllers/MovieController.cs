using Microsoft.AspNetCore.Mvc;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Services;

namespace ReelScore.Api.Controllers;

[ApiController]
[Route("api/movies")]
public sealed class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<MovieResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<MovieResponse>>> GetMovies(
        CancellationToken cancellationToken)
    {
        var movies = await _movieService.GetAllMoviesAsync(cancellationToken);

        return Ok(movies);
    }

    [HttpGet("{movieId:long}")]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieResponse>> GetMovie(
        long movieId,
        CancellationToken cancellationToken)
    {
        var movie = await _movieService.GetMovieByIdAsync(
            movieId,
            cancellationToken);

        if (movie is null)
        {
            return NotFound(new
            {
                message = $"Movie with ID {movieId} was not found."
            });
        }

        return Ok(movie);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieResponse>> PostMovie(
        [FromBody] CreateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var movie = await _movieService.CreateMovieAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetMovie),
            new
            {
                movieId = movie.MovieId
            },
            movie);
    }

    [HttpPut("{movieId:long}")]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieResponse>> PutMovie(
        long movieId,
        [FromBody] UpdateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var movie = await _movieService.UpdateMovieAsync(
            movieId,
            request,
            cancellationToken);

        if (movie is null)
        {
            return NotFound(new
            {
                message = $"Movie with ID {movieId} was not found."
            });
        }

        return Ok(movie);
    }

    [HttpDelete("{movieId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMovie(
        long movieId,
        CancellationToken cancellationToken)
    {
        var deleted = await _movieService.DeleteMovieAsync(
            movieId,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Movie with ID {movieId} was not found."
            });
        }

        return NoContent();
    }
}
