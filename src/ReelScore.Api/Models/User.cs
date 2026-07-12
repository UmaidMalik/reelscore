using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReelScore.Api.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("user_id")]
    public long UserId
    {
        get; set;
    }

    [Required]
    [StringLength(50)]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public User(long userId, string username, string email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    }

    public User(string username, string email)
    {
        Username = username;
        Email = email;
    }

    public User()
    {
    }
}
