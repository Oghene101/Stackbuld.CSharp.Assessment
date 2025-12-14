using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stackbuld.Assessment.CSharp.Domain.Entities;

public class Product : EntityBase
{
    [Required, MaxLength(500)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(1000)] public string Description { get; set; } = string.Empty;

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required] public int StockQuantity { get; set; }
    public Guid MerchantId { get; set; }

    //Navigation property
    public Merchant Merchant { get; set; } = null!;
}