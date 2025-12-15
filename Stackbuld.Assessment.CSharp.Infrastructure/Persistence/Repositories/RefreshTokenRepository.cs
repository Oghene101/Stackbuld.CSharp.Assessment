using System.Data;
using Dapper;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(
    IDbConnection connection,
    IDbTransaction? transaction) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        var sql = """
                  SELECT * FROM "RefreshTokens" 
                           WHERE "Token" = @token
                           AND "IsRevoked" = 0
                           AND "IsUsed" = 0
                           AND "ExpiresAt" > SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00')
                  """;

        var result = await connection.QuerySingleOrDefaultAsync<RefreshToken>(sql, new { token }, transaction);
        return result;
    }
}