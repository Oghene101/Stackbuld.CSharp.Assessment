using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts;

public static class Services
{
    public static class Jwt
    {
        public record GenerateTokenRequest(User User, IList<string> Roles);

        public record GenerateTokenResponse(string AccessToken, int ExpireMinutes, string RefreshToken);
    }

    public static class Authentication
    {
        public record CheckPasswordRequest(
            User User,
            string Password);
    }
}