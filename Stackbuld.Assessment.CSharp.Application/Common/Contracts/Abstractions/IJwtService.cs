using System.Security.Claims;

namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;

public interface IJwtService
{
    Task<Services.Jwt.GenerateTokenResponse> GenerateToken(Services.Jwt.GenerateTokenRequest request,
        CancellationToken cancellationToken = default);

    ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
}