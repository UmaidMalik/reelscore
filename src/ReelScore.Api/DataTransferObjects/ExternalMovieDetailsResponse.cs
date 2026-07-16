namespace ReelScore.Api.DataTransferObjects;

public sealed class ExternalMovieDetailsResponse
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

    public int? RuntimeMinutes
    {
        get; init;
    }

    public int? NumberOfSeasons
    {
        get; init;
    }

    public int? NumberOfEpisodes
    {
        get; init;
    }

    public string? PosterPath
    {
        get; init;
    }

    public string? PosterUrl
    {
        get; init;
    }

    public string? BackdropPath
    {
        get; init;
    }

    public string? BackdropUrl
    {
        get; init;
    }

    public IReadOnlyCollection<string> Genres
    {
        get; init;
    } =
        Array.Empty<string>();

    public string OriginalLanguage { get; init; } = string.Empty;

    public double TmdbScore
    {
        get; init;
    }

    public int TmdbVoteCount
    {
        get; init;
    }

    public string? Status
    {
        get; init;
    }
}
