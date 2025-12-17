using MediatR;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Application.Common.Filters;
using Stackbuld.Assessment.CSharp.Application.Extensions;
using GetProductsResponse = Stackbuld.Assessment.CSharp.Application.Common.Contracts.Product.GetProductsResponse;

namespace Stackbuld.Assessment.CSharp.Application.Features.Merchant.Queries;

public static class GetProductsByMerchantId
{
    public record Query(
        PaginationFilter PaginationFilter,
        DateRangeFilter DateRangeFilter) : IRequest<Result<PaginatorVm<IEnumerable<GetProductsResponse>>>>;

    public class Handler(
        IAuthService auth,
        IUnitOfWork uOw) : IRequestHandler<Query, Result<PaginatorVm<IEnumerable<GetProductsResponse>>>>
    {
        public async Task<Result<PaginatorVm<IEnumerable<GetProductsResponse>>>> Handle(Query request,
            CancellationToken cancellationToken)
        {
            var merchantId = Guid.Parse(auth.GetSignedInMerchantId());
            var pageNumber = request.PaginationFilter.PageNumber;
            var pageSize = request.PaginationFilter.PageSize;
            var startDate = request.DateRangeFilter.EffectiveStartDate;
            var endDate = request.DateRangeFilter.EffectiveEndDate;

            var (products, totalCount) =
                await uOw.ProductsReadRepository.GetProductsByMerchantIdAsync(merchantId, pageNumber, pageSize,
                    startDate, endDate);

            return products.ToVm(totalCount, pageNumber, pageSize);
        }
    }
}