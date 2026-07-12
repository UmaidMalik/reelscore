using Microsoft.AspNetCore.Mvc;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Services;

namespace ReelScore.Api.Controllers;

[ApiController]
[Route("api/ratings")]
public sealed class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<RatingResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<RatingResponse>>> GetRatings(
        CancellationToken cancellationToken)
    {
        var ratings = await _ratingService.GetAllRatingsAsync(
            cancellationToken);

        return Ok(ratings);
    }

    [HttpGet("{ratingId:long}")]
    [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RatingResponse>> GetRating(
        long ratingId,
        CancellationToken cancellationToken)
    {
        var rating = await _ratingService.GetRatingByIdAsync(
            ratingId,
            cancellationToken);

        if (rating is null)
        {
            return NotFound(new
            {
                message = $"Rating with ID {ratingId} was not found."
            });
        }

        return Ok(rating);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RatingResponse>> PostRating(
        [FromBody] CreateRatingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _ratingService.CreateRatingAsync(
            request,
            cancellationToken);

        if (result.Status == CreateRatingStatus.UserNotFound)
        {
            return NotFound(new
            {
                message = $"User with ID {request.UserId} was not found."
            });
        }

        if (result.Status == CreateRatingStatus.MovieNotFound)
        {
            return NotFound(new
            {
                message = $"Movie with ID {request.MovieId} was not found."
            });
        }

        if (result.Status == CreateRatingStatus.AlreadyExists)
        {
            return Conflict(new
            {
                message =
                    "This user has already rated this movie. Update the existing rating instead."
            });
        }

        var rating = result.Rating!;

        return CreatedAtAction(
            nameof(GetRating),
            new
            {
                ratingId = rating.RatingId
            },
            rating);
    }

    [HttpPut("{ratingId:long}")]
    [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RatingResponse>> PutRating(
        long ratingId,
        [FromBody] UpdateRatingRequest request,
        CancellationToken cancellationToken)
    {
        var rating = await _ratingService.UpdateRatingAsync(
            ratingId,
            request,
            cancellationToken);

        if (rating is null)
        {
            return NotFound(new
            {
                message = $"Rating with ID {ratingId} was not found."
            });
        }

        return Ok(rating);
    }

    [HttpDelete("{ratingId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRating(
        long ratingId,
        CancellationToken cancellationToken)
    {
        var deleted = await _ratingService.DeleteRatingAsync(
            ratingId,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Rating with ID {ratingId} was not found."
            });
        }

        return NoContent();
    }
}
