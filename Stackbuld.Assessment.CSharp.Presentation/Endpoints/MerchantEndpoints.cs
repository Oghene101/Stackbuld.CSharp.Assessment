using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Filters;
using Stackbuld.Assessment.CSharp.Application.Extensions;
using Stackbuld.Assessment.CSharp.Domain.Constants;
using Stackbuld.Assessment.CSharp.Presentation.Abstractions;

namespace Stackbuld.Assessment.CSharp.Presentation.Endpoints;

public class MerchantEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/merchant").WithTags("Merchant");

        group.MapGet("products", GetProductsByMerchantIdAsync)
            .WithName("GetProductsByMerchant")
            .WithSummary("Get all products belonging to the signed-in merchant")
            .WithDescription(
                "Retrieves all products created by the merchant currently logged into the system.")
            .RequireAuthorization(Roles.Merchant);
    }

    private static async
        Task<Results<Ok<ApiResponse<PaginatorVm<IEnumerable<Product.GetProductsResponse>>>>,
            BadRequest<ValidationProblemDetails>>>
        GetProductsByMerchantIdAsync(
            [AsParameters] Product.GetProductsByMerchantIdRequest request,
            ISender sender, CancellationToken cancellationToken)
    {
        var query = request.ToQuery();
        var result = await sender.Send(query, cancellationToken);
        var apiResponse = ApiResponse.Success(result.Value);

        return TypedResults.Ok(apiResponse);
    }
}