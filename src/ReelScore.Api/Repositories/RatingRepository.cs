using ReelScore.Api.Models;

namespace ReelScore.Api.Repositories;

public class RatingRepository : IRatingRepository
{
    public Task<IEnumerable<Rating>> GetAllRatingsAsync()
    {
        throw new NotImplementedException();
    }
}