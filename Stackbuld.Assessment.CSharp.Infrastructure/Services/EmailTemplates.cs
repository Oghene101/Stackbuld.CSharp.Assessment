using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Mailing;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Services;

public class EmailTemplates : IEmailTemplates
{
    public string EmailConfirmation(User user, string confirmationLink)
    {
        return $"""
                <p>Hello {user.FirstName},</p>
                <p>Please confirm your email by clicking the link below:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>This link will expire shortly for your security.</p>
                """;
    }
}