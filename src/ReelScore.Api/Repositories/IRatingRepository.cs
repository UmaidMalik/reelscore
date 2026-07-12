using ReelScore.Api.Models;

namespace ReelScore.Api.Repositories;

public interface IRatingRepository
{
    Task<IReadOnlyCollection<Rating>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Rating?> GetByIdAsync(
        long ratingId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Rating>> GetByMovieIdAsync(
        long movieId,
        CancellationToken cancellationToken = default);

    Task<Rating?> GetByUserAndMovieAsync(
        long userId,
        long movieId,
        CancellationToken cancellationToken = default);

    Task<Rating> AddAsync(
        Rating rating,
        CancellationToken cancellationToken = default);

    Task<Rating?> UpdateAsync(
        long ratingId,
        int score,
        string? review,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        long ratingId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForUserAndMovieAsync(
        long userId,
        long movieId,
        CancellationToken cancellationToken = default);
}
