using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;
using ReelScore.Api.Repositories;

namespace ReelScore.Api.Services;

public sealed class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
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
            ReleaseYear = request.ReleaseYear
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

    private static MovieResponse MapToResponse(Movie movie)
    {
        var ratingCount = movie.Ratings.Count;

        var averageRating = ratingCount == 0
            ? 0
            : movie.Ratings.Average(rating => rating.Score);

        return new MovieResponse
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            Summary = movie.Summary,
            ReleaseYear = movie.ReleaseYear,
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
}
