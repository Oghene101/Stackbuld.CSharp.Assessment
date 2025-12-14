namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

public interface IProductWriteRepository : IRepository<Domain.Entities.Product>
{
    Task<bool> TryReduceStockAsync(Guid productId, int quantity,
        CancellationToken cancellationToken = default);
}