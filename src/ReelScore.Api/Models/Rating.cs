using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReelScore.Api.Models;

public class Rating
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("rating_id")]
    public long RatingId
    {
        get; set;
    }

    [Required]
    [Range(1, 10)]
    [Column("score")]
    public int Score
    {
        get; set;
    }

    [StringLength(2000)]
    [Column("review")]
    public string? Review
    {
        get; set;
    }

    [Column("rated_at")]
    public DateTime RatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("user_id")]
    public long UserId
    {
        get; set;
    }

    public User User { get; set; } = null!;

    [Required]
    [Column("movie_id")]
    public long MovieId
    {
        get; set;
    }

    public Movie Movie { get; set; } = null!;
}
