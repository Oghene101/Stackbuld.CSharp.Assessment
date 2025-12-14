using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Filters;
using Stackbuld.Assessment.CSharp.Application.Extensions;
using Stackbuld.Assessment.CSharp.Application.Features.Product.Commands;
using Stackbuld.Assessment.CSharp.Application.Features.Product.Queries;
using Stackbuld.Assessment.CSharp.Domain.Constants;
using Stackbuld.Assessment.CSharp.Presentation.Abstractions;

namespace Stackbuld.Assessment.CSharp.Presentation.Endpoints;

public class ProductEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/product").WithTags("Product");

        group.MapPost("create", CreateAsync)
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .WithDescription(
                "Allows a merchant to create a new product with name, description, price, and stock quantity.")
            .RequireAuthorization(Roles.Merchant);

        group.MapGet("{id:guid}", GetByIdAsync)
            .WithName("GetProductById")
            .WithSummary("Get a product by ID")
            .WithDescription("Retrieves a product’s details using its unique identifier.")
            .AllowAnonymous();

        group.MapGet("", GetAsync)
            .WithName("GetProducts")
            .WithSummary("Get all products")
            .WithDescription("Retrieves a paginated list of products.")
            .AllowAnonymous();

        group.MapPut("{id:guid}", UpdateAsync)
            .WithName("UpdateProduct")
            .WithSummary("Update a product")
            .WithDescription("Updates an existing product using its unique identifier.")
            .RequireAuthorization(Roles.Merchant);

        group.MapDelete("{id:guid}", DeleteAsync)
            .WithName("DeleteProduct")
            .WithSummary("Delete a product")
            .WithDescription("Deletes an existing product using its unique identifier.")
            .RequireAuthorization(Roles.Merchant);
    }

    private static async Task<Results<Created<ApiResponse<Guid>>, BadRequest<ValidationProblemDetails>>> CreateAsync(
        Product.CreateProductRequest request,
        ISender sender, CancellationToken cancellationToken = default)
    {
        var command = request.ToCommand();
        Guid value = await sender.Send(command, cancellationToken);
        var apiResponse = ApiResponse.Success(value);

        return TypedResults.Created("", apiResponse);
    }

    private static async
        Task<Results<Ok<ApiResponse<Product.GetProductByIdResponse>>, BadRequest<ValidationProblemDetails>>>
        GetByIdAsync(
            Guid id,
            ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductById.Query(id), cancellationToken);
        var apiResponse = ApiResponse.Success(result.Value);

        return TypedResults.Ok(apiResponse);
    }

    private static async
        Task<Results<Ok<ApiResponse<PaginatorVm<IEnumerable<Product.GetProductsResponse>>>>,
            BadRequest<ValidationProblemDetails>>>
        GetAsync(
            [AsParameters] Product.GetProductsRequest request,
            ISender sender, CancellationToken cancellationToken)
    {
        var query = request.ToQuery();
        var result = await sender.Send(query, cancellationToken);
        var apiResponse = ApiResponse.Success(result.Value);

        return TypedResults.Ok(apiResponse);
    }

    private static async Task<Results<Ok<ApiResponse<Guid>>, BadRequest<ValidationProblemDetails>>> UpdateAsync(
        Guid id, Product.UpdateProductRequest request,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);
        Guid value = await sender.Send(command, cancellationToken);
        var apiResponse = ApiResponse.Success(value);

        return TypedResults.Ok(apiResponse);
    }

    private static async Task<Results<Ok<ApiResponse<Guid>>, BadRequest<ValidationProblemDetails>>> DeleteAsync(
        Guid id,
        ISender sender, CancellationToken cancellationToken)
    {
        Guid value = await sender.Send(new DeleteProduct.Command(id), cancellationToken);
        var apiResponse = ApiResponse.Success(value);

        return TypedResults.Ok(apiResponse);
    }
}