using System.Text.Json.Serialization;

namespace ReelScore.Api.Integrations.Tmdb.Models;

internal sealed class TmdbTvSearchItem
{
    [JsonPropertyName("id")]
    public int Id
    {
        get; init;
    }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("original_name")]
    public string OriginalName { get; init; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; init; } = string.Empty;

    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate
    {
        get; init;
    }

    [JsonPropertyName("poster_path")]
    public string? PosterPath
    {
        get; init;
    }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath
    {
        get; init;
    }

    [JsonPropertyName("genre_ids")]
    public IReadOnlyCollection<int> GenreIds
    {
        get; init;
    } =
        Array.Empty<int>();

    [JsonPropertyName("original_language")]
    public string OriginalLanguage { get; init; } = string.Empty;

    [JsonPropertyName("popularity")]
    public double Popularity
    {
        get; init;
    }

    [JsonPropertyName("vote_average")]
    public double VoteAverage
    {
        get; init;
    }

    [JsonPropertyName("vote_count")]
    public int VoteCount
    {
        get; init;
    }

    [JsonPropertyName("adult")]
    public bool Adult
    {
        get; init;
    }
}
