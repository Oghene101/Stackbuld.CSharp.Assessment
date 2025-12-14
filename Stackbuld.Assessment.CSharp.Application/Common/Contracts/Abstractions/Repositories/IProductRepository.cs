namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

public interface IProductRepository
{
    Task<Domain.Entities.Product?> GetProductByIdAsync(Guid id);

    Task<IEnumerable<Domain.Entities.Product>> GetProductsByIdsAsync(IEnumerable<Guid> ids);

    Task<(IEnumerable<Domain.Entities.Product> Products, int TotalCount)> GetProductsByMerchantIdAsync(
        Guid merchantId,
        int pageNumber, int pageSize,
        DateTimeOffset? startDate, DateTimeOffset? endDate);

    Task<(IEnumerable<Domain.Entities.Product> Products, int TotalCount)> GetProductsAsync(
        int pageNumber, int pageSize,
        DateTimeOffset? startDate, DateTimeOffset? endDate);
}