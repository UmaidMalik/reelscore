using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ReelScore.Api.Models;
using Npgsql;

namespace ReelScore.Api.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly MovieRatingDbContext _context;
    
    public MovieRepository(MovieRatingDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
    {   
        return await _context.Movies
            .Include(m => m.Ratings)
            .ToListAsync();
    }
    
    public async Task<Movie?> GetMovieByIdAsync(long id)
    {
        if (!await MovieExistsAsync(id))
        {
            throw new ArgumentException($"Movie with id {id} not found");
        }
        return await _context.Movies
            .Include(m => m.Ratings).FirstOrDefaultAsync(m => m.MovieId == id);
    }

    public async Task<Movie?> AddMovieAsync(Movie movie)
    {
        // Adding the movie to the context
        var entityToAdd = _context.Movies.Add(movie);
        
        // asynchronously saving the changes to the database
        await _context.SaveChangesAsync();
        
        // returning the added movie
        return entityToAdd.Entity;
    }

    public async Task<Movie?> UpdateMovieAsync(long id, Movie movie)
    {
        var existingMovie = await GetMovieByIdAsync(id);
        if (existingMovie == null)
        {
            throw new ArgumentException($"Movie with id {id} not found");
        }
        _context.Entry(existingMovie).CurrentValues.SetValues(movie);
        await _context.SaveChangesAsync();
        return existingMovie;
    }

    public async Task<Movie?> DeleteMovieAsync(long id)
    {
        if (!await MovieExistsAsync(id)) 
        {
            throw new ArgumentException($"Movie with id {id} not found");
        }
        var movie = await GetMovieByIdAsync(id);
        if (movie != null)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }
        return movie;
    }

    public Task<IEnumerable<Movie>> GetMoviesByTitleAsync(string title)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Movie>> GetMoviesByReleaseYearAsync(int releaseYear)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MovieExistsAsync(long id)
    {
        return await _context.Movies.AnyAsync(m => m.MovieId == id);
    }
    
}