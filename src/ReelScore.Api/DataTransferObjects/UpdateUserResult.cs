using ReelScore.Api.DataTransferObjects;

namespace ReelScore.Api.Services;

public enum UpdateUserStatus
{
    Updated,
    NotFound,
    EmailAlreadyExists,
    UsernameAlreadyExists
}

public sealed record UpdateUserResult(
    UpdateUserStatus Status,
    UserResponse? User = null);
