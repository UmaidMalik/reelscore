using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReelScore.Api.Models;

public class Movie
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("movie_id")]
    public long MovieId { get; set; }
    
    [Required]
    [StringLength(255)]
    [Column("title")]
    public string? Title { get; set; }
    [Column("summary")]
    public string? Summary { get; set; }
    
    [Required]
    [Range(1900, 2100)]
    [Column("release_year")]
    public int ReleaseYear  { get; set; }
    
    // Navigation Property: Movie has many Ratings
    public ICollection<Rating> Ratings { get; set; }
    
    public Movie(long movieId, string title, string summary, int releaseYear)
    {
        MovieId = movieId;
        Title = title;
        Summary = summary;
        ReleaseYear = releaseYear;
    }
    
    public Movie(string title, string summary, int releaseYear)
    {
        Title = title;
        Summary = summary;
        ReleaseYear = releaseYear;
    }
    
    public Movie()
    {
    }
    
    public override string ToString()
    {
        return $"MovieId: {MovieId}, Title: {Title}, Summary: {Summary}, ReleaseYear: {ReleaseYear}";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        Movie movie = (Movie) obj;
        return MovieId == movie.MovieId && Title == movie.Title && Summary == movie.Summary && ReleaseYear == movie.ReleaseYear;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(MovieId, Title, Summary, ReleaseYear);
    }
}