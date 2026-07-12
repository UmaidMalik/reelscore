using System.ComponentModel.DataAnnotations;

namespace ReelScore.Api.DataTransferObjects;

public sealed class UpdateUserRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; init; } = string.Empty;
}
