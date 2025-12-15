namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts;

public static class Cart
{
    public record AddToCartRequest(int Quantity);

    public record GetCartByUserIdResponse(
        Guid CartId,
        CartItemVm[] CartItems)
    {
        public decimal TotalAmount => CartItems.Sum(x => x.TotalPrice);
    };

    public record CartItemVm(
        Guid CartItemId,
        Guid ProductId,
        string ProductName,
        decimal UnitPrice,
        int Quantity)
    {
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}