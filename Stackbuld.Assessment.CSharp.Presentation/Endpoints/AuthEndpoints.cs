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

        group.MapPost("confirm-email", ConfirmEmailAsync)
            .WithName("ConfirmEmail")
            .WithSummary("Confirm user email")
            .WithDescription("Confirms a user's email using the confirmation token.")
            .AllowAnonymous();

        group.MapPost("sign-in", SignInAsync)
            .WithName("SignIn")
            .WithSummary("Authenticate user")
            .WithDescription("Authenticates a user with their credentials and returns access and refresh tokens.")
            .AllowAnonymous();

        group.MapPost("refresh-token", RefreshTokenAsync)
            .WithName("RefreshToken")
            .WithSummary("Refresh access token")
            .WithDescription("Exchanges a valid refresh token for a new access token a new refresh token.")
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

    private static async Task<Results<Ok<ApiResponse>, BadRequest<ValidationProblemDetails>>> ConfirmEmailAsync(
        Auth.ConfirmEmailRequest request,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        string result = await sender.Send(command, cancellationToken);
        var apiResponse = ApiResponse.Success(message: result);

        return TypedResults.Ok(apiResponse);
    }

    private static async Task<Results<Ok<ApiResponse<Auth.SignInResponse>>, BadRequest<ValidationProblemDetails>>>
        SignInAsync(
            Auth.SignInRequest request,
            ISender sender, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        Auth.SignInResponse result = await sender.Send(command, cancellationToken);
        var apiResponse = ApiResponse.Success(result);

        return TypedResults.Ok(apiResponse);
    }

    private static async Task<Results<Ok<ApiResponse<Services.Jwt.GenerateTokenResponse>>,
            BadRequest<ValidationProblemDetails>>>
        RefreshTokenAsync(
            Auth.RefreshTokenRequest request,
            ISender sender, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await sender.Send(command, cancellationToken);
        var apiResponse = ApiResponse.Success(result.Value);

        return TypedResults.Ok(apiResponse);
    }
}