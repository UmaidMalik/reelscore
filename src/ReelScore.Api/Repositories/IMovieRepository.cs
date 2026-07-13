using ReelScore.Api.Models;

namespace ReelScore.Api.Repositories;

public interface IMovieRepository
{
    Task<IReadOnlyCollection<Movie>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Movie?> GetByIdAsync(
        long movieId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Movie>> SearchAsync(
        string? title,
        int? releaseYear,
        CancellationToken cancellationToken = default);

    Task<Movie> AddAsync(
        Movie movie,
        CancellationToken cancellationToken = default);

    Task<Movie?> UpdateAsync(
        long movieId,
        string title,
        string? summary,
        int releaseYear,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        long movieId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        long movieId,
        CancellationToken cancellationToken = default);

    Task<Movie?> GetByTmdbIdAsync(
        int tmdbId,
        CancellationToken cancellationToken = default);

    Task<bool> TmdbMovieExistsAsync(
        int tmdbId,
        CancellationToken cancellationToken = default);
}
