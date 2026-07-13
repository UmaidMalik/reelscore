using System.ComponentModel.DataAnnotations;

namespace ReelScore.Api.DataTransferObjects;

public sealed class SearchExternalMoviesRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Query { get; init; } = string.Empty;

    [Range(1, 500)]
    public int Page { get; init; } = 1;

    [StringLength(10)]
    public string Language { get; init; } = "en-US";

    [Range(1888, 2100)]
    public int? ReleaseYear
    {
        get; init;
    }
}
