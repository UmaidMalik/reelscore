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
            MediaType = movie.MediaType switch
            {
                MediaType.Movie => "movie",
                MediaType.TvSeries => "tv",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(movie.MediaType),
                    movie.MediaType,
                    "Unsupported media type.")
            },
            Title = movie.Title,
            OriginalTitle = movie.OriginalTitle,
            Summary = movie.Summary,
            ReleaseDate = movie.ReleaseDate,
            ReleaseYear = movie.ReleaseYear,
            RuntimeMinutes = movie.RuntimeMinutes,
            NumberOfSeasons = movie.NumberOfSeasons,
            NumberOfEpisodes = movie.NumberOfEpisodes,
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

    public async Task<ImportMovieResult> ImportTitleAsync(
        int tmdbId,
        MediaType mediaType,
        string language = "en-US",
        CancellationToken cancellationToken = default)
    {
        var alreadyImported =
            await _movieRepository.TmdbTitleExistsAsync(
                tmdbId,
                mediaType,
                cancellationToken);

        if (alreadyImported)
        {
            var existingTitle =
                await _movieRepository.GetByTmdbIdentityAsync(
                    tmdbId,
                    mediaType,
                    cancellationToken);

            return new ImportMovieResult(
                ImportMovieStatus.AlreadyImported,
                existingTitle is null
                    ? null
                    : MapToResponse(existingTitle));
        }

        var externalTitle = mediaType switch
        {
            MediaType.Movie =>
                await _tmdbClient.GetMovieDetailsAsync(
                    tmdbId,
                    language,
                    cancellationToken),

            MediaType.TvSeries =>
                await _tmdbClient.GetTvDetailsAsync(
                    tmdbId,
                    language,
                    cancellationToken),

            _ => throw new ArgumentOutOfRangeException(
                nameof(mediaType),
                mediaType,
                "Unsupported media type.")
        };

        if (externalTitle is null)
        {
            return new ImportMovieResult(
                ImportMovieStatus.ExternalMovieNotFound);
        }

        if (string.IsNullOrWhiteSpace(externalTitle.Title))
        {
            return new ImportMovieResult(
                ImportMovieStatus.InvalidMovieData);
        }

        var title = new Movie
        {
            TmdbId = externalTitle.TmdbId,
            MediaType = mediaType,
            Title = externalTitle.Title.Trim(),
            OriginalTitle = NormalizeOptionalText(
                externalTitle.OriginalTitle),
            Summary = NormalizeOptionalText(
                externalTitle.Overview),
            ReleaseDate = externalTitle.ReleaseDate,
            ReleaseYear = externalTitle.ReleaseYear,
            RuntimeMinutes = externalTitle.RuntimeMinutes,
            NumberOfSeasons = mediaType == MediaType.TvSeries
                ? externalTitle.NumberOfSeasons
                : null,
            NumberOfEpisodes = mediaType == MediaType.TvSeries
                ? externalTitle.NumberOfEpisodes
                : null,
            PosterPath = externalTitle.PosterPath,
            BackdropPath = externalTitle.BackdropPath,
            Genres = externalTitle.Genres.ToArray()
        };

        var importedTitle = await _movieRepository.AddAsync(
            title,
            cancellationToken);

        return new ImportMovieResult(
            ImportMovieStatus.Imported,
            MapToResponse(importedTitle));
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
