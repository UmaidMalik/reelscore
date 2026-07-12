using ReelScore.Api.DataTransferObjects;

namespace ReelScore.Api.Services;

public enum CreateRatingStatus
{
    Created,
    UserNotFound,
    MovieNotFound,
    AlreadyExists
}

public sealed record CreateRatingResult(
    CreateRatingStatus Status,
    RatingResponse? Rating = null);
