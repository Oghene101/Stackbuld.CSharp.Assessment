using System.Data;
using Dapper;
using LoanApplication.Domain.Entities;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Repositories;

public class KycVerificationRepository(
    IDbConnection connection,
    IDbTransaction? transaction) : IKycVerificationRepository
{
    public async Task<KycVerification?> GetKycVerificationAsync(Guid userId)
    {
        var sql = """
                  SELECT * FROM "KycVerifications" 
                           WHERE "UserId" = @userId
                  """;

        var result =
            await connection.QuerySingleOrDefaultAsync<KycVerification>(sql, new { userId }, transaction);
        return result;
    }

    public async Task<KycVerification?> GetKycVerificationWithAddressesAsync(Guid userId)
    {
        var sql = """
                  SELECT * FROM "KycVerifications" as K
                           LEFT JOIN "Addresses" as A ON K."Id" = A."KycVerificationId"
                           WHERE "UserId" = @userId
                  """;

        var kycDictionary = new Dictionary<Guid, KycVerification>();
        await connection.QueryAsync<KycVerification, Address, KycVerification>(
            sql,
            (kyc, address) =>
            {
                if (!kycDictionary.TryGetValue(kyc.Id, out var currentKyc))
                {
                    currentKyc = kyc;
                    kycDictionary.Add(kyc.Id, currentKyc);
                }

                if (address is not null)
                {
                    currentKyc.Addresses.Add(address);
                }

                return currentKyc;
            },
            new { userId },
            transaction,
            splitOn: "Id");

        return kycDictionary.Values.SingleOrDefault();
    }
}