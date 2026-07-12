using ReelScore.Api.Models;

namespace ReelScore.Api.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyCollection<User>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<User?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<User> AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<User?> UpdateAsync(
        long userId,
        string username,
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<bool> UsernameExistsAsync(
        string username,
        long? excludingUserId = null,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        string email,
        long? excludingUserId = null,
        CancellationToken cancellationToken = default);
}
