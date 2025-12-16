using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Extensions;
using Stackbuld.Assessment.CSharp.Presentation.Abstractions;

namespace Stackbuld.Assessment.CSharp.Presentation.Endpoints;

public class AuthEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth").WithTags("Auth");

        group.MapPost("sign-up", SignUpAsync)
            .WithName("SignUp")
            .WithSummary("Register a new user")
            .WithDescription("Creates a user account with credentials for authentication.")
            .AllowAnonymous();

        group.MapPost("merchant/sign-up", MerchantSignUpAsync)
            .WithName("MerchantSignUp")
            .WithSummary("Register a new merchant")
            .WithDescription("Creates a merchant account with credentials for authentication.")
            .AllowAnonymous();
    }


    private static async Task<Results<Ok<ApiResponse<Guid>>, BadRequest<ValidationProblemDetails>>> SignUpAsync(
        Auth.SignUpRequest request,
        ISender sender, CancellationToken cancellationToken = default)
    {
        var command = request.ToCommand();
        Guid result = await sender.Send(command, cancellationToken);
        var apiResponse = ApiResponse.Success(result);

        return TypedResults.Ok(apiResponse);
    }

    private static async Task<Results<Ok<ApiResponse<Guid>>, BadRequest<ValidationProblemDetails>>> MerchantSignUpAsync(
        Auth.SignUpMerchantRequest request,
        ISender sender, CancellationToken cancellationToken = default)
    {
        var command = request.ToCommand();
        Guid result = await sender.Send(command, cancellationToken);
        var apiResponse = ApiResponse.Success(result);

        return TypedResults.Ok(apiResponse);
    }
}