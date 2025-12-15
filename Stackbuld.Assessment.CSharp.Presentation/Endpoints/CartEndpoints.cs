using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Extensions;
using Stackbuld.Assessment.CSharp.Application.Features.Cart.Commands;
using Stackbuld.Assessment.CSharp.Application.Features.Cart.Queries;
using Stackbuld.Assessment.CSharp.Presentation.Abstractions;

namespace Stackbuld.Assessment.CSharp.Presentation.Endpoints;

public class CartEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/cart").WithTags("Cart");

        group.MapGet("", GetCartByUserIdAsync)
            .WithName("GetCartByUserId")
            .WithSummary("Get cart")
            .WithDescription("Returns the signed-in user's cart.")
            .RequireAuthorization();

        group.MapPost("add/{productId:guid}", AddToCartAsync)
            .WithName("AddToCart")
            .WithSummary("Add a product to cart")
            .WithDescription(
                "Adds a product to the signed-in user's cart with the desired quantity.")
            .RequireAuthorization();

        group.MapDelete("remove/{cartItemId:guid}", RemoveCartItemAsync)
            .WithName("RemoveCartItem")
            .WithSummary("Remove item from cart")
            .WithDescription("Removes a specific item from the signed-in user's cart.")
            .RequireAuthorization();

        group.MapPost("checkout", CheckoutCartAsync)
            .WithName("Checkout")
            .WithSummary("Checkout cart")
            .WithDescription(
                "Creates an order from the signed-in user's cart and clears the cart.")
            .RequireAuthorization();
    }

    private static async Task<Results<Ok<ApiResponse>, BadRequest<ValidationProblemDetails>>> AddToCartAsync(
        Guid productId, Cart.AddToCartRequest request,
        ISender sender, CancellationToken cancellationToken = default)
    {
        var command = request.ToCommand(productId);
        string result = await sender.Send(command, cancellationToken);
        var apiResponse = ApiResponse.Success(message: result);

        return TypedResults.Ok(apiResponse);
    }

    private static async Task<Results<Ok<ApiResponse<Cart.GetCartByUserIdResponse>>, BadRequest<ProblemDetails>>>
        GetCartByUserIdAsync(
            ISender sender, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetCartByUserId.Query(), cancellationToken);
        var apiResponse = ApiResponse.Success(result.Value);

        return TypedResults.Ok(apiResponse);
    }

    private static async Task<Results<Ok<ApiResponse<Guid>>, BadRequest<ProblemDetails>>> RemoveCartItemAsync(
        Guid cartItemId,
        ISender sender, CancellationToken cancellationToken = default)
    {
        Guid result = await sender.Send(new RemoveCartItem.Command(cartItemId), cancellationToken);
        var apiResponse = ApiResponse.Success(result);

        return TypedResults.Ok(apiResponse);
    }

    private static async Task<Results<Ok<ApiResponse<Guid>>, BadRequest<ProblemDetails>>> CheckoutCartAsync(
        ISender sender, CancellationToken cancellationToken = default)
    {
        Guid result = await sender.Send(new CheckoutCart.Command(), cancellationToken);
        var apiResponse = ApiResponse.Success(result);

        return TypedResults.Ok(apiResponse);
    }
}