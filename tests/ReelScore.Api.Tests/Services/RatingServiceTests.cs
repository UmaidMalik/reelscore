using Moq;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;
using ReelScore.Api.Repositories;
using ReelScore.Api.Services;

namespace ReelScore.Api.Tests.Services;

public sealed class RatingServiceTests
{
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IMovieRepository> _movieRepository = new();

    [Fact]
    public async Task CreateRatingAsync_WhenUserDoesNotExist_ReturnsUserNotFound()
    {
        _userRepository
            .Setup(repository => repository.ExistsAsync(
                100,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService();
        var request = CreateRequest();

        var result = await service.CreateRatingAsync(request);

        Assert.Equal(CreateRatingStatus.UserNotFound, result.Status);
        Assert.Null(result.Rating);

        _movieRepository.Verify(
            repository => repository.ExistsAsync(
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateRatingAsync_WhenRatingAlreadyExists_ReturnsAlreadyExists()
    {
        _userRepository
            .Setup(repository => repository.ExistsAsync(
                100,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _movieRepository
            .Setup(repository => repository.ExistsAsync(
                200,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _ratingRepository
            .Setup(repository => repository.ExistsForUserAndMovieAsync(
                100,
                200,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService();

        var result = await service.CreateRatingAsync(CreateRequest());

        Assert.Equal(CreateRatingStatus.AlreadyExists, result.Status);
        Assert.Null(result.Rating);
    }

    [Fact]
    public async Task CreateRatingAsync_WithValidRequest_CreatesRating()
    {
        _userRepository
            .Setup(repository => repository.ExistsAsync(
                100,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _movieRepository
            .Setup(repository => repository.ExistsAsync(
                200,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _ratingRepository
            .Setup(repository => repository.ExistsForUserAndMovieAsync(
                100,
                200,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Rating? capturedRating = null;

        _ratingRepository
            .Setup(repository => repository.AddAsync(
                It.IsAny<Rating>(),
                It.IsAny<CancellationToken>()))
            .Callback<Rating, CancellationToken>((rating, _) =>
            {
                rating.RatingId = 300;
                capturedRating = rating;
            })
            .ReturnsAsync((Rating rating, CancellationToken _) => rating);

        var service = CreateService();
        var request = CreateRequest();

        var result = await service.CreateRatingAsync(request);

        Assert.Equal(CreateRatingStatus.Created, result.Status);
        Assert.NotNull(result.Rating);
        Assert.Equal(300, result.Rating.RatingId);
        Assert.Equal(9, result.Rating.Score);
        Assert.Equal("Excellent film.", result.Rating.Review);
        Assert.NotNull(capturedRating);
        Assert.Equal(100, capturedRating.UserId);
        Assert.Equal(200, capturedRating.MovieId);
    }

    [Fact]
    public async Task UpdateRatingAsync_WhenMissing_ReturnsNull()
    {
        _ratingRepository
            .Setup(repository => repository.UpdateAsync(
                999,
                8,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Rating?)null);

        var service = CreateService();
        var request = new UpdateRatingRequest
        {
            Score = 8,
            Review = "   "
        };

        var result = await service.UpdateRatingAsync(999, request);

        Assert.Null(result);
    }

    private RatingService CreateService()
    {
        return new RatingService(
            _ratingRepository.Object,
            _userRepository.Object,
            _movieRepository.Object);
    }

    private static CreateRatingRequest CreateRequest()
    {
        return new CreateRatingRequest
        {
            UserId = 100,
            MovieId = 200,
            Score = 9,
            Review = "  Excellent film.  "
        };
    }
}
