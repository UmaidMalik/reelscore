using ReelScore.Api.DataTransferObjects;

namespace ReelScore.Api.Services;

public interface IUserService
{
    Task<IReadOnlyCollection<UserResponse>> GetAllUsersAsync(
        CancellationToken cancellationToken = default);

    Task<UserResponse?> GetUserByIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<CreateUserResult> CreateUserAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default);

    Task<UpdateUserResult> UpdateUserAsync(
        long userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteUserAsync(
        long userId,
        CancellationToken cancellationToken = default);
}
