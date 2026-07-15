using Microsoft.Extensions.Options;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Integrations.Tmdb;
using ReelScore.Api.Models;
using ReelScore.Api.Repositories;

namespace ReelScore.Api.Services;

public sealed class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    private const string PosterSize = "w500";
    private const string BackdropSize = "w1280";

    private readonly ITmdbClient _tmdbClient;
    private readonly TmdbOptions _tmdbOptions;

    public MovieService(
        IMovieRepository movieRepository,
        ITmdbClient tmdbClient,
        IOptions<TmdbOptions> tmdbOptions)
    {
        _movieRepository = movieRepository;
        _tmdbClient = tmdbClient;
        _tmdbOptions = tmdbOptions.Value;
    }

    public async Task<IReadOnlyCollection<MovieResponse>> GetAllMoviesAsync(
        CancellationToken cancellationToken = default)
    {
        var movies = await _movieRepository.GetAllAsync(cancellationToken);

        return movies
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<MovieResponse?> GetMovieByIdAsync(
        long movieId,
        CancellationToken cancellationToken = default)
    {
        var movie = await _movieRepository.GetByIdAsync(
            movieId,
            cancellationToken);

        return movie is null
            ? null
            : MapToResponse(movie);
    }

    public async Task<MovieResponse> CreateMovieAsync(
        CreateMovieRequest request,
        CancellationToken cancellationToken = default)
    {
        var movie = new Movie
        {
            Title = request.Title.Trim(),
            Summary = NormalizeOptionalText(request.Summary),
            ReleaseYear = request.ReleaseYear,
            MediaType = MediaType.Movie
        };

        var createdMovie = await _movieRepository.AddAsync(
            movie,
            cancellationToken);

        return MapToResponse(createdMovie);
    }

    public async Task<MovieResponse?> UpdateMovieAsync(
        long movieId,
        UpdateMovieRequest request,
        CancellationToken cancellationToken = default)
    {
        var updatedMovie = await _movieRepository.UpdateAsync(
            movieId,
            request.Title.Trim(),
            NormalizeOptionalText(request.Summary),
            request.ReleaseYear,
            cancellationToken);

        return updatedMovie is null
            ? null
            : MapToResponse(updatedMovie);
    }

    public Task<bool> DeleteMovieAsync(
        long movieId,
        CancellationToken cancellationToken = default)
    {
        return _movieRepository.DeleteAsync(
            movieId,
            cancellationToken);
    }

    private MovieResponse MapToResponse(Movie movie)
    {
        var ratingCount = movie.Ratings.Count;

        var averageRating = ratingCount == 0
            ? 0
            : movie.Ratings.Average(rating => rating.Score);

        return new MovieResponse
        {
            MovieId = movie.MovieId,
            TmdbId = movie.TmdbId,
            Title = movie.Title,
            OriginalTitle = movie.OriginalTitle,
            Summary = movie.Summary,
            ReleaseDate = movie.ReleaseDate,
            ReleaseYear = movie.ReleaseYear,
            RuntimeMinutes = movie.RuntimeMinutes,
            PosterUrl = BuildImageUrl(
                PosterSize,
                movie.PosterPath),
            BackdropUrl = BuildImageUrl(
                BackdropSize,
                movie.BackdropPath),
            Genres = movie.Genres,
            AverageRating = Math.Round(averageRating, 1),
            RatingCount = ratingCount
        };
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    public async Task<ImportMovieResult> ImportMovieAsync(
        int tmdbId,
        string language = "en-US",
        CancellationToken cancellationToken = default)
    {
        var alreadyImported =
            await _movieRepository.TmdbMovieExistsAsync(
                tmdbId,
                cancellationToken);

        if (alreadyImported)
        {
            var existingMovie =
                await _movieRepository.GetByTmdbIdAsync(
                    tmdbId,
                    cancellationToken);

            return new ImportMovieResult(
                ImportMovieStatus.AlreadyImported,
                existingMovie is null
                    ? null
                    : MapToResponse(existingMovie));
        }

        var externalMovie = await _tmdbClient.GetMovieDetailsAsync(
            tmdbId,
            language,
            cancellationToken);

        if (externalMovie is null)
        {
            return new ImportMovieResult(
                ImportMovieStatus.ExternalMovieNotFound);
        }

        if (externalMovie.ReleaseYear is null
            || string.IsNullOrWhiteSpace(externalMovie.Title))
        {
            return new ImportMovieResult(
                ImportMovieStatus.InvalidMovieData);
        }

        var movie = new Movie
        {
            TmdbId = externalMovie.TmdbId,
            Title = externalMovie.Title.Trim(),
            OriginalTitle = NormalizeOptionalText(
                externalMovie.OriginalTitle),
            Summary = NormalizeOptionalText(
                externalMovie.Overview),
            ReleaseDate = externalMovie.ReleaseDate,
            ReleaseYear = externalMovie.ReleaseYear.Value,
            RuntimeMinutes = externalMovie.RuntimeMinutes,
            PosterPath = externalMovie.PosterPath,
            BackdropPath = externalMovie.BackdropPath,
            Genres = externalMovie.Genres.ToArray()
        };

        var importedMovie = await _movieRepository.AddAsync(
            movie,
            cancellationToken);

        return new ImportMovieResult(
            ImportMovieStatus.Imported,
            MapToResponse(importedMovie));
    }

    private string? BuildImageUrl(
        string size,
        string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        return $"{_tmdbOptions.ImageBaseUrl.TrimEnd('/')}/{size}/{path.TrimStart('/')}";
    }

}
