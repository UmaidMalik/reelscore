using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReelScore.Api.Models;

public class Movie
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("movie_id")]
    public long MovieId
    {
        get; set;
    }

    [Column("tmdb_id")]
    public int? TmdbId
    {
        get; set;
    }

    [Required]
    [StringLength(255)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [StringLength(255)]
    [Column("original_title")]
    public string? OriginalTitle
    {
        get; set;
    }

    [StringLength(5000)]
    [Column("summary")]
    public string? Summary
    {
        get; set;
    }

    [Column("release_date")]
    public DateOnly? ReleaseDate
    {
        get; set;
    }

    [Required]
    [Range(1888, 2100)]
    [Column("release_year")]
    public int? ReleaseYear
    {
        get; set;
    }

    [Column("runtime_minutes")]
    public int? RuntimeMinutes
    {
        get; set;
    }

    [StringLength(500)]
    [Column("poster_path")]
    public string? PosterPath
    {
        get; set;
    }

    [StringLength(500)]
    [Column("backdrop_path")]
    public string? BackdropPath
    {
        get; set;
    }

    [Column("genres", TypeName = "text[]")]
    public string[] Genres { get; set; } = Array.Empty<string>();

    [Required]
    [Column("media_type")]
    public MediaType MediaType { get; set; } = MediaType.Movie;

    [Column("number_of_seasons")]
    public int? NumberOfSeasons
    {
        get; set;
    }

    [Column("number_of_episodes")]
    public int? NumberOfEpisodes
    {
        get; set;
    }

    public ICollection<Rating> Ratings
    {
        get; set;
    } =
        new List<Rating>();
}
