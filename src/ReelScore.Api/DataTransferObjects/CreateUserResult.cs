using ReelScore.Api.DataTransferObjects;

namespace ReelScore.Api.Services;

public enum CreateUserStatus
{
    Created,
    EmailAlreadyExists,
    UsernameAlreadyExists
}

public sealed record CreateUserResult(
    CreateUserStatus Status,
    UserResponse? User = null);
