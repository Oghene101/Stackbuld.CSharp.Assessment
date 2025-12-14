using FluentValidation;
using MediatR;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Application.Extensions;

namespace Stackbuld.Assessment.CSharp.Application.Features.Product.Commands;

public static class CreateProduct
{
    public record Command(
        string Name,
        string Description,
        decimal Price,
        int StockQuantity) : IRequest<Result<Guid>>;

    public class Handler(
        IAuthService auth,
        IUnitOfWork uOw) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var merchantId = Guid.Parse(auth.GetSignedInUserId());
            var product = request.ToEntity(merchantId);

            await uOw.ProductsWriteRepository.AddAsync(product, cancellationToken);
            await uOw.SaveChangesAsync(cancellationToken);

            return Result.Success(product.Id);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(500).WithMessage("Name must not exceed 500 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(1000).WithMessage("Description must not exceed 500 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero");

            RuleFor(x => x.StockQuantity)
                .GreaterThan(0).WithMessage("Stock quantity must be greater than zero");
        }
    }
}