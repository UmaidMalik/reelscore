using ReelScore.Api.DataTransferObjects;

namespace ReelScore.Api.Integrations.Tmdb;

public interface ITmdbClient
{
    Task<ExternalMovieSearchResponse> SearchMoviesAsync(
        string query,
        int page,
        string language,
        int? releaseYear,
        CancellationToken cancellationToken = default);
}
