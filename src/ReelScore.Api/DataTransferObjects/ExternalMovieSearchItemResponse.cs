namespace ReelScore.Api.DataTransferObjects;

public sealed class ExternalMovieSearchItemResponse
{
    public int TmdbId
    {
        get; init;
    }

    public string MediaType { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string OriginalTitle { get; init; } = string.Empty;

    public string Overview { get; init; } = string.Empty;

    public DateOnly? ReleaseDate
    {
        get; init;
    }

    public int? ReleaseYear => ReleaseDate?.Year;

    public string? PosterUrl
    {
        get; init;
    }

    public string? BackdropUrl
    {
        get; init;
    }

    public IReadOnlyCollection<int> GenreIds
    {
        get; init;
    } =
        Array.Empty<int>();

    public string OriginalLanguage { get; init; } = string.Empty;

    public double Popularity
    {
        get; init;
    }

    public double TmdbScore
    {
        get; init;
    }

    public int TmdbVoteCount
    {
        get; init;
    }
}
