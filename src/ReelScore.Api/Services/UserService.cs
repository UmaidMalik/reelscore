using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;
using ReelScore.Api.Repositories;

namespace ReelScore.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public UserDto MapUserToDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email
        };
    }

    public Task<IEnumerable<User>> GetAllUsers()
    {
        return _userRepository.GetAllUsersAsync();
    }

    public Task<User?> GetUserById(long userId)
    {
        return _userRepository.GetUserByIdAsync(userId);
    }

    public Task<User?> AddUser(User user)
    {
        return _userRepository.AddUserAsync(user);
    }

    public Task<User?> UpdateUser(long userId, User user)
    {
        return _userRepository.UpdateUserAsync(userId, user);
    }

    public Task<User?> DeleteUser(long userId)
    {
        return _userRepository.DeleteUserAsync(userId);
    }

    public Task<IEnumerable<User>> GetUsersByUsername(string username)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetUsersByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserExists(long userId)
    {
        return _userRepository.UserExistsAsync(userId);
    }
}
