using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stackbuld.Assessment.CSharp.Domain.Entities;

public class Order : EntityBase
{
    public Guid UserId { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    //Navigation property
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}