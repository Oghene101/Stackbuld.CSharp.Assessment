using Dapper;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Repositories;

public class MerchantRepository(
    IUnitOfWork uOw) : IMerchantRepository
{
    public async Task<string?> GetMerchantNameById(Guid id)
    {
        var connection = uOw.DbConnection;
        var sql = """
                  SELECT "Name" FROM "Merchants" 
                          WHERE "Id" = @id
                  """;

        var result = await connection.ExecuteScalarAsync<string>(sql, new { id }, uOw.DbTransaction);
        return result;
    }
}