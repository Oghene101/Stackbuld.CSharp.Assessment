using System.Data;
using Dapper;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Repositories;

public class MerchantRepository(
    IDbConnection connection,
    IDbTransaction? transaction) : IMerchantRepository
{
    public async Task<string?> GetMerchantNameById(Guid id)
    {
        var sql = """
                  SELECT "Name" FROM "Merchants" 
                          WHERE "Id" = @id
                  """;

        var result = await connection.ExecuteScalarAsync<string>(sql, new { id }, transaction);
        return result;
    }
}