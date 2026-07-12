using ReelScore.Api.Models;

namespace ReelScore.Api.Repositories;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetAllMoviesAsync();
    Task<Movie?> GetMovieByIdAsync(long id);
    Task<Movie?> AddMovieAsync(Movie movie);
    Task<Movie?> UpdateMovieAsync(long id, Movie movie);
    Task<Movie?> DeleteMovieAsync(long id);
    Task<IEnumerable<Movie>> GetMoviesByTitleAsync(string title);
    Task<IEnumerable<Movie>> GetMoviesByReleaseYearAsync(int releaseYear);
    Task<bool> MovieExistsAsync(long id);
}
