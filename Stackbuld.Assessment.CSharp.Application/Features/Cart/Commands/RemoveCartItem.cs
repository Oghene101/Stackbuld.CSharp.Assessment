using MediatR;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

namespace Stackbuld.Assessment.CSharp.Application.Features.Cart.Commands;

public static class RemoveCartItem
{
    public record Command(
        Guid Id) : IRequest<Result<Guid>>;

    public class Handler(
        IUnitOfWork uOw) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            await uOw.CartItemsWriteRepository.RemoveAsync(request.Id, cancellationToken);
            await uOw.SaveChangesAsync(cancellationToken);

            return Result.Success(request.Id);
        }
    }
}