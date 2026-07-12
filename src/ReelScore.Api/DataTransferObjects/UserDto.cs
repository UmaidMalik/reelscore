namespace ReelScore.Api.DataTransferObjects;

public class UserDto
{
    public long UserId
    {
        get; set;
    }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
}
