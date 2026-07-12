using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;
using ReelScore.Api.Repositories;

namespace ReelScore.Api.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public MovieDto MapMovieToDto(Movie movie)
    {
        return new MovieDto
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            Summary = movie.Summary,
            ReleaseYear = movie.ReleaseYear
        };
    }

    public Task<IEnumerable<Movie>> GetAllMovies()
    {
        return _movieRepository.GetAllMoviesAsync();
    }

    public Task<Movie?> GetMovieById(long id)
    {
        return _movieRepository.GetMovieByIdAsync(id);
    }

    public Task<Movie?> AddMovie(Movie movie)
    {
        return _movieRepository.AddMovieAsync(movie);
    }

    public Task<Movie?> UpdateMovie(long id, Movie movie)
    {
        return _movieRepository.UpdateMovieAsync(id, movie);
    }

    public Task<Movie?> DeleteMovie(long id)
    {
        return _movieRepository.DeleteMovieAsync(id);
    }

    public Task<IEnumerable<Movie>> GetMoviesByTitle(string title)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Movie>> GetMoviesByReleaseYear(int releaseYear)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MovieExists(long id)
    {
        return _movieRepository.MovieExistsAsync(id);
    }
}
