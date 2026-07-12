using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;
using ReelScore.Api.Repositories;

namespace ReelScore.Api.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<UserResponse>> GetAllUsersAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        return users
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<UserResponse?> GetUserByIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            userId,
            cancellationToken);

        return user is null
            ? null
            : MapToResponse(user);
    }

    public async Task<CreateUserResult> CreateUserAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedUsername = request.Username.Trim();
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _userRepository.EmailExistsAsync(
            normalizedEmail,
            cancellationToken: cancellationToken);

        if (emailExists)
        {
            return new CreateUserResult(
                CreateUserStatus.EmailAlreadyExists);
        }

        var usernameExists = await _userRepository.UsernameExistsAsync(
            normalizedUsername,
            cancellationToken: cancellationToken);

        if (usernameExists)
        {
            return new CreateUserResult(
                CreateUserStatus.UsernameAlreadyExists);
        }

        var user = new User
        {
            Username = normalizedUsername,
            Email = normalizedEmail,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(
            user,
            cancellationToken);

        return new CreateUserResult(
            CreateUserStatus.Created,
            MapToResponse(createdUser));
    }

    public async Task<UpdateUserResult> UpdateUserAsync(
        long userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByIdAsync(
            userId,
            cancellationToken);

        if (existingUser is null)
        {
            return new UpdateUserResult(
                UpdateUserStatus.NotFound);
        }

        var normalizedUsername = request.Username.Trim();
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _userRepository.EmailExistsAsync(
            normalizedEmail,
            excludingUserId: userId,
            cancellationToken: cancellationToken);

        if (emailExists)
        {
            return new UpdateUserResult(
                UpdateUserStatus.EmailAlreadyExists);
        }

        var usernameExists = await _userRepository.UsernameExistsAsync(
            normalizedUsername,
            excludingUserId: userId,
            cancellationToken: cancellationToken);

        if (usernameExists)
        {
            return new UpdateUserResult(
                UpdateUserStatus.UsernameAlreadyExists);
        }

        var updatedUser = await _userRepository.UpdateAsync(
            userId,
            normalizedUsername,
            normalizedEmail,
            cancellationToken);

        if (updatedUser is null)
        {
            return new UpdateUserResult(
                UpdateUserStatus.NotFound);
        }

        return new UpdateUserResult(
            UpdateUserStatus.Updated,
            MapToResponse(updatedUser));
    }

    public Task<bool> DeleteUserAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return _userRepository.DeleteAsync(
            userId,
            cancellationToken);
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
}
