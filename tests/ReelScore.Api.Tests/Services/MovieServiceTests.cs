using Moq;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;
using ReelScore.Api.Repositories;
using ReelScore.Api.Services;

namespace ReelScore.Api.Tests.Services;

public sealed class MovieServiceTests
{
    private readonly Mock<IMovieRepository> _movieRepository = new();

    [Fact]
    public async Task GetAllMoviesAsync_MapsRatingSummary()
    {
        var movies = new List<Movie>
        {
            new()
            {
                MovieId = 10,
                Title = "Arrival",
                Summary = "First contact.",
                ReleaseYear = 2016,
                Ratings = new List<Rating>
                {
                    new() { Score = 8 },
                    new() { Score = 9 }
                }
            }
        };

        _movieRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(movies);

        var service = new MovieService(_movieRepository.Object);

        var result = await service.GetAllMoviesAsync();

        var movie = Assert.Single(result);
        Assert.Equal(10, movie.MovieId);
        Assert.Equal("Arrival", movie.Title);
        Assert.Equal(2, movie.RatingCount);
        Assert.Equal(8.5, movie.AverageRating);
    }

    [Fact]
    public async Task CreateMovieAsync_TrimsAndNormalizesInput()
    {
        Movie? capturedMovie = null;

        _movieRepository
            .Setup(repository => repository.AddAsync(
                It.IsAny<Movie>(),
                It.IsAny<CancellationToken>()))
            .Callback<Movie, CancellationToken>((movie, _) =>
            {
                movie.MovieId = 42;
                capturedMovie = movie;
            })
            .ReturnsAsync((Movie movie, CancellationToken _) => movie);

        var service = new MovieService(_movieRepository.Object);
        var request = new CreateMovieRequest
        {
            Title = "  Blade Runner  ",
            Summary = "   ",
            ReleaseYear = 1982
        };

        var result = await service.CreateMovieAsync(request);

        Assert.NotNull(capturedMovie);
        Assert.Equal("Blade Runner", capturedMovie.Title);
        Assert.Null(capturedMovie.Summary);
        Assert.Equal(42, result.MovieId);
        Assert.Equal(0, result.RatingCount);
        Assert.Equal(0, result.AverageRating);
    }

    [Fact]
    public async Task GetMovieByIdAsync_WhenMissing_ReturnsNull()
    {
        _movieRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie?)null);

        var service = new MovieService(_movieRepository.Object);

        var result = await service.GetMovieByIdAsync(999);

        Assert.Null(result);
    }
}
