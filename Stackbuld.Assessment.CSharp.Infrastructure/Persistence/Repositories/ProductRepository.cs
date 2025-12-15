using System.Data;
using Dapper;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Domain.Entities;
using Stackbuld.Assessment.CSharp.Infrastructure.Persistence.DbContexts;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Repositories;

public class ProductRepository(
    IDbConnection connection,
    IDbTransaction? transaction) : IProductRepository
{
    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        var sql = """
                  SELECT * FROM "Products" 
                           WHERE "Id" = @id
                           AND "ISDELETED" = FALSE;
                  """;

        var result = await connection.QueryFirstOrDefaultAsync<Product>(sql, new { id }, transaction);
        return result;
    }

    public async Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<Guid> ids)
    {
        var sql = """
                  SELECT * FROM "Products" 
                           WHERE "Id" = ANY(@ids)
                           AND "ISDELETED" = FALSE;
                  """;

        var result = await connection.QueryAsync<Product>(sql, new { ids }, transaction);
        return result;
    }

    public async Task<(IEnumerable<Product>, int)> GetProductsByMerchantIdAsync(
        Guid merchantId,
        int pageNumber, int pageSize,
        DateTimeOffset? startDate, DateTimeOffset? endDate)
    {
        var parameters = new DynamicParameters();
        var whereClause = """ WHERE "MerchantId" = @MerchantId AND "IsDeleted" = false """;
        parameters.Add("MerchantId", merchantId);

        if (startDate.HasValue && endDate.HasValue)
        {
            whereClause += """ AND "CreatedAt" >= @StartDate AND "CreatedAt" < @EndDate """;
            parameters.Add("StartDate", startDate.Value);
            parameters.Add("EndDate", endDate.Value);
        }

        parameters.Add("Skip", (pageNumber - 1) * pageSize);
        parameters.Add("Take", pageSize);

        var sql = $"""
                   -- Query 1: paged data
                   SELECT "Id", "Name", "Price" 
                    FROM "Products"
                   {whereClause}
                   ORDER BY "CreatedAt" DESC
                   OFFSET @Skip LIMIT @Take;

                   -- Query 2: total count
                   SELECT COUNT(*) FROM "Products"
                   {whereClause};
                   """;

        await using var reader = await connection.QueryMultipleAsync(sql, parameters, transaction);
        var products = reader.Read<Product>();
        var totalCount = await reader.ReadSingleAsync<int>();

        return (products, totalCount);
    }

    public async Task<(IEnumerable<Product>, int)> GetProductsAsync(
        int pageNumber, int pageSize,
        DateTimeOffset? startDate, DateTimeOffset? endDate)
    {
        var parameters = new DynamicParameters();
        var whereClause = """ WHERE 1=1 AND "IsDeleted" = false """;

        if (startDate.HasValue && endDate.HasValue)
        {
            whereClause += """ AND "CreatedAt" >= @StartDate AND "CreatedAt" < @EndDate """;
            parameters.Add("StartDate", startDate.Value);
            parameters.Add("EndDate", endDate.Value);
        }

        parameters.Add("Skip", (pageNumber - 1) * pageSize);
        parameters.Add("Take", pageSize);

        var sql = $"""
                   -- Query 1: paged data
                   SELECT "Id", "Name", "Price" 
                    FROM "Products"
                   {whereClause}
                   ORDER BY "CreatedAt" DESC
                   OFFSET @Skip LIMIT @Take;

                   -- Query 2: total count
                   SELECT COUNT(*) FROM "Products"
                   {whereClause};
                   """;

        await using var reader = await connection.QueryMultipleAsync(sql, parameters, transaction);
        var products = reader.Read<Product>();
        var totalCount = await reader.ReadSingleAsync<int>();

        return (products, totalCount);
    }
}