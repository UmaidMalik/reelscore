using System.ComponentModel.DataAnnotations;

namespace ReelScore.Api.DataTransferObjects;

public sealed class UpdateRatingRequest
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
}
