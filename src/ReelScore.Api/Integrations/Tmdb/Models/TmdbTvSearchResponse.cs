using System.Text.Json.Serialization;

namespace ReelScore.Api.Integrations.Tmdb.Models;

internal sealed class TmdbTvSearchResponse
{
    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; init; }

    [JsonPropertyName("total_results")]
    public int TotalResults { get; init; }

    [JsonPropertyName("results")]
    public IReadOnlyCollection<TmdbTvSearchItem> Results { get; init; } =
        Array.Empty<TmdbTvSearchItem>();
}