using Microsoft.EntityFrameworkCore;
using ReelScore.Api.Models;

namespace ReelScore.Api.Repositories;

public sealed class MovieRepository : IMovieRepository
{
    private readonly MovieRatingDbContext _context;

    public MovieRepository(MovieRatingDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Movie>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Movies
            .AsNoTracking()
            .Include(movie => movie.Ratings)
            .OrderBy(movie => movie.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<Movie?> GetByIdAsync(
        long movieId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Movies
            .AsNoTracking()
            .Include(movie => movie.Ratings)
            .FirstOrDefaultAsync(
                movie => movie.MovieId == movieId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Movie>> SearchAsync(
        string? title,
        int? releaseYear,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Movies
            .AsNoTracking()
            .Include(movie => movie.Ratings)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            var normalizedTitle = title.Trim();

            query = query.Where(movie =>
                EF.Functions.ILike(
                    movie.Title,
                    $"%{normalizedTitle}%"));
        }

        if (releaseYear.HasValue)
        {
            query = query.Where(movie =>
                movie.ReleaseYear == releaseYear.Value);
        }

        return await query
            .OrderBy(movie => movie.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<Movie> AddAsync(
        Movie movie,
        CancellationToken cancellationToken = default)
    {
        _context.Movies.Add(movie);

        await _context.SaveChangesAsync(cancellationToken);

        return movie;
    }

    public async Task<Movie?> UpdateAsync(
        long movieId,
        string title,
        string? summary,
        int releaseYear,
        CancellationToken cancellationToken = default)
    {
        var movie = await _context.Movies
            .FirstOrDefaultAsync(
                existingMovie => existingMovie.MovieId == movieId,
                cancellationToken);

        if (movie is null)
        {
            return null;
        }

        movie.Title = title;
        movie.Summary = summary;
        movie.ReleaseYear = releaseYear;

        await _context.SaveChangesAsync(cancellationToken);

        return movie;
    }

    public async Task<bool> DeleteAsync(
        long movieId,
        CancellationToken cancellationToken = default)
    {
        var movie = await _context.Movies
            .FirstOrDefaultAsync(
                existingMovie => existingMovie.MovieId == movieId,
                cancellationToken);

        if (movie is null)
        {
            return false;
        }

        _context.Movies.Remove(movie);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public Task<bool> ExistsAsync(
        long movieId,
        CancellationToken cancellationToken = default)
    {
        return _context.Movies.AnyAsync(
            movie => movie.MovieId == movieId,
            cancellationToken);
    }
}
