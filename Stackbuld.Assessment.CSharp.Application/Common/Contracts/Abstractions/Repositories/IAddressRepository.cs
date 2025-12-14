using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

public interface IAddressRepository
{
    Task<IEnumerable<Address>> GetAddressesAsync(Guid kycVerificationId);

    Task<bool> AddressExistsAsync(Guid kycVerificationId, string houseNumber,
        string street, string city, string state, string country);
}