using MediatR;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Application.Common.Exceptions;

namespace Stackbuld.Assessment.CSharp.Application.Features.Product.Commands;

public static class DeleteProduct
{
    public record Command(Guid Id) : IRequest<Result<Guid>>;

    public class Handler(
        IAuthService auth,
        IUnitOfWork uOw) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var merchantEmail = auth.GetSignedInUserEmail();
            var product = await uOw.ProductsReadRepository.GetProductByIdAsync(request.Id);
            if (product is null) throw ApiException.NotFound(new Error("Product.Error", "Product not found"));

            product.IsDeleted = true;
            product.DeletedAt = DateTimeOffset.UtcNow;
            product.DeletedBy = merchantEmail;
            uOw.ProductsWriteRepository.Update(product, x => x.IsDeleted);
            await uOw.SaveChangesAsync(cancellationToken);


            return Result.Success(request.Id);
        }
    }
}