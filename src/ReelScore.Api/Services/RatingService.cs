using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;

namespace ReelScore.Api.Services;

public class RatingService : IRatingService
{

    public RatingDto MapRatingToDto(Rating rating)
    {
        return new RatingDto
        {
            RatingId = rating.RatingId,
            UserId = rating.UserId,
            MovieId = rating.MovieId,
            MovieRating = rating.MovieRating,
            Review = rating.Review,
            RatedAt = rating.RatedAt
        };
    }

    public Task<IEnumerable<Rating>> GetAllRatings()
    {
        throw new NotImplementedException();
    }
}