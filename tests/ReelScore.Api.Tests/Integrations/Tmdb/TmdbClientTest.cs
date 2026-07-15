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
    public async Task SearchTitlesAsync_WhenSuccessful_MapsMovieAndTvResults()
    {
        const string movieResponseJson = """
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
              "poster_path": "/alien-poster.jpg",
              "backdrop_path": "/alien-backdrop.jpg",
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

        const string tvResponseJson = """
        {
          "page": 1,
          "total_pages": 2,
          "total_results": 18,
          "results": [
            {
              "id": 1396,
              "name": "Breaking Bad",
              "original_name": "Breaking Bad",
              "overview": "A chemistry teacher enters the drug trade.",
              "first_air_date": "2008-01-20",
              "poster_path": "/breaking-bad-poster.jpg",
              "backdrop_path": "/breaking-bad-backdrop.jpg",
              "genre_ids": [18, 80],
              "original_language": "en",
              "popularity": 120.4,
              "vote_average": 8.9,
              "vote_count": 14000,
              "adult": false
            }
          ]
        }
        """;

        using var httpClient = CreateSearchHttpClient(
            movieResponseJson,
            tvResponseJson);

        var client = CreateTmdbClient(httpClient);

        var result = await client.SearchTitlesAsync(
            "Alien",
            1,
            "en-US",
            null);

        Assert.Equal(1, result.Page);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(60, result.TotalResults);
        Assert.Equal(2, result.Results.Count);

        var tvShow = result.Results.First();
        var movie = result.Results.Last();

        Assert.Equal("tv", tvShow.MediaType);
        Assert.Equal(1396, tvShow.TmdbId);
        Assert.Equal("Breaking Bad", tvShow.Title);
        Assert.Equal(new DateOnly(2008, 1, 20), tvShow.ReleaseDate);
        Assert.Equal(2008, tvShow.ReleaseYear);
        Assert.Equal(8.9, tvShow.TmdbScore);

        Assert.Equal("movie", movie.MediaType);
        Assert.Equal(348, movie.TmdbId);
        Assert.Equal("Alien", movie.Title);
        Assert.Equal(new DateOnly(1979, 5, 25), movie.ReleaseDate);
        Assert.Equal(1979, movie.ReleaseYear);
        Assert.Equal(8.2, movie.TmdbScore);
    }

    [Fact]
    public async Task SearchTitlesAsync_FiltersAdultMovieAndTvResults()
    {
        const string movieResponseJson = """
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

        const string tvResponseJson = """
        {
          "page": 1,
          "total_pages": 1,
          "total_results": 2,
          "results": [
            {
              "id": 3,
              "name": "Visible Show",
              "original_name": "Visible Show",
              "overview": "",
              "first_air_date": "2021-01-01",
              "poster_path": null,
              "backdrop_path": null,
              "genre_ids": [],
              "original_language": "en",
              "popularity": 2,
              "vote_average": 8,
              "vote_count": 20,
              "adult": false
            },
            {
              "id": 4,
              "name": "Filtered Show",
              "original_name": "Filtered Show",
              "overview": "",
              "first_air_date": "2021-01-01",
              "poster_path": null,
              "backdrop_path": null,
              "genre_ids": [],
              "original_language": "en",
              "popularity": 2,
              "vote_average": 8,
              "vote_count": 20,
              "adult": true
            }
          ]
        }
        """;

        using var httpClient = CreateSearchHttpClient(
            movieResponseJson,
            tvResponseJson);

        var client = CreateTmdbClient(httpClient);

        var result = await client.SearchTitlesAsync(
            "Visible",
            1,
            "en-US",
            null);

        Assert.Equal(2, result.Results.Count);
        Assert.Contains(
            result.Results,
            item => item.Title == "Visible Movie"
                && item.MediaType == "movie");

        Assert.Contains(
            result.Results,
            item => item.Title == "Visible Show"
                && item.MediaType == "tv");
    }

    [Fact]
    public async Task SearchTitlesAsync_BuildsExpectedMovieAndTvQueryParameters()
    {
        var capturedRequestUris = new List<Uri>();

        const string emptyResponseJson = """
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
                Assert.NotNull(request.RequestUri);
                capturedRequestUris.Add(request.RequestUri);

                return CreateJsonResponse(
                    HttpStatusCode.OK,
                    emptyResponseJson);
            });

        var client = CreateTmdbClient(httpClient);

        await client.SearchTitlesAsync(
            "Alien & Aliens",
            2,
            "fr-CA",
            1979);

        Assert.Equal(2, capturedRequestUris.Count);

        var movieRequest = capturedRequestUris.Single(
            uri => uri.AbsolutePath.EndsWith(
                "/search/movie",
                StringComparison.Ordinal));

        var tvRequest = capturedRequestUris.Single(
            uri => uri.AbsolutePath.EndsWith(
                "/search/tv",
                StringComparison.Ordinal));

        AssertCommonSearchParameters(movieRequest);
        AssertCommonSearchParameters(tvRequest);

        Assert.Contains(
            "primary_release_year=1979",
            movieRequest.AbsoluteUri,
            StringComparison.Ordinal);

        Assert.Contains(
            "first_air_date_year=1979",
            tvRequest.AbsoluteUri,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task SearchTitlesAsync_WhenImagePathsAreMissing_ReturnsNullUrls()
    {
        const string movieResponseJson = """
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

        const string tvResponseJson = """
        {
          "page": 1,
          "total_pages": 1,
          "total_results": 0,
          "results": []
        }
        """;

        using var httpClient = CreateSearchHttpClient(
            movieResponseJson,
            tvResponseJson);

        var client = CreateTmdbClient(httpClient);

        var result = await client.SearchTitlesAsync(
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
    public async Task SearchTitlesAsync_WhenMovieResponseIsNull_ThrowsInvalidOperationException()
    {
        const string tvResponseJson = """
        {
          "page": 1,
          "total_pages": 1,
          "total_results": 0,
          "results": []
        }
        """;

        using var httpClient = CreateSearchHttpClient(
            "null",
            tvResponseJson);

        var client = CreateTmdbClient(httpClient);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.SearchTitlesAsync(
                "Alien",
                1,
                "en-US",
                null));

        Assert.Equal(
            "TMDB returned an empty or invalid search response.",
            exception.Message);
    }

    [Fact]
    public async Task SearchTitlesAsync_WhenTvResponseIsNull_ThrowsInvalidOperationException()
    {
        const string movieResponseJson = """
        {
          "page": 1,
          "total_pages": 1,
          "total_results": 0,
          "results": []
        }
        """;

        using var httpClient = CreateSearchHttpClient(
            movieResponseJson,
            "null");

        var client = CreateTmdbClient(httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.SearchTitlesAsync(
                "Alien",
                1,
                "en-US",
                null));
    }

    [Fact]
    public async Task SearchTitlesAsync_WhenEitherRequestFails_ThrowsHttpRequestException()
    {
        using var httpClient = CreateHttpClient(
            (request, _) =>
            {
                if (request.RequestUri?.AbsolutePath.EndsWith(
                        "/search/movie",
                        StringComparison.Ordinal) == true)
                {
                    return CreateJsonResponse(
                        HttpStatusCode.OK,
                        """
                        {
                          "page": 1,
                          "total_pages": 1,
                          "total_results": 0,
                          "results": []
                        }
                        """);
                }

                return Task.FromResult(
                    new HttpResponseMessage(
                        HttpStatusCode.ServiceUnavailable));
            });

        var client = CreateTmdbClient(httpClient);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => client.SearchTitlesAsync(
                "Alien",
                1,
                "en-US",
                null));
    }

    [Fact]
    public async Task SearchTitlesAsync_WhenCancelled_ThrowsOperationCancelledException()
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
            () => client.SearchTitlesAsync(
                "Alien",
                1,
                "en-US",
                null,
                cancellationTokenSource.Token));
    }

    private static void AssertCommonSearchParameters(
        Uri requestUri)
    {
        var requestUrl = requestUri.AbsoluteUri;

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
    }

    private static HttpClient CreateSearchHttpClient(
        string movieResponseJson,
        string tvResponseJson)
    {
        return CreateHttpClient(
            (request, _) =>
            {
                var path = request.RequestUri?.AbsolutePath;

                if (path?.EndsWith(
                        "/search/movie",
                        StringComparison.Ordinal) == true)
                {
                    return CreateJsonResponse(
                        HttpStatusCode.OK,
                        movieResponseJson);
                }

                if (path?.EndsWith(
                        "/search/tv",
                        StringComparison.Ordinal) == true)
                {
                    return CreateJsonResponse(
                        HttpStatusCode.OK,
                        tvResponseJson);
                }

                return Task.FromResult(
                    new HttpResponseMessage(
                        HttpStatusCode.NotFound));
            });
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
            return _handler(
                request,
                cancellationToken);
        }
    }
}
