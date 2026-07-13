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

    [HttpPost("import/{tmdbId:int}")]
    [ProducesResponseType(
        typeof(MovieResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(MovieResponse),
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<MovieResponse>> ImportMovie(
        int tmdbId,
        [FromQuery] string language = "en-US",
        CancellationToken cancellationToken = default)
    {
        var result = await _movieService.ImportMovieAsync(
            tmdbId,
            language,
            cancellationToken);

        if (result.Status == ImportMovieStatus.ExternalMovieNotFound)
        {
            return NotFound(new
            {
                message = $"TMDB movie with ID {tmdbId} was not found."
            });
        }

        if (result.Status == ImportMovieStatus.InvalidMovieData)
        {
            return UnprocessableEntity(new
            {
                message =
                    "The external movie does not contain the required title and release-year information."
            });
        }

        if (result.Status == ImportMovieStatus.AlreadyImported)
        {
            return Conflict(new
            {
                message = "This TMDB movie already exists in the ReelScore library.",
                movie = result.Movie
            });
        }

        var movie = result.Movie!;

        return CreatedAtAction(
            nameof(GetMovie),
            new
            {
                movieId = movie.MovieId
            },
            movie);
    }

}
