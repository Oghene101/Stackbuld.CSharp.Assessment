using MediatR;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Application.Common.Exceptions;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Features.Cart.Commands;

public static class CheckoutCart
{
    public record Command() : IRequest<Result<Guid>>;

    public class Handler(
        IAuthService auth,
        IUnitOfWork uOw) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            await uOw.BeginTransactionAsync(cancellationToken);
            try
            {
                var userId = Guid.Parse(auth.GetSignedInUserId());
                var cart = await uOw.CartsReadRepository.GetCartWithCartItemsByUserIdAsync(userId);
                if (cart is null) throw ApiException.NotFound(new Error("Cart.Error", "Cart not found"));

                var productIds = cart.CartItems.Select(x => x.ProductId).ToArray();
                var products = (await uOw.ProductsReadRepository.GetProductsByIdsAsync(productIds))
                    .ToDictionary(x => x.Id);

                foreach (var item in cart.CartItems)
                {
                    if (!products.TryGetValue(item.ProductId, out var product))
                        throw ApiException.BadRequest(
                            new Error("Cart.Error", "Product no longer exists"));

                    if (product.StockQuantity < item.Quantity)
                        throw ApiException.BadRequest(
                            new Error("Cart.Error", $"Insufficient stock for {product.Name}"));
                }

                var totalAmount = cart.CartItems.Sum(x =>
                {
                    var product = products[x.ProductId];
                    return product.Price * x.Quantity;
                });

                var order = new Order
                {
                    UserId = userId,
                    TotalAmount = totalAmount
                };

                foreach (var item in cart.CartItems)
                {
                    var product = products[item.ProductId];

                    order.OrderItems.Add(new OrderItem
                    {
                        UnitPrice = product.Price,
                        Quantity = item.Quantity,
                        OrderId = order.Id,
                        ProductId = product.Id
                    });

                    var sufficient =
                        await uOw.ProductsWriteRepository.TryReduceStockAsync(product.Id, item.Quantity,
                            cancellationToken);
                    if (!sufficient)
                        throw ApiException.BadRequest(new Error("Cart.Error",
                            $"Insufficient stock for {product.Name}"));
                }

                await uOw.OrdersWriteRepository.AddAsync(order, cancellationToken);
                uOw.CartItemsWriteRepository.RemoveRange(cart.CartItems);
                await uOw.CommitTransactionAsync(cancellationToken);

                return Result.Success(order.Id);
            }
            catch (Exception)
            {
                await uOw.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}