using FluentValidation;
using MediatR;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Application.Common.Exceptions;
using Stackbuld.Assessment.CSharp.Application.Extensions;

namespace Stackbuld.Assessment.CSharp.Application.Features.Cart.Commands;

public static class AddToCart
{
    public record Command(
        Guid ProductId,
        int Quantity) : IRequest<Result<string>>;

    public class Handler(
        IAuthService auth,
        IUnitOfWork uOw) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(auth.GetSignedInUserId());
            var userEmail = auth.GetSignedInUserEmail();

            var product = await uOw.ProductsReadRepository.GetProductByIdAsync(request.ProductId);
            if (product is null) throw ApiException.NotFound(new Error("Cart.Error", "Product not found"));

            var cart = (await uOw.CartsReadRepository.GetCartByUserIdAsync(userId)) ?? new Domain.Entities.Cart
            {
                UserId = userId,
                CreatedBy = userEmail,
                UpdatedBy = userEmail
            };

            var cartItem = product.ToCartItem(request.Quantity, cart.Id);
            cart.CartItems.Add(cartItem);

            await uOw.CartsWriteRepository.AddAsync(cart, cancellationToken);
            await uOw.SaveChangesAsync(cancellationToken);

            return Result.Success($"{product.Name} added to cart");
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero");
        }
    }
}