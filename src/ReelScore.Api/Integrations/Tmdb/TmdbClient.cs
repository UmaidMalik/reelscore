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

    public async Task<ExternalMovieSearchResponse> SearchTitlesAsync(
        string query,
        int page,
        string language,
        int? releaseYear,
        CancellationToken cancellationToken = default)
    {
        var movieRequestUri = BuildMovieSearchUri(
            query,
            page,
            language,
            releaseYear);

        var tvRequestUri = BuildTvSearchUri(
            query,
            page,
            language,
            releaseYear);

        _logger.LogInformation(
            "Searching TMDB movies and TV series for query {SearchQuery} on page {Page}",
            query,
            page);

        var movieResponseTask = _httpClient.GetAsync(
            movieRequestUri,
            cancellationToken);

        var tvResponseTask = _httpClient.GetAsync(
            tvRequestUri,
            cancellationToken);

        await Task.WhenAll(
            movieResponseTask,
            tvResponseTask);

        using var movieHttpResponse = await movieResponseTask;
        using var tvHttpResponse = await tvResponseTask;

        movieHttpResponse.EnsureSuccessStatusCode();
        tvHttpResponse.EnsureSuccessStatusCode();

        var movieResponse =
            await movieHttpResponse.Content
                .ReadFromJsonAsync<TmdbMovieSearchResponse>(
                    cancellationToken: cancellationToken);

        var tvResponse =
            await tvHttpResponse.Content
                .ReadFromJsonAsync<TmdbTvSearchResponse>(
                    cancellationToken: cancellationToken);

        if (movieResponse is null || tvResponse is null)
        {
            throw new InvalidOperationException(
                "TMDB returned an empty or invalid search response.");
        }

        var movieResults = movieResponse.Results
            .Where(movie => !movie.Adult)
            .Select(MapMovieSearchResult);

        var tvResults = tvResponse.Results
            .Where(show => !show.Adult)
            .Select(MapTvSearchResult);

        var combinedResults = movieResults
            .Concat(tvResults)
            .OrderByDescending(result => result.Popularity)
            .ThenBy(result => result.Title)
            .ToArray();

        return new ExternalMovieSearchResponse
        {
            Page = page,
            TotalPages = Math.Max(
                movieResponse.TotalPages,
                tvResponse.TotalPages),
            TotalResults =
                movieResponse.TotalResults +
                tvResponse.TotalResults,
            Results = combinedResults
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

    public async Task<ExternalMovieDetailsResponse?> GetTvDetailsAsync(
        int tmdbId,
        string language = "en-US",
        CancellationToken cancellationToken = default)
    {
        var requestUri =
            $"tv/{tmdbId}?language={Uri.EscapeDataString(language)}";

        _logger.LogInformation(
            "Fetching TMDB TV details for series {TmdbId}",
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
            await response.Content.ReadFromJsonAsync<TmdbTvDetails>(
                cancellationToken: cancellationToken);

        if (details is null)
        {
            throw new InvalidOperationException(
                "TMDB returned an empty or invalid TV-details response.");
        }

        return MapTvDetailsToResponse(details);
    }

    private ExternalMovieSearchItemResponse MapMovieSearchResult(
        TmdbMovieSearchItem movie)
    {
        return new ExternalMovieSearchItemResponse
        {
            TmdbId = movie.Id,
            MediaType = "movie",
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

    private ExternalMovieSearchItemResponse MapTvSearchResult(
        TmdbTvSearchItem show)
    {
        return new ExternalMovieSearchItemResponse
        {
            TmdbId = show.Id,
            MediaType = "tv",
            Title = show.Name,
            OriginalTitle = show.OriginalName,
            Overview = show.Overview,
            ReleaseDate = ParseReleaseDate(show.FirstAirDate),
            PosterUrl = BuildImageUrl(
                PosterSize,
                show.PosterPath),
            BackdropUrl = BuildImageUrl(
                BackdropSize,
                show.BackdropPath),
            GenreIds = show.GenreIds,
            OriginalLanguage = show.OriginalLanguage,
            Popularity = show.Popularity,
            TmdbScore = Math.Round(show.VoteAverage, 1),
            TmdbVoteCount = show.VoteCount
        };
    }

    private ExternalMovieDetailsResponse MapDetailsToResponse(
        TmdbMovieDetails movie)
    {
        var releaseDate = ParseReleaseDate(movie.ReleaseDate);

        return new ExternalMovieDetailsResponse
        {
            TmdbId = movie.Id,
            MediaType = "movie",
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

    private ExternalMovieDetailsResponse MapTvDetailsToResponse(
        TmdbTvDetails show)
    {
        var firstAirDate = ParseReleaseDate(show.FirstAirDate);

        var runtimeMinutes = show.EpisodeRunTime
            .Where(runtime => runtime > 0)
            .Cast<int?>()
            .FirstOrDefault();

        return new ExternalMovieDetailsResponse
        {
            TmdbId = show.Id,
            MediaType = "tv",
            Title = show.Name,
            OriginalTitle = show.OriginalName,
            Overview = show.Overview,
            ReleaseDate = firstAirDate,
            RuntimeMinutes = runtimeMinutes,
            NumberOfSeasons = show.NumberOfSeasons,
            NumberOfEpisodes = show.NumberOfEpisodes,
            PosterPath = show.PosterPath,
            PosterUrl = BuildImageUrl(
                PosterSize,
                show.PosterPath),
            BackdropPath = show.BackdropPath,
            BackdropUrl = BuildImageUrl(
                BackdropSize,
                show.BackdropPath),
            Genres = show.Genres
                .Select(genre => genre.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .ToArray(),
            OriginalLanguage = show.OriginalLanguage,
            TmdbScore = Math.Round(show.VoteAverage, 1),
            TmdbVoteCount = show.VoteCount,
            Status = string.IsNullOrWhiteSpace(show.Status)
                ? null
                : show.Status
        };
    }

    private static string BuildMovieSearchUri(
        string query,
        int page,
        string language,
        int? releaseYear)
    {
        var queryParameters = BuildCommonSearchParameters(
            query,
            page,
            language);

        if (releaseYear.HasValue)
        {
            queryParameters.Add(
                $"primary_release_year={releaseYear.Value}");
        }

        return $"search/movie?{string.Join("&", queryParameters)}";
    }

    private static string BuildTvSearchUri(
        string query,
        int page,
        string language,
        int? releaseYear)
    {
        var queryParameters = BuildCommonSearchParameters(
            query,
            page,
            language);

        if (releaseYear.HasValue)
        {
            queryParameters.Add(
                $"first_air_date_year={releaseYear.Value}");
        }

        return $"search/tv?{string.Join("&", queryParameters)}";
    }

    private static List<string> BuildCommonSearchParameters(
        string query,
        int page,
        string language)
    {
        return new List<string>
        {
            $"query={Uri.EscapeDataString(query.Trim())}",
            $"page={page}",
            $"language={Uri.EscapeDataString(language)}",
            "include_adult=false"
        };
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
