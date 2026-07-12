using System.ComponentModel.DataAnnotations;

namespace ReelScore.Api.DataTransferObjects;

public sealed class CreateRatingRequest
{
    [Range(1, 10)]
    public int Score
    {
        get; init;
    }

    [StringLength(2000)]
    public string? Review
    {
        get; init;
    }

    [Range(1, long.MaxValue)]
    public long UserId
    {
        get; init;
    }

    [Range(1, long.MaxValue)]
    public long MovieId
    {
        get; init;
    }
}
