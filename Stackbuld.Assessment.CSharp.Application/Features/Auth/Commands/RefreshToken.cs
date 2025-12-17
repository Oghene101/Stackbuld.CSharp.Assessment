using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Application.Common.Exceptions;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Features.Auth.Commands;

public static class RefreshToken
{
    public record Command(string AccessToken, string RefreshToken)
        : IRequest<Result<Services.Jwt.GenerateTokenResponse>>;

    public class Handler(
        IJwtService jwt,
        UserManager<User> userManager,
        IUnitOfWork uOw) : IRequestHandler<Command, Result<Services.Jwt.GenerateTokenResponse>>
    {
        public async Task<Result<Services.Jwt.GenerateTokenResponse>> Handle(Command request,
            CancellationToken cancellationToken)
        {
            ClaimsPrincipal principal;
            ClaimsIdentity identity;
            try
            {
                principal = jwt.GetPrincipalFromExpiredToken(request.AccessToken);
            }
            catch (Exception)
            {
                throw ApiException.Unauthorized(new Error("Auth.Error", "Invalid token"));
            }

            var email = principal.FindFirstValue(ClaimTypes.Email);

            var user = await userManager.FindByEmailAsync(email!);
            if (user is null) throw ApiException.Unauthorized(new Error("Auth.Error", "Invalid token"));

            var refreshToken = await uOw.RefreshTokensReadRepository.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshToken is null || refreshToken.UserId != user.Id)
                throw ApiException.Unauthorized(new Error("Auth.Error", "Invalid token"));

            var roles = await userManager.GetRolesAsync(user);
            var token = await jwt.GenerateToken(new Services.Jwt.GenerateTokenRequest(user, roles), cancellationToken);

            refreshToken.IsUsed = true;
            refreshToken.UpdatedAt = DateTimeOffset.UtcNow;
            uOw.RefreshTokensWriteRepository.Update(refreshToken,
                x => x.IsUsed,
                x => x.UpdatedAt,
                x => x.UpdatedBy);

            await uOw.SaveChangesAsync(cancellationToken);

            return Result.Success(token);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage("Access token is required");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token required")
                .MaximumLength(44).WithMessage("Refresh token is invalid");
        }
    }
}