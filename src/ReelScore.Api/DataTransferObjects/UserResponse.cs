namespace ReelScore.Api.DataTransferObjects;

public sealed class UserResponse
{
    public long UserId
    {
        get; init;
    }

    public string Username { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public DateTime CreatedAt
    {
        get; init;
    }
}
