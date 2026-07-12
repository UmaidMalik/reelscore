namespace ReelScore.Api.DataTransferObjects;

public sealed class RatingResponse
{
    public long RatingId
    {
        get; init;
    }

    public int Score
    {
        get; init;
    }

    public string? Review
    {
        get; init;
    }

    public DateTime RatedAt
    {
        get; init;
    }

    public long UserId
    {
        get; init;
    }

    public long MovieId
    {
        get; init;
    }
}
