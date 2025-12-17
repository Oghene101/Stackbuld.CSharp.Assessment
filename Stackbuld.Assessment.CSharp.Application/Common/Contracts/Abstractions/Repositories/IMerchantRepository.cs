namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

public interface IMerchantRepository
{
    Task<string?> GetMerchantNameByIdAsync(Guid id);
    Task<Guid?> GetMerchantIdByUserIdAsync(Guid userId);
}