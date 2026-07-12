namespace ReelScore.Api.DataTransferObjects;

public sealed class MovieResponse
{
    public long MovieId
    {
        get; init;
    }

    public string Title { get; init; } = string.Empty;

    public string? Summary
    {
        get; init;
    }

    public int ReleaseYear
    {
        get; init;
    }

    public double AverageRating
    {
        get; init;
    }

    public int RatingCount
    {
        get; init;
    }
}
