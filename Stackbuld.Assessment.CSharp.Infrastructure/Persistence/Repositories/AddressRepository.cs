using Dapper;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Repositories;

public class AddressRepository(
    IUnitOfWork uOw) : IAddressRepository
{
    public async Task<IEnumerable<Address>> GetAddressesAsync(Guid kycVerificationId)
    {
        var connection = uOw.DbConnection;
        var sql = """
                  SELECT * FROM Addresses 
                           WHERE KycVerificationId = @kycVerificationId
                  """;

        var result = await connection.QueryAsync<Address>(sql, new { kycVerificationId }, uOw.DbTransaction);
        return result;
    }

    public async Task<IEnumerable<Address>> GetMostRecentAddressAsync(Guid kycVerificationId)
    {
        var connection = uOw.DbConnection;
        var sql = """
                  SELECT top (1) * FROM "Addresses" 
                           WHERE "KycVerificationId" = @kycVerificationId
                           ORDER BY "CreatedAt" DESC
                  """;

        var result = await connection.QueryAsync<Address>(sql, new { kycVerificationId }, uOw.DbTransaction);
        return result;
    }

    public async Task<bool> AddressExistsAsync(Guid kycVerificationId, string houseNumber,
        string street, string city, string state, string country)
    {
        var connection = uOw.DbConnection;
        var sql = """
                  SELECT 1 FROM "Addresses" 
                           WHERE "KycVerificationId" = @kycVerificationId
                           AND "HouseNumber" = @houseNumber
                           AND "Street" = @street
                           AND "City" = @city
                           AND "State" = @state
                           AND "Country" = @country
                  """;

        var result = await connection.ExecuteScalarAsync<int?>(sql,
            new { kycVerificationId, houseNumber, street, city, state, country }, uOw.DbTransaction);

        return result.HasValue;
    }
}