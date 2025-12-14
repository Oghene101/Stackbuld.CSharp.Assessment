using System.Data;
using LoanApplication.Domain.Entities;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

public interface IUnitOfWork
{
    IDbConnection DbConnection { get; }
    IDbTransaction? DbTransaction { get; }

    public IRefreshTokenRepository RefreshTokensReadRepository { get; }
    public IRepository<RefreshToken> RefreshTokensWriteRepository { get; }
    public IKycVerificationRepository KycVerificationsReadRepository { get; }
    public IRepository<KycVerification> KycVerificationsWriteRepository { get; }
    public IAddressRepository AddressesReadRepository { get; }
    public IRepository<Address> AddressesWriteRepository { get; }
    public IProductRepository ProductsReadRepository { get; }
    public IProductWriteRepository ProductsWriteRepository { get; }
    public IMerchantRepository MerchantsReadRepository { get; }
    public IRepository<Merchant> MerchantsWriteRepository { get; }
    public ICartRepository CartsReadRepository { get; }
    public IRepository<Domain.Entities.Cart> CartsWriteRepository { get; }
    public IRepository<CartItem> CartItemsWriteRepository { get; }
    public IRepository<Order> OrdersWriteRepository { get; }
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}