using ReelScore.Api.Models;

namespace ReelScore.Api.Repositories;

public interface IRatingRepository
{
    Task<IEnumerable<Rating>> GetAllRatingsAsync();
}