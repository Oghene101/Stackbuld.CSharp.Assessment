using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
}