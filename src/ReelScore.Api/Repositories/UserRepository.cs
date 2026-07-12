using Microsoft.EntityFrameworkCore;
using ReelScore.Api.Models;

namespace ReelScore.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MovieRatingDbContext _context;

    public UserRepository(MovieRatingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Ratings)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(long id)
    {
        if (!await UserExistsAsync(id))
        {
            throw new ArgumentException($"Movie with id {id} not found");
        }
        return await _context.Users
            .Include(u => u.Ratings).FirstOrDefaultAsync(u => u.UserId == id);
    }

    public async Task<User?> AddUserAsync(User user)
    {
        // Adding the user to the context
        var entityToAdd = _context.Users.Add(user);

        // asynchronously saving the changes to the database
        await _context.SaveChangesAsync();

        // returning the added user
        return entityToAdd.Entity;
    }

    public async Task<User?> UpdateUserAsync(long id, User user)
    {
        var existingUser = await GetUserByIdAsync(id);
        if (existingUser == null)
        {
            throw new ArgumentException($"User with id {id} not found");
        }
        _context.Entry(existingUser).CurrentValues.SetValues(user);
        await _context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<User?> DeleteUserAsync(long id)
    {
        if (!await UserExistsAsync(id))
        {
            throw new ArgumentException($"User with id {id} not found");
        }
        var user = await GetUserByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        return user;
    }

    public Task<IEnumerable<User>> GetUsersByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetUsersByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UserExistsAsync(long id)
    {
        return await _context.Movies.AnyAsync(m => m.MovieId == id);
    }
}