using System.ComponentModel.DataAnnotations;

namespace Stackbuld.Assessment.CSharp.Domain.Entities;

public class CartItem : EntityBase
{
    [Required] public int Quantity { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }

    //Navigation property
    public Cart Cart { get; set; } = null!;
    public Product Product { get; set; } = null!;
}