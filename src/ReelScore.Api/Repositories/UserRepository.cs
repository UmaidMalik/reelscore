using Microsoft.EntityFrameworkCore;
using ReelScore.Api.Models;

namespace ReelScore.Api.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly MovieRatingDbContext _context;

    public UserRepository(MovieRatingDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<User>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(user => user.Username)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.UserId == userId,
                cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim();

        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => EF.Functions.ILike(
                    user.Username,
                    normalizedUsername),
                cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim();

        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => EF.Functions.ILike(
                    user.Email,
                    normalizedEmail),
                cancellationToken);
    }

    public async Task<User> AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<User?> UpdateAsync(
        long userId,
        string username,
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(
                existingUser => existingUser.UserId == userId,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        user.Username = username.Trim();
        user.Email = email.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<bool> DeleteAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(
                existingUser => existingUser.UserId == userId,
                cancellationToken);

        if (user is null)
        {
            return false;
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public Task<bool> ExistsAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Users.AnyAsync(
            user => user.UserId == userId,
            cancellationToken);
    }

    public Task<bool> UsernameExistsAsync(
        string username,
        long? excludingUserId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim();

        return _context.Users.AnyAsync(
            user =>
                EF.Functions.ILike(
                    user.Username,
                    normalizedUsername)
                && (!excludingUserId.HasValue
                    || user.UserId != excludingUserId.Value),
            cancellationToken);
    }

    public Task<bool> EmailExistsAsync(
        string email,
        long? excludingUserId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim();

        return _context.Users.AnyAsync(
            user =>
                EF.Functions.ILike(
                    user.Email,
                    normalizedEmail)
                && (!excludingUserId.HasValue
                    || user.UserId != excludingUserId.Value),
            cancellationToken);
    }
}
