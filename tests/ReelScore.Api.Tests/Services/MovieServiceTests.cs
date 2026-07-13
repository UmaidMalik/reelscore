using Microsoft.Extensions.Options;
using Moq;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Integrations.Tmdb;
using ReelScore.Api.Models;
using ReelScore.Api.Repositories;
using ReelScore.Api.Services;

namespace ReelScore.Api.Tests.Services;

public sealed class MovieServiceTests
{
    private readonly Mock<IMovieRepository> _movieRepository = new();
    private readonly Mock<ITmdbClient> _tmdbClient = new();

    private readonly IOptions<TmdbOptions> _options =
        Options.Create(
            new TmdbOptions
            {
                BaseUrl = "https://api.themoviedb.org/3/",
                ImageBaseUrl = "https://image.tmdb.org/t/p/",
                AccessToken = "test-token"
            });

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
            .Setup(repository =>
                repository.GetAllAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(movies);

        var service = CreateService();

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
            .Setup(repository =>
                repository.AddAsync(
                    It.IsAny<Movie>(),
                    It.IsAny<CancellationToken>()))
            .Callback<Movie, CancellationToken>(
                (movie, _) =>
                {
                    movie.MovieId = 42;
                    capturedMovie = movie;
                })
            .ReturnsAsync(
                (Movie movie, CancellationToken _) => movie);

        var service = CreateService();

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
            .Setup(repository =>
                repository.GetByIdAsync(
                    999,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie?)null);

        var service = CreateService();

        var result = await service.GetMovieByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task ImportMovieAsync_WhenValid_ImportsMovie()
    {
        const int tmdbId = 348;

        _movieRepository
            .Setup(repository =>
                repository.TmdbMovieExistsAsync(
                    tmdbId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _tmdbClient
            .Setup(client =>
                client.GetMovieDetailsAsync(
                    tmdbId,
                    "en-US",
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ExternalMovieDetailsResponse
                {
                    TmdbId = tmdbId,
                    Title = "Alien",
                    OriginalTitle = "Alien",
                    Overview = "A space crew encounters a dangerous lifeform.",
                    ReleaseDate = new DateOnly(1979, 5, 25),
                    RuntimeMinutes = 117,
                    PosterPath = "/poster.jpg",
                    BackdropPath = "/backdrop.jpg",
                    Genres = new[] { "Horror", "Science Fiction" },
                    OriginalLanguage = "en",
                    TmdbScore = 8.2,
                    TmdbVoteCount = 15000
                });

        _movieRepository
            .Setup(repository =>
                repository.AddAsync(
                    It.IsAny<Movie>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (Movie movie, CancellationToken _) =>
                {
                    movie.MovieId = 100;
                    return movie;
                });



        var service = CreateService();

        var result = await service.ImportMovieAsync(tmdbId);

        Assert.Equal(ImportMovieStatus.Imported, result.Status);
        Assert.NotNull(result.Movie);

        Assert.Equal(100, result.Movie.MovieId);
        Assert.Equal(tmdbId, result.Movie.TmdbId);
        Assert.Equal("Alien", result.Movie.Title);
        Assert.Equal(1979, result.Movie.ReleaseYear);
        Assert.Equal(117, result.Movie.RuntimeMinutes);
        Assert.Equal(0, result.Movie.RatingCount);
        Assert.Equal(0, result.Movie.AverageRating);
    }

    [Fact]
    public async Task ImportMovieAsync_WhenAlreadyImported_ReturnsAlreadyImported()
    {
        const int tmdbId = 348;

        var existingMovie = new Movie
        {
            MovieId = 55,
            TmdbId = tmdbId,
            Title = "Alien",
            OriginalTitle = "Alien",
            Summary = "Existing movie.",
            ReleaseDate = new DateOnly(1979, 5, 25),
            ReleaseYear = 1979,
            Genres = new[] { "Horror", "Science Fiction" }
        };

        _movieRepository
            .Setup(repository =>
                repository.TmdbMovieExistsAsync(
                    tmdbId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _movieRepository
            .Setup(repository =>
                repository.GetByTmdbIdAsync(
                    tmdbId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingMovie);

        var service = CreateService();

        var result = await service.ImportMovieAsync(tmdbId);

        Assert.Equal(
            ImportMovieStatus.AlreadyImported,
            result.Status);

        Assert.NotNull(result.Movie);
        Assert.Equal(55, result.Movie.MovieId);
        Assert.Equal(tmdbId, result.Movie.TmdbId);

        _tmdbClient.Verify(
            client => client.GetMovieDetailsAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _movieRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<Movie>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ImportMovieAsync_WhenTmdbMovieDoesNotExist_ReturnsNotFound()
    {
        const int tmdbId = 999999;

        _movieRepository
            .Setup(repository =>
                repository.TmdbMovieExistsAsync(
                    tmdbId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _tmdbClient
            .Setup(client =>
                client.GetMovieDetailsAsync(
                    tmdbId,
                    "en-US",
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExternalMovieDetailsResponse?)null);

        var service = CreateService();

        var result = await service.ImportMovieAsync(tmdbId);

        Assert.Equal(
            ImportMovieStatus.ExternalMovieNotFound,
            result.Status);

        Assert.Null(result.Movie);

        _movieRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<Movie>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ImportMovieAsync_WhenReleaseYearMissing_ReturnsInvalidMovieData()
    {
        const int tmdbId = 123;

        _movieRepository
            .Setup(repository =>
                repository.TmdbMovieExistsAsync(
                    tmdbId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _tmdbClient
            .Setup(client =>
                client.GetMovieDetailsAsync(
                    tmdbId,
                    "en-US",
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ExternalMovieDetailsResponse
                {
                    TmdbId = tmdbId,
                    Title = "Unknown Release",
                    OriginalTitle = "Unknown Release",
                    Overview = "No release date is available.",
                    ReleaseDate = null,
                    Genres = Array.Empty<string>()
                });

        var service = CreateService();

        var result = await service.ImportMovieAsync(tmdbId);

        Assert.Equal(
            ImportMovieStatus.InvalidMovieData,
            result.Status);

        Assert.Null(result.Movie);

        _movieRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<Movie>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ImportMovieAsync_MapsExternalMetadataToMovieEntity()
    {
        const int tmdbId = 603;

        Movie? capturedMovie = null;

        _movieRepository
            .Setup(repository =>
                repository.TmdbMovieExistsAsync(
                    tmdbId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _tmdbClient
            .Setup(client =>
                client.GetMovieDetailsAsync(
                    tmdbId,
                    "fr-CA",
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ExternalMovieDetailsResponse
                {
                    TmdbId = tmdbId,
                    Title = "  The Matrix  ",
                    OriginalTitle = "  The Matrix  ",
                    Overview = "  A simulated reality.  ",
                    ReleaseDate = new DateOnly(1999, 3, 30),
                    RuntimeMinutes = 136,
                    PosterPath = "/matrix-poster.jpg",
                    BackdropPath = "/matrix-backdrop.jpg",
                    Genres = new[]
                    {
                        "Action",
                        "Science Fiction"
                    },
                    OriginalLanguage = "en",
                    TmdbScore = 8.2,
                    TmdbVoteCount = 26000
                });

        _movieRepository
            .Setup(repository =>
                repository.AddAsync(
                    It.IsAny<Movie>(),
                    It.IsAny<CancellationToken>()))
            .Callback<Movie, CancellationToken>(
                (movie, _) =>
                {
                    capturedMovie = movie;
                    movie.MovieId = 77;
                })
            .ReturnsAsync(
                (Movie movie, CancellationToken _) => movie);

        var service = CreateService();

        var result = await service.ImportMovieAsync(
            tmdbId,
            "fr-CA");

        Assert.Equal(ImportMovieStatus.Imported, result.Status);
        Assert.NotNull(capturedMovie);

        Assert.Equal(tmdbId, capturedMovie.TmdbId);
        Assert.Equal("The Matrix", capturedMovie.Title);
        Assert.Equal("The Matrix", capturedMovie.OriginalTitle);
        Assert.Equal(
            "A simulated reality.",
            capturedMovie.Summary);

        Assert.Equal(
            new DateOnly(1999, 3, 30),
            capturedMovie.ReleaseDate);

        Assert.Equal(1999, capturedMovie.ReleaseYear);
        Assert.Equal(136, capturedMovie.RuntimeMinutes);
        Assert.Equal(
            "/matrix-poster.jpg",
            capturedMovie.PosterPath);

        Assert.Equal(
            "/matrix-backdrop.jpg",
            capturedMovie.BackdropPath);

        Assert.Equal(
            new[] { "Action", "Science Fiction" },
            capturedMovie.Genres);

        Assert.NotNull(result.Movie);

        Assert.Equal(
            "https://image.tmdb.org/t/p/w500/matrix-poster.jpg",
            result.Movie.PosterUrl);

        Assert.Equal(
            "https://image.tmdb.org/t/p/w1280/matrix-backdrop.jpg",
            result.Movie.BackdropUrl);
    }

    private MovieService CreateService()
    {
        return new MovieService(
            _movieRepository.Object,
            _tmdbClient.Object,
            _options);
    }
}
