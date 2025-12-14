namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts;

public static class Product
{
    public record CreateProductRequest(string Name, string Description, decimal Price, int StockQuantity);

    public record GetProductByIdResponse(
        Guid MerchantId,
        string MerchantName,
        Guid ProductId,
        string ProductName,
        string Description,
        decimal Price,
        int StockQuantity);

    public record GetProductsRequest(
        DateTimeOffset? StartDate,
        DateTimeOffset? EndDate,
        int PageNumber = 1,
        int PageSize = 10);

    public record GetProductsResponse(
        Guid ProductId,
        string ProductName,
        decimal Price);

    public record UpdateProductRequest(
        string ProductName,
        string Description,
        decimal Price,
        int StockQuantity);

    public record GetProductsByMerchantIdRequest(
        DateTimeOffset? StartDate,
        DateTimeOffset? EndDate,
        int PageNumber = 1,
        int PageSize = 10);
}