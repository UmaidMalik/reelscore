using Microsoft.EntityFrameworkCore;
using ReelScore.Api.Models;

namespace ReelScore.Api;

public class MovieRatingDbContext : DbContext
{
    public MovieRatingDbContext(DbContextOptions<MovieRatingDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Movie>().ToTable("movies");
        modelBuilder.Entity<Rating>().ToTable("ratings");
    }
}