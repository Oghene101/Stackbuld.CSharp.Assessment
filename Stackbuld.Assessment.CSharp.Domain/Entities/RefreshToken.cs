using System.ComponentModel.DataAnnotations;

namespace Stackbuld.Assessment.CSharp.Domain.Entities;

public class RefreshToken : EntityBase
{
    [Required, MaxLength(50)] public string TokenHash { get; set; } = string.Empty;
    [Required] public DateTimeOffset ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsUsed { get; set; }
    [Required] public Guid UserId { get; set; }

    //Navigation props
    public User User { get; set; } = null!;
}