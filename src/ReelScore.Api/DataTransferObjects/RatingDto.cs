namespace ReelScore.Api.DataTransferObjects;

public class RatingDto
{
    public long RatingId { get; set; }
    public long UserId { get; set; }
    public long MovieId { get; set; }
    public int MovieRating { get; set; }
    public string? Review { get; set; }
    public DateTime RatedAt { get; set; }
}