using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ReelScore.Api.Integrations.Tmdb;

namespace ReelScore.Api.Tests.Integrations.Tmdb;

public sealed class TmdbClientTests
{
    private const string BaseUrl = "https://api.themoviedb.org/3/";
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/";

    [Fact]
    public async Task SearchMoviesAsync_WhenSuccessful_MapsResponse()
    {
        const string responseJson = """
        {
          "page": 1,
          "total_pages": 3,
          "total_results": 42,
          "results": [
            {
              "id": 348,
              "title": "Alien",
              "original_title": "Alien",
              "overview": "A space crew encounters a dangerous lifeform.",
              "release_date": "1979-05-25",
              "poster_path": "/poster.jpg",
              "backdrop_path": "/backdrop.jpg",
              "genre_ids": [27, 878],
              "original_language": "en",
              "popularity": 91.5,
              "vote_average": 8.153,
              "vote_count": 15000,
              "adult": false
            }
          ]
        }
        """;

        using var httpClient = CreateHttpClient(
            (_, _) => CreateJsonResponse(
                HttpStatusCode.OK,
                responseJson));

        var client = CreateTmdbClient(httpClient);

        var result = await client.SearchMoviesAsync(
            "Alien",
            1,
            "en-US",
            null);

        var movie = Assert.Single(result.Results);

        Assert.Equal(1, result.Page);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(42, result.TotalResults);

        Assert.Equal(348, movie.TmdbId);
        Assert.Equal("Alien", movie.Title);
        Assert.Equal("Alien", movie.OriginalTitle);
        Assert.Equal(
            "A space crew encounters a dangerous lifeform.",
            movie.Overview);

        Assert.Equal(new DateOnly(1979, 5, 25), movie.ReleaseDate);
        Assert.Equal(1979, movie.ReleaseYear);

        Assert.Equal(
            "https://image.tmdb.org/t/p/w500/poster.jpg",
            movie.PosterUrl);

        Assert.Equal(
            "https://image.tmdb.org/t/p/w1280/backdrop.jpg",
            movie.BackdropUrl);

        Assert.Equal(new[] { 27, 878 }, movie.GenreIds);
        Assert.Equal("en", movie.OriginalLanguage);
        Assert.Equal(91.5, movie.Popularity);
        Assert.Equal(8.2, movie.TmdbScore);
        Assert.Equal(15000, movie.TmdbVoteCount);
    }

    [Fact]
    public async Task SearchMoviesAsync_FiltersAdultResults()
    {
        const string responseJson = """
        {
          "page": 1,
          "total_pages": 1,
          "total_results": 2,
          "results": [
            {
              "id": 1,
              "title": "Visible Movie",
              "original_title": "Visible Movie",
              "overview": "",
              "release_date": "2020-01-01",
              "poster_path": null,
              "backdrop_path": null,
              "genre_ids": [],
              "original_language": "en",
              "popularity": 1,
              "vote_average": 7,
              "vote_count": 10,
              "adult": false
            },
            {
              "id": 2,
              "title": "Filtered Movie",
              "original_title": "Filtered Movie",
              "overview": "",
              "release_date": "2020-01-01",
              "poster_path": null,
              "backdrop_path": null,
              "genre_ids": [],
              "original_language": "en",
              "popularity": 1,
              "vote_average": 7,
              "vote_count": 10,
              "adult": true
            }
          ]
        }
        """;

        using var httpClient = CreateHttpClient(
            (_, _) => CreateJsonResponse(
                HttpStatusCode.OK,
                responseJson));

        var client = CreateTmdbClient(httpClient);

        var result = await client.SearchMoviesAsync(
            "Movie",
            1,
            "en-US",
            null);

        var movie = Assert.Single(result.Results);

        Assert.Equal(1, movie.TmdbId);
        Assert.Equal("Visible Movie", movie.Title);
    }

    [Fact]
    public async Task SearchMoviesAsync_BuildsExpectedQueryParameters()
    {
        Uri? capturedRequestUri = null;

        const string responseJson = """
        {
          "page": 2,
          "total_pages": 2,
          "total_results": 0,
          "results": []
        }
        """;

        using var httpClient = CreateHttpClient(
            (request, _) =>
            {
                capturedRequestUri = request.RequestUri;

                return CreateJsonResponse(
                    HttpStatusCode.OK,
                    responseJson);
            });

        var client = CreateTmdbClient(httpClient);

        await client.SearchMoviesAsync(
            "Alien & Aliens",
            2,
            "fr-CA",
            1979);

        Assert.NotNull(capturedRequestUri);

        var requestUrl = capturedRequestUri.AbsoluteUri;

        Assert.Contains(
            "search/movie?",
            requestUrl,
            StringComparison.Ordinal);

        Assert.Contains(
            "query=Alien%20%26%20Aliens",
            requestUrl,
            StringComparison.Ordinal);

        Assert.Contains(
            "page=2",
            requestUrl,
            StringComparison.Ordinal);

        Assert.Contains(
            "language=fr-CA",
            requestUrl,
            StringComparison.Ordinal);

        Assert.Contains(
            "include_adult=false",
            requestUrl,
            StringComparison.Ordinal);

        Assert.Contains(
            "primary_release_year=1979",
            requestUrl,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task SearchMoviesAsync_WhenImagePathsAreMissing_ReturnsNullUrls()
    {
        const string responseJson = """
        {
          "page": 1,
          "total_pages": 1,
          "total_results": 1,
          "results": [
            {
              "id": 123,
              "title": "No Images",
              "original_title": "No Images",
              "overview": "",
              "release_date": "",
              "poster_path": null,
              "backdrop_path": null,
              "genre_ids": [],
              "original_language": "en",
              "popularity": 0,
              "vote_average": 0,
              "vote_count": 0,
              "adult": false
            }
          ]
        }
        """;

        using var httpClient = CreateHttpClient(
            (_, _) => CreateJsonResponse(
                HttpStatusCode.OK,
                responseJson));

        var client = CreateTmdbClient(httpClient);

        var result = await client.SearchMoviesAsync(
            "No Images",
            1,
            "en-US",
            null);

        var movie = Assert.Single(result.Results);

        Assert.Null(movie.PosterUrl);
        Assert.Null(movie.BackdropUrl);
        Assert.Null(movie.ReleaseDate);
        Assert.Null(movie.ReleaseYear);
    }

    [Fact]
    public async Task SearchMoviesAsync_WhenTmdbReturnsNull_ThrowsInvalidOperationException()
    {
        using var httpClient = CreateHttpClient(
            (_, _) => CreateJsonResponse(
                HttpStatusCode.OK,
                "null"));

        var client = CreateTmdbClient(httpClient);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.SearchMoviesAsync(
                "Alien",
                1,
                "en-US",
                null));

        Assert.Equal(
            "TMDB returned an empty or invalid response.",
            exception.Message);
    }

    [Fact]
    public async Task SearchMoviesAsync_WhenTmdbReturnsFailure_ThrowsHttpRequestException()
    {
        using var httpClient = CreateHttpClient(
            (_, _) => Task.FromResult(
                new HttpResponseMessage(
                    HttpStatusCode.ServiceUnavailable)));

        var client = CreateTmdbClient(httpClient);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => client.SearchMoviesAsync(
                "Alien",
                1,
                "en-US",
                null));
    }

    [Fact]
    public async Task SearchMoviesAsync_WhenCancelled_ThrowsOperationCancelledException()
    {
        using var httpClient = CreateHttpClient(
            async (_, cancellationToken) =>
            {
                await Task.Delay(
                    Timeout.InfiniteTimeSpan,
                    cancellationToken);

                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var client = CreateTmdbClient(httpClient);

        using var cancellationTokenSource =
            new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.SearchMoviesAsync(
                "Alien",
                1,
                "en-US",
                null,
                cancellationTokenSource.Token));
    }

    private static TmdbClient CreateTmdbClient(
        HttpClient httpClient)
    {
        var options = Options.Create(
            new TmdbOptions
            {
                BaseUrl = BaseUrl,
                ImageBaseUrl = ImageBaseUrl,
                AccessToken = "test-access-token"
            });

        return new TmdbClient(
            httpClient,
            options,
            NullLogger<TmdbClient>.Instance);
    }

    private static HttpClient CreateHttpClient(
        Func<
            HttpRequestMessage,
            CancellationToken,
            Task<HttpResponseMessage>> handler)
    {
        return new HttpClient(
            new StubHttpMessageHandler(handler))
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    private static Task<HttpResponseMessage> CreateJsonResponse(
        HttpStatusCode statusCode,
        string json)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json")
        };

        return Task.FromResult(response);
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<
            HttpRequestMessage,
            CancellationToken,
            Task<HttpResponseMessage>> _handler;

        public StubHttpMessageHandler(
            Func<
                HttpRequestMessage,
                CancellationToken,
                Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _handler(request, cancellationToken);
        }
    }
}
