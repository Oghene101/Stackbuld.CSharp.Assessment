namespace Stackbuld.Assessment.CSharp.Domain.Entities;

public class Cart : EntityBase
{
    public Guid UserId { get; set; }

    //Navigation property
    public ICollection<CartItem> CartItems { get; set; } = [];
}