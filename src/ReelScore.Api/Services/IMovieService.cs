using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;

namespace ReelScore.Api.Services;

public interface IMovieService
{
    Task<IReadOnlyCollection<MovieResponse>> GetAllMoviesAsync(
        CancellationToken cancellationToken = default);

    Task<MovieResponse?> GetMovieByIdAsync(
        long movieId,
        CancellationToken cancellationToken = default);

    Task<MovieResponse> CreateMovieAsync(
        CreateMovieRequest request,
        CancellationToken cancellationToken = default);

    Task<MovieResponse?> UpdateMovieAsync(
        long movieId,
        UpdateMovieRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteMovieAsync(
        long movieId,
        CancellationToken cancellationToken = default);

    Task<ImportMovieResult> ImportTitleAsync(
        int tmdbId,
        MediaType mediaType,
        string language = "en-US",
        CancellationToken cancellationToken = default);
}
