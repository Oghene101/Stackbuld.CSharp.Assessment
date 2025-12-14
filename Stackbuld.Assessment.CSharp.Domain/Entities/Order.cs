using System.ComponentModel.DataAnnotations;

namespace Stackbuld.Assessment.CSharp.Domain.Entities;

public class Order : EntityBase
{
    public Guid UserId { get; set; }
    [Required] public decimal TotalAmount { get; set; }

    //Navigation property
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}