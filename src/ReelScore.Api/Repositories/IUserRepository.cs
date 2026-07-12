using ReelScore.Api.Models;

namespace ReelScore.Api.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(long id);
    Task<User?> AddUserAsync(User user);
    Task<User?> UpdateUserAsync(long id, User user);
    Task<User?> DeleteUserAsync(long id);
    Task<IEnumerable<User>> GetUsersByUsernameAsync(string username);
    Task<IEnumerable<User>> GetUsersByEmailAsync(string email);
    Task<bool> UserExistsAsync(long id);
}
