namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts;

public static class Cart
{
    public record AddToCartRequest(int Quantity);

    public record GetCartByUserIdResponse(
        Guid CartId);
}