using ReelScore.Api.Models;

namespace ReelScore.Api.Services;

public interface IRatingService
{
    Task<IEnumerable<Rating>> GetAllRatings();
}