using MediatR;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

namespace Stackbuld.Assessment.CSharp.Application.Features.Cart.Queries;

public static class GetCartByUserId
{
    public record Query() : IRequest<Result<object>>;

    public class Handler(
        IAuthService auth,
        IUnitOfWork uOw) : IRequestHandler<Query, Result<object>>
    {
        public async Task<Result<object>> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(auth.GetSignedInUserId());
            var cart = await uOw.CartsReadRepository.GetCartWithCartItemsByUserIdAsync(userId);
            
            throw new NotImplementedException();
        }
    }
}