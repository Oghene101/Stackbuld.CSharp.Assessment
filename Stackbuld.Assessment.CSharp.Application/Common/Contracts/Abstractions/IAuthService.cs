using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;

public interface IAuthService
{
    int MaxFailedAttempts { get; }
    int BaseLockoutMinutes { get; }
    int LockoutMultiplier { get; }
    int MaxLockoutMinutes { get; }

    string GetSignedInUserId();
    string GetSignedInUserEmail();
    string GetSignedInMerchantId();
    string GetSignedInUserName();
    Task SendEmailConfirmationAsync(User user, CancellationToken cancellationToken = default);
    Task SendForgotPasswordEmailAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> CheckPassword(Services.Authentication.CheckPasswordRequest request);
}