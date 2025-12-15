using System.Data;
using Dapper;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Repositories;

public class CartRepository(
    IDbConnection connection,
    IDbTransaction? transaction) : ICartRepository
{
    public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
    {
        var sql = """
                  SELECT * FROM "Carts" 
                           WHERE "UserId" = @userId;
                  """;

        var result = await connection.QueryFirstOrDefaultAsync<Cart>(sql, new { userId }, transaction);
        return result;
    }

    public async Task<Cart?> GetCartWithCartItemsByUserIdAsync(Guid userId)
    {
        var sql = """
                   SELECT * FROM "Carts" AS C
                            INNER JOIN "CartItems" AS CI ON CI."CartId" = C."Id"
                   WHERE C."UserId" = @userId;
                  """;

        var cartDictionary = new Dictionary<Guid, Cart>();

        await connection.QueryAsync<Cart, CartItem, Cart>(
            sql,
            (cart, cartItem) =>
            {
                if (!cartDictionary.TryGetValue(cart.Id, out var existingCart))
                {
                    existingCart = cart;
                    cartDictionary.Add(existingCart.Id, existingCart);
                }

                existingCart.CartItems.Add(cartItem);

                return existingCart;
            },
            new { userId },
            transaction,
            splitOn: "Id"
        );

        return cartDictionary.Values.SingleOrDefault();
    }
}