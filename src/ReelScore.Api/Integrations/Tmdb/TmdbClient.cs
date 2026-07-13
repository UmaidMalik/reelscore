using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Integrations.Tmdb.Models;

namespace ReelScore.Api.Integrations.Tmdb;

public sealed class TmdbClient : ITmdbClient
{
    private const string PosterSize = "w500";
    private const string BackdropSize = "w1280";

    private readonly HttpClient _httpClient;
    private readonly TmdbOptions _options;
    private readonly ILogger<TmdbClient> _logger;

    public TmdbClient(
        HttpClient httpClient,
        IOptions<TmdbOptions> options,
        ILogger<TmdbClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ExternalMovieSearchResponse> SearchMoviesAsync(
        string query,
        int page,
        string language,
        int? releaseYear,
        CancellationToken cancellationToken = default)
    {
        var requestUri = BuildSearchUri(
            query,
            page,
            language,
            releaseYear);

        _logger.LogInformation(
            "Searching TMDB for query {MovieQuery} on page {Page}",
            query,
            page);

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var tmdbResponse =
            await response.Content.ReadFromJsonAsync<TmdbMovieSearchResponse>(
                cancellationToken: cancellationToken);

        if (tmdbResponse is null)
        {
            throw new InvalidOperationException(
                "TMDB returned an empty or invalid response.");
        }

        return new ExternalMovieSearchResponse
        {
            Page = tmdbResponse.Page,
            TotalPages = tmdbResponse.TotalPages,
            TotalResults = tmdbResponse.TotalResults,
            Results = tmdbResponse.Results
                .Where(movie => !movie.Adult)
                .Select(MapToResponse)
                .ToList()
        };
    }

    public async Task<ExternalMovieDetailsResponse?> GetMovieDetailsAsync(
        int tmdbId,
        string language = "en-US",
        CancellationToken cancellationToken = default)
    {
        var requestUri =
            $"movie/{tmdbId}?language={Uri.EscapeDataString(language)}";

        _logger.LogInformation(
            "Fetching TMDB movie details for movie {TmdbId}",
            tmdbId);

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var details =
            await response.Content.ReadFromJsonAsync<TmdbMovieDetails>(
                cancellationToken: cancellationToken);

        if (details is null)
        {
            throw new InvalidOperationException(
                "TMDB returned an empty or invalid movie-details response.");
        }

        return MapDetailsToResponse(details);
    }

    private ExternalMovieSearchItemResponse MapToResponse(
        TmdbMovieSearchItem movie)
    {
        return new ExternalMovieSearchItemResponse
        {
            TmdbId = movie.Id,
            Title = movie.Title,
            OriginalTitle = movie.OriginalTitle,
            Overview = movie.Overview,
            ReleaseDate = ParseReleaseDate(movie.ReleaseDate),
            PosterUrl = BuildImageUrl(
                PosterSize,
                movie.PosterPath),
            BackdropUrl = BuildImageUrl(
                BackdropSize,
                movie.BackdropPath),
            GenreIds = movie.GenreIds,
            OriginalLanguage = movie.OriginalLanguage,
            Popularity = movie.Popularity,
            TmdbScore = Math.Round(movie.VoteAverage, 1),
            TmdbVoteCount = movie.VoteCount
        };
    }

    private ExternalMovieDetailsResponse MapDetailsToResponse(
        TmdbMovieDetails movie)
    {
        var releaseDate = ParseReleaseDate(movie.ReleaseDate);

        return new ExternalMovieDetailsResponse
        {
            TmdbId = movie.Id,
            Title = movie.Title,
            OriginalTitle = movie.OriginalTitle,
            Overview = movie.Overview,
            ReleaseDate = releaseDate,
            RuntimeMinutes = movie.Runtime,
            PosterPath = movie.PosterPath,
            PosterUrl = BuildImageUrl(
                PosterSize,
                movie.PosterPath),
            BackdropPath = movie.BackdropPath,
            BackdropUrl = BuildImageUrl(
                BackdropSize,
                movie.BackdropPath),
            Genres = movie.Genres
                .Select(genre => genre.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .ToArray(),
            OriginalLanguage = movie.OriginalLanguage,
            TmdbScore = Math.Round(movie.VoteAverage, 1),
            TmdbVoteCount = movie.VoteCount
        };
    }

    private string BuildSearchUri(
        string query,
        int page,
        string language,
        int? releaseYear)
    {
        var queryParameters = new List<string>
        {
            $"query={Uri.EscapeDataString(query.Trim())}",
            $"page={page}",
            $"language={Uri.EscapeDataString(language)}",
            "include_adult=false"
        };

        if (releaseYear.HasValue)
        {
            queryParameters.Add(
                $"primary_release_year={releaseYear.Value}");
        }

        return $"search/movie?{string.Join("&", queryParameters)}";
    }

    private string? BuildImageUrl(
        string size,
        string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return null;
        }

        return $"{_options.ImageBaseUrl.TrimEnd('/')}/{size}/{filePath.TrimStart('/')}";
    }

    private static DateOnly? ParseReleaseDate(string? releaseDate)
    {
        return DateOnly.TryParse(releaseDate, out var parsedDate)
            ? parsedDate
            : null;
    }
}
