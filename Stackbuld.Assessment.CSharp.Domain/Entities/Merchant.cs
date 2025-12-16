using System.ComponentModel.DataAnnotations;

namespace Stackbuld.Assessment.CSharp.Domain.Entities;

public class Merchant : EntityBase
{
    [Required, MaxLength(200)] public string BusinessName { get; set; } = string.Empty;
    public Guid UserId { get; set; }

    //Navigation property
    public User User { get; set; } = null!;
}