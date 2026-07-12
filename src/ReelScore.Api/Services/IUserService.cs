using ReelScore.Api.Models;

namespace ReelScore.Api.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> GetUserById(long userId);
    Task<User?> AddUser(User user);
    Task<User?> UpdateUser(long userId, User user);
    Task<User?> DeleteUser(long userId);
    Task<IEnumerable<User>> GetUsersByUsername(string username);
    Task<IEnumerable<User>> GetUsersByEmail(string email);
    Task<bool> UserExists(long userId);
}
