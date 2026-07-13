using System.Text.Json.Serialization;

namespace ReelScore.Api.Integrations.Tmdb.Models;

internal sealed class TmdbGenre
{
    [JsonPropertyName("id")]
    public int Id
    {
        get; init;
    }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
