using Microsoft.EntityFrameworkCore;
using ReelScore.Api.Models;

namespace ReelScore.Api;

public sealed class MovieRatingDbContext : DbContext
{
    public MovieRatingDbContext(
        DbContextOptions<MovieRatingDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Movie> Movies => Set<Movie>();

    public DbSet<Rating> Ratings => Set<Rating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureMovies(modelBuilder);
        ConfigureRatings(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();

        user.ToTable("users");

        user.HasKey(entity => entity.UserId);

        user.Property(entity => entity.UserId)
            .HasColumnName("user_id")
            .ValueGeneratedOnAdd();

        user.Property(entity => entity.Username)
            .HasColumnName("username")
            .HasMaxLength(50)
            .IsRequired();

        user.Property(entity => entity.Email)
            .HasColumnName("email")
            .HasMaxLength(254)
            .IsRequired();

        user.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        user.HasIndex(entity => entity.Username)
            .IsUnique();

        user.HasIndex(entity => entity.Email)
            .IsUnique();
    }

    private static void ConfigureMovies(ModelBuilder modelBuilder)
    {
        var movie = modelBuilder.Entity<Movie>();

        movie.ToTable("movies");

        movie.HasKey(entity => entity.MovieId);

        movie.Property(entity => entity.MovieId)
            .HasColumnName("movie_id")
            .ValueGeneratedOnAdd();

        movie.Property(entity => entity.Title)
            .HasColumnName("title")
            .HasMaxLength(255)
            .IsRequired();

        movie.Property(entity => entity.Summary)
            .HasColumnName("summary")
            .HasMaxLength(5000);

        movie.Property(entity => entity.ReleaseYear)
            .HasColumnName("release_year")
            .IsRequired();

        movie.Property(entity => entity.TmdbId)
            .HasColumnName("tmdb_id");

        movie.Property(entity => entity.OriginalTitle)
            .HasColumnName("original_title")
            .HasMaxLength(255);

        movie.Property(entity => entity.ReleaseDate)
            .HasColumnName("release_date");

        movie.Property(entity => entity.RuntimeMinutes)
            .HasColumnName("runtime_minutes");

        movie.Property(entity => entity.PosterPath)
            .HasColumnName("poster_path")
            .HasMaxLength(500);

        movie.Property(entity => entity.BackdropPath)
            .HasColumnName("backdrop_path")
            .HasMaxLength(500);

        movie.Property(entity => entity.Genres)
            .HasColumnName("genres")
            .HasColumnType("text[]")
            .IsRequired();

        movie.HasIndex(entity => entity.TmdbId)
            .IsUnique();
    }

    private static void ConfigureRatings(ModelBuilder modelBuilder)
    {
        var rating = modelBuilder.Entity<Rating>();

        rating.ToTable("ratings");

        rating.HasKey(entity => entity.RatingId);

        rating.Property(entity => entity.RatingId)
            .HasColumnName("rating_id")
            .ValueGeneratedOnAdd();

        rating.Property(entity => entity.Score)
            .HasColumnName("score")
            .IsRequired();

        rating.Property(entity => entity.Review)
            .HasColumnName("review")
            .HasMaxLength(2000);

        rating.Property(entity => entity.RatedAt)
            .HasColumnName("rated_at")
            .IsRequired();

        rating.Property(entity => entity.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        rating.Property(entity => entity.MovieId)
            .HasColumnName("movie_id")
            .IsRequired();

        rating.HasIndex(entity => new
        {
            entity.UserId,
            entity.MovieId
        })
            .IsUnique();

        rating.HasOne(entity => entity.User)
            .WithMany(user => user.Ratings)
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        rating.HasOne(entity => entity.Movie)
            .WithMany(movie => movie.Ratings)
            .HasForeignKey(entity => entity.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        rating.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_ratings_score_range",
                "\"score\" >= 1 AND \"score\" <= 10");
        });
    }
}
