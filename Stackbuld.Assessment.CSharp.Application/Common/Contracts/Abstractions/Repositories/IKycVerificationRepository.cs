using LoanApplication.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

public interface IKycVerificationRepository
{
    Task<KycVerification?> GetKycVerificationAsync(Guid userId);
    Task<KycVerification?> GetKycVerificationWithAddressesAsync(Guid userId);
}