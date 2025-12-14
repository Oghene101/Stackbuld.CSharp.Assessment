using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Mailing;

public interface IEmailTemplates
{
    string EmailConfirmation(User user, string confirmationLink);

}