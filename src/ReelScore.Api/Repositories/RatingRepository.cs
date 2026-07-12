using Microsoft.EntityFrameworkCore;
using ReelScore.Api.Models;

namespace ReelScore.Api.Repositories;

public sealed class RatingRepository : IRatingRepository
{
    private readonly MovieRatingDbContext _context;

    public RatingRepository(MovieRatingDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Rating>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Ratings
            .AsNoTracking()
            .OrderByDescending(rating => rating.RatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Rating?> GetByIdAsync(
        long ratingId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Ratings
            .AsNoTracking()
            .FirstOrDefaultAsync(
                rating => rating.RatingId == ratingId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Rating>> GetByMovieIdAsync(
        long movieId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Ratings
            .AsNoTracking()
            .Where(rating => rating.MovieId == movieId)
            .OrderByDescending(rating => rating.RatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Rating?> GetByUserAndMovieAsync(
        long userId,
        long movieId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Ratings
            .AsNoTracking()
            .FirstOrDefaultAsync(
                rating =>
                    rating.UserId == userId
                    && rating.MovieId == movieId,
                cancellationToken);
    }

    public async Task<Rating> AddAsync(
        Rating rating,
        CancellationToken cancellationToken = default)
    {
        _context.Ratings.Add(rating);

        await _context.SaveChangesAsync(cancellationToken);

        return rating;
    }

    public async Task<Rating?> UpdateAsync(
        long ratingId,
        int score,
        string? review,
        CancellationToken cancellationToken = default)
    {
        var rating = await _context.Ratings
            .FirstOrDefaultAsync(
                existingRating =>
                    existingRating.RatingId == ratingId,
                cancellationToken);

        if (rating is null)
        {
            return null;
        }

        rating.Score = score;
        rating.Review = review;
        rating.RatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return rating;
    }

    public async Task<bool> DeleteAsync(
        long ratingId,
        CancellationToken cancellationToken = default)
    {
        var rating = await _context.Ratings
            .FirstOrDefaultAsync(
                existingRating =>
                    existingRating.RatingId == ratingId,
                cancellationToken);

        if (rating is null)
        {
            return false;
        }

        _context.Ratings.Remove(rating);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public Task<bool> ExistsForUserAndMovieAsync(
        long userId,
        long movieId,
        CancellationToken cancellationToken = default)
    {
        return _context.Ratings.AnyAsync(
            rating =>
                rating.UserId == userId
                && rating.MovieId == movieId,
            cancellationToken);
    }
}
