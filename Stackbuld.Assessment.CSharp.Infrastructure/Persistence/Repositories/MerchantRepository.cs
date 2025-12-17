using System.Data;
using Dapper;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Repositories;

public class MerchantRepository(
    IDbConnection connection,
    IDbTransaction? transaction) : IMerchantRepository
{
    public async Task<string?> GetMerchantNameByIdAsync(Guid id)
    {
        var sql = """
                  SELECT "BusinessName" FROM "Merchants" 
                          WHERE "Id" = @id
                  """;

        var result = await connection.ExecuteScalarAsync<string>(sql, new { id }, transaction);
        return result;
    }

    public async Task<Guid?> GetMerchantIdByUserIdAsync(Guid userId)
    {
        var sql = """
                  SELECT "Id" FROM "Merchants" 
                          WHERE "UserId" = @userId
                  """;

        var result = await connection.ExecuteScalarAsync<Guid>(sql, new { userId }, transaction);
        return result;
    }
}