using Microsoft.EntityFrameworkCore;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;
using ReelScore.Api.Repositories;

namespace ReelScore.Api.Services;

public sealed class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMovieRepository _movieRepository;

    public RatingService(
        IRatingRepository ratingRepository,
        IUserRepository userRepository,
        IMovieRepository movieRepository)
    {
        _ratingRepository = ratingRepository;
        _userRepository = userRepository;
        _movieRepository = movieRepository;
    }

    public async Task<IReadOnlyCollection<RatingResponse>> GetAllRatingsAsync(
        CancellationToken cancellationToken = default)
    {
        var ratings = await _ratingRepository.GetAllAsync(
            cancellationToken);

        return ratings
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<RatingResponse?> GetRatingByIdAsync(
        long ratingId,
        CancellationToken cancellationToken = default)
    {
        var rating = await _ratingRepository.GetByIdAsync(
            ratingId,
            cancellationToken);

        return rating is null
            ? null
            : MapToResponse(rating);
    }

    public async Task<CreateRatingResult> CreateRatingAsync(
        CreateRatingRequest request,
        CancellationToken cancellationToken = default)
    {
        var userExists = await _userRepository.ExistsAsync(
            request.UserId,
            cancellationToken);

        if (!userExists)
        {
            return new CreateRatingResult(
                CreateRatingStatus.UserNotFound);
        }

        var movieExists = await _movieRepository.ExistsAsync(
            request.MovieId,
            cancellationToken);

        if (!movieExists)
        {
            return new CreateRatingResult(
                CreateRatingStatus.MovieNotFound);
        }

        var ratingAlreadyExists =
            await _ratingRepository.ExistsForUserAndMovieAsync(
                request.UserId,
                request.MovieId,
                cancellationToken);

        if (ratingAlreadyExists)
        {
            return new CreateRatingResult(
                CreateRatingStatus.AlreadyExists);
        }

        var rating = new Rating
        {
            Score = request.Score,
            Review = NormalizeOptionalText(request.Review),
            UserId = request.UserId,
            MovieId = request.MovieId,
            RatedAt = DateTime.UtcNow
        };

        try
        {
            var createdRating = await _ratingRepository.AddAsync(
                rating,
                cancellationToken);

            return new CreateRatingResult(
                CreateRatingStatus.Created,
                MapToResponse(createdRating));
        }
        catch (DbUpdateException)
        {
            /*
             * The database has a unique constraint on UserId + MovieId.
             * This catches a race where two requests pass the existence
             * check before either insert is committed.
             */
            return new CreateRatingResult(
                CreateRatingStatus.AlreadyExists);
        }
    }

    public async Task<RatingResponse?> UpdateRatingAsync(
        long ratingId,
        UpdateRatingRequest request,
        CancellationToken cancellationToken = default)
    {
        var updatedRating = await _ratingRepository.UpdateAsync(
            ratingId,
            request.Score,
            NormalizeOptionalText(request.Review),
            cancellationToken);

        return updatedRating is null
            ? null
            : MapToResponse(updatedRating);
    }

    public Task<bool> DeleteRatingAsync(
        long ratingId,
        CancellationToken cancellationToken = default)
    {
        return _ratingRepository.DeleteAsync(
            ratingId,
            cancellationToken);
    }

    private static RatingResponse MapToResponse(Rating rating)
    {
        return new RatingResponse
        {
            RatingId = rating.RatingId,
            Score = rating.Score,
            Review = rating.Review,
            RatedAt = rating.RatedAt,
            UserId = rating.UserId,
            MovieId = rating.MovieId
        };
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
