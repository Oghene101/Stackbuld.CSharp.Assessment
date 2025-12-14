namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

public interface ICartRepository
{
    Task<Domain.Entities.Cart?> GetCartByUserIdAsync(Guid userId);
    Task<Domain.Entities.Cart?> GetCartWithCartItemsByUserIdAsync(Guid userId);
}