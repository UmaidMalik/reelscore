namespace ReelScore.Api.DataTransferObjects;

public class UserDto
{
    public long UserId
    {
        get; set;
    }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
