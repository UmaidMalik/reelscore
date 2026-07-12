using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace ReelScore.Api.Models;

public class Rating
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("rating_id")]
    public long RatingId
    {
        get; set;
    }

    [Required]
    [Range(1, 10)]
    [Column("rating")]
    public int MovieRating
    {
        get; set;
    }

    [Column("review")]
    public string? Review
    {
        get; set;
    }

    [Column("rated_at")]
    public DateTime RatedAt { get; set; } = DateTime.Now;

    // Foreign Key: Rating belongs to a User
    [Required]
    [Column("user_id")]
    public long UserId
    {
        get; set;
    }
    public User User
    {
        get; set;
    } // Navigation Property: Rating belongs to a User

    // Foreign Key: Rating belongs to a Movie
    [Required]
    [Column("movie_id")]
    public long MovieId
    {
        get; set;
    }
    public Movie Movie
    {
        get; set;
    } // Navigation Property: Rating belongs to a Movie

    public Rating(long ratingId, int movieRating, string review, long userId, long movieId)
    {
        RatingId = ratingId;
        MovieRating = movieRating;
        Review = review;
        UserId = userId;
        MovieId = movieId;
    }

    public Rating(int movieRating, string review, long userId, long movieId)
    {
        MovieRating = movieRating;
        Review = review;
        UserId = userId;
        MovieId = movieId;
    }


}
