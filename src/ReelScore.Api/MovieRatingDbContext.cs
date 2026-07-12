using Microsoft.EntityFrameworkCore;
using ReelScore.Api.Models;

namespace ReelScore.Api;

public class MovieRatingDbContext : DbContext
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

        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Movie>().ToTable("movies");
        modelBuilder.Entity<Rating>().ToTable("ratings");

        modelBuilder.Entity<Rating>()
            .HasIndex(rating => new
            {
                rating.UserId,
                rating.MovieId
            })
            .IsUnique();

        modelBuilder.Entity<Rating>()
            .HasOne(rating => rating.User)
            .WithMany(user => user.Ratings)
            .HasForeignKey(rating => rating.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Rating>()
            .HasOne(rating => rating.Movie)
            .WithMany(movie => movie.Ratings)
            .HasForeignKey(rating => rating.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
