namespace ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;

public sealed class MovieResponse
{
    public long MovieId
    {
        get; init;
    }

    public int? TmdbId
    {
        get; init;
    }

    public string Title { get; init; } = string.Empty;

    public string? OriginalTitle
    {
        get; init;
    }

    public string? Summary
    {
        get; init;
    }

    public DateOnly? ReleaseDate
    {
        get; init;
    }

    public int? ReleaseYear
    {
        get; init;
    }

    public int? RuntimeMinutes
    {
        get; init;
    }

    public string? PosterUrl
    {
        get; init;
    }

    public string? BackdropUrl
    {
        get; init;
    }

    public IReadOnlyCollection<string> Genres { get; init; } = Array.Empty<string>();

    public double AverageRating
    {
        get; init;
    }

    public int RatingCount
    {
        get; init;
    }

    public string MediaType
    {
        get; init;
    } = string.Empty;

    public int? NumberOfSeasons
    {
        get; init;
    }

    public int? NumberOfEpisodes
    {
        get; init;
    }
}
