using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Application.Common.Exceptions;
using Stackbuld.Assessment.CSharp.Domain.Entities;
using Stackbuld.Assessment.CSharp.Infrastructure.Configurations;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Services;

public class JwtService(
    IOptions<JwtSettings> jwt,
    IUnitOfWork uOw) : IJwtService
{
    private readonly JwtSettings _jwt = jwt.Value;

    public async Task<Application.Common.Contracts.Services.Jwt.GenerateTokenResponse> GenerateToken(
        Application.Common.Contracts.Services.Jwt.GenerateTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var (user, roles) = request;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName}  {user.LastName}"),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(),
                ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString("N")),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        if (!string.IsNullOrEmpty(user.SecurityStamp))
        {
            claims.Add(new Claim("sec_stamp", user.SecurityStamp));
        }

        var rsa = RSA.Create();
        rsa.ImportFromPem(_jwt.PrivateKey);
        var creds = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpireMinutes),
            signingCredentials: creds
        );

        var refreshToken = new RefreshToken
        {
            TokenHash = GenerateRefreshToken(),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwt.RefreshTokenExpireDays),
            UserId = user.Id,
            CreatedBy = $"{user.FirstName} {user.LastName}",
            UpdatedBy = $"{user.FirstName} {user.LastName}"
        };
        await uOw.RefreshTokensWriteRepository.AddAsync(refreshToken, cancellationToken);
        await uOw.SaveChangesAsync(cancellationToken);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new Application.Common.Contracts.Services.Jwt.GenerateTokenResponse(accessToken, _jwt.ExpireMinutes,
            refreshToken.TokenHash);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(_jwt.PublicKey);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwt.Audience,
            ValidateLifetime = false, // allow expired token for refresh flow
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
        };

        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw ApiException.Unauthorized(new Error("Auth.Error", "Invalid token"));

        return principal;
    }
}