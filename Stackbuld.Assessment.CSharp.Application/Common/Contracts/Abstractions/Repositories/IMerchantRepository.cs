namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

public interface IMerchantRepository
{
    Task<string?> GetMerchantNameById(Guid id);
}