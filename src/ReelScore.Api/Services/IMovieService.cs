using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;

namespace ReelScore.Api.Services;

public interface IMovieService
{
    MovieDto MapMovieToDto(Movie movie);
    Task<IEnumerable<Movie>> GetAllMovies();
    Task<Movie?> GetMovieById(long id);
    Task<Movie?> AddMovie(Movie movie);
    Task<Movie?> UpdateMovie(long id, Movie movie);
    Task<Movie?> DeleteMovie(long id);
    Task<IEnumerable<Movie>> GetMoviesByTitle(string title);
    Task<IEnumerable<Movie>> GetMoviesByReleaseYear(int releaseYear);

    Task<bool> MovieExists(long id);
}