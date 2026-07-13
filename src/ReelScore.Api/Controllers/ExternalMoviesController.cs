using Microsoft.AspNetCore.Mvc;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Integrations.Tmdb;

namespace ReelScore.Api.Controllers;

[ApiController]
[Route("api/external/movies")]
public sealed class ExternalMoviesController : ControllerBase
{
    private readonly ITmdbClient _tmdbClient;

    public ExternalMoviesController(ITmdbClient tmdbClient)
    {
        _tmdbClient = tmdbClient;
    }

    [HttpGet("search")]
    [ProducesResponseType(
        typeof(ExternalMovieSearchResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<ExternalMovieSearchResponse>> SearchMovies(
        [FromQuery] SearchExternalMoviesRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var results = await _tmdbClient.SearchMoviesAsync(
                request.Query,
                request.Page,
                request.Language,
                request.ReleaseYear,
                cancellationToken);

            return Ok(results);
        }
        catch (HttpRequestException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "Movie provider unavailable",
                detail: "TMDB could not complete the movie search request.");
        }
    }
}
