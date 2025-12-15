using MediatR;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Application.Common.Exceptions;
using Stackbuld.Assessment.CSharp.Application.Extensions;
using GetCartByUserIdResponse = Stackbuld.Assessment.CSharp.Application.Common.Contracts.Cart.GetCartByUserIdResponse;

namespace Stackbuld.Assessment.CSharp.Application.Features.Cart.Queries;

public static class GetCartByUserId
{
    public record Query() : IRequest<Result<GetCartByUserIdResponse>>;

    public class Handler(
        IAuthService auth,
        IUnitOfWork uOw) : IRequestHandler<Query, Result<GetCartByUserIdResponse>>
    {
        public async Task<Result<GetCartByUserIdResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(auth.GetSignedInUserId());

            var cart = await uOw.CartsReadRepository.GetCartWithCartItemsByUserIdAsync(userId);
            if (cart is null) throw ApiException.NotFound(new Error("Cart.Error", "Cart not found"));

            var productIds = cart.CartItems.Select(x => x.ProductId);
            var products = (await uOw.ProductsReadRepository.GetProductsByIdsAsync(productIds)).ToDictionary(x => x.Id);

            var getCartByUserIdResponse = cart.ToVm(products);
            return Result.Success(getCartByUserIdResponse);
        }
    }
}