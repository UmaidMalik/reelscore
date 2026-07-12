using System.ComponentModel.DataAnnotations;

namespace ReelScore.Api.DataTransferObjects;

public sealed class CreateMovieRequest
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    [StringLength(5000)]
    public string? Summary
    {
        get; init;
    }

    [Range(1888, 2100)]
    public int ReleaseYear
    {
        get; init;
    }
}
