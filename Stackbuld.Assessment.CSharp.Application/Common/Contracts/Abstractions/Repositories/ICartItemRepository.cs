namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

public interface ICartItemRepository
{
    void DeleteCartItem(Guid id);
}