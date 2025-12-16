using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Application.Common.Exceptions;
using Stackbuld.Assessment.CSharp.Application.Extensions;
using Stackbuld.Assessment.CSharp.Domain.Constants;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Features.Auth.Commands;

public class SignUpMerchant
{
    public record Command(
        string FirstName,
        string LastName,
        string Email,
        string BusinessName,
        string Password) : IRequest<Result<Guid>>;

    public class Handler(
        UserManager<User> userManager,
        IBackgroundTaskQueue queue,
        IUnitOfWork uOw,
        ILogger<Handler> logger) : IRequestHandler<Command, Result<Guid>>
    {
        private static readonly string Separator = new('*', 110);

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            await uOw.BeginTransactionAsync(cancellationToken);
            var user = request.ToEntity();

            try
            {
                var result = await userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                    throw ApiException.BadRequest(
                        result.Errors.Select(e => new Error(e.Code, e.Description))
                            .ToArray());

                result = await userManager.AddToRoleAsync(user, Roles.Merchant);
                if (!result.Succeeded)
                {
                    throw ApiException.BadRequest(
                        result.Errors.Select(e => new Error(e.Code, e.Description))
                            .ToArray());
                }

                var merchant = new Domain.Entities.Merchant
                {
                    BusinessName = request.BusinessName,
                    UserId = user.Id,
                    CreatedBy = $"{user.FirstName} {user.LastName}",
                    UpdatedBy = $"{user.FirstName} {user.LastName}"
                };
                await uOw.MerchantsWriteRepository.AddAsync(merchant, cancellationToken);

                await uOw.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception)
            {
                await uOw.RollbackTransactionAsync(cancellationToken);
                throw;
            }

            await SendEmailAsync(user, cancellationToken);

            return Result.Success(user.Id);
        }

        private async Task SendEmailAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                await queue.QueueBackgroundWorkItemAsync(async (sp, ct) =>
                {
                    try
                    {
                        var auth = sp.GetRequiredService<IAuthService>();
                        await auth.SendEmailConfirmationAsync(user, ct);
                    }
                    catch (Exception ex)
                    {
                        // log and swallow, so it doesn't crash the worker
                        logger.LogError("""
                                        {Separator}
                                        Error occured while sending email confirmation

                                        Exception Message: {Message}

                                        Exception Type: {ExceptionType}
                                        {Separator}

                                        Stack Trace: {StackTrace}

                                        """, Separator, ex.Message,
                            ex.GetType().FullName ?? ex.GetType().Name, ex.StackTrace, Separator);
                    }
                }, cancellationToken);
            }
            catch (InvalidOperationException exception)
            {
                logger.LogError("""
                                {Separator} 

                                Exception Message: {Message}

                                {Separator}
                                """, Separator, exception.Message, Separator);
            }
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be a valid email address");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
        }
    }
}