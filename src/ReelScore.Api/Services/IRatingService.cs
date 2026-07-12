using ReelScore.Api.DataTransferObjects;

namespace ReelScore.Api.Services;

public interface IRatingService
{
    Task<IReadOnlyCollection<RatingResponse>> GetAllRatingsAsync(
        CancellationToken cancellationToken = default);

    Task<RatingResponse?> GetRatingByIdAsync(
        long ratingId,
        CancellationToken cancellationToken = default);

    Task<CreateRatingResult> CreateRatingAsync(
        CreateRatingRequest request,
        CancellationToken cancellationToken = default);

    Task<RatingResponse?> UpdateRatingAsync(
        long ratingId,
        UpdateRatingRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteRatingAsync(
        long ratingId,
        CancellationToken cancellationToken = default);
}
