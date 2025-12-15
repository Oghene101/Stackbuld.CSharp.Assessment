using MediatR;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Application.Common.Exceptions;
using Stackbuld.Assessment.CSharp.Application.Extensions;
using GetProductByIdResponse = Stackbuld.Assessment.CSharp.Application.Common.Contracts.Product.GetProductByIdResponse;

namespace Stackbuld.Assessment.CSharp.Application.Features.Product.Queries;

public static class GetProductById
{
    public record Query(Guid Id) : IRequest<Result<GetProductByIdResponse>>;

    public class Handler(
        IUnitOfWork uOw) : IRequestHandler<Query, Result<GetProductByIdResponse>>
    {
        public async Task<Result<GetProductByIdResponse>> Handle(Query request,
            CancellationToken cancellationToken)
        {
            var product = await uOw.ProductsReadRepository.GetProductByIdAsync(request.Id);
            if (product is null)
                throw ApiException.NotFound(new Error("Product.Error", $"Product with id '{request.Id}' not found"));

            var merchantName = await uOw.MerchantsReadRepository.GetMerchantNameById(product.MerchantId);

            var getProductByIdResponse = product.ToVm(merchantName!);
            return Result.Success(getProductByIdResponse);
        }
    }
}