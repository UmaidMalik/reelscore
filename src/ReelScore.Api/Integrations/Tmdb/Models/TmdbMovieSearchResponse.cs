using System.Text.Json.Serialization;

namespace ReelScore.Api.Integrations.Tmdb.Models;

internal sealed class TmdbMovieSearchResponse
{
    [JsonPropertyName("page")]
    public int Page
    {
        get; init;
    }

    [JsonPropertyName("results")]
    public IReadOnlyCollection<TmdbMovieSearchItem> Results
    {
        get; init;
    } =
        Array.Empty<TmdbMovieSearchItem>();

    [JsonPropertyName("total_pages")]
    public int TotalPages
    {
        get; init;
    }

    [JsonPropertyName("total_results")]
    public int TotalResults
    {
        get; init;
    }
}
