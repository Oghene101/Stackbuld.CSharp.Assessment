using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Exceptions;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Features.Auth.Commands;

public static class ForgotPassword
{
    public record Command(string Email) : IRequest<Result<string>>;

    public class Handler(
        UserManager<User> userManager,
        IAuthService auth) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                throw ApiException.NotFound(new Error("Auth.Error", $"User with email '{request.Email}' not found"));

            await auth.SendForgotPasswordEmailAsync(user, cancellationToken);
            return Result.Success("A link has been sent to your email address to reset your password.");
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be a valid email address");
        }
    }
}