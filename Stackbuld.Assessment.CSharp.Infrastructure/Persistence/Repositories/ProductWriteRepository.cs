using Microsoft.EntityFrameworkCore;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Domain.Entities;
using Stackbuld.Assessment.CSharp.Infrastructure.Persistence.DbContexts;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Repositories;

public class ProductWriteRepository(
    AppDbContext context) : Repository<Product>(context), IProductWriteRepository
{
    public async Task<bool> TryReduceStockAsync(Guid productId, int quantity,
        CancellationToken cancellationToken = default)
    {
        var rowsAffected = await DbSet
            .Where(p => p.Id == productId &&
                        !p.IsDeleted &&
                        p.StockQuantity >= quantity)
            .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(p => p.StockQuantity,
                        p => p.StockQuantity - quantity),
                cancellationToken);

        return rowsAffected > 0;
    }
}