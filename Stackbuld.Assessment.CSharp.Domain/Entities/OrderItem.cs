using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stackbuld.Assessment.CSharp.Domain.Entities;

public class OrderItem : EntityBase
{
    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Required] public int Quantity { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }

    //Navigation Property
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}