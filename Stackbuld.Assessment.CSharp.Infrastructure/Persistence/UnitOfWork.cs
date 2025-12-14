using System.Data;
using LoanApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Domain.Entities;
using Stackbuld.Assessment.CSharp.Infrastructure.Persistence.DbContexts;
using Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Repositories;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence;

public class UnitOfWork(
    AppDbContext context,
    IRefreshTokenRepository refreshTokensReadRepository,
    IRepository<RefreshToken> refreshTokensWriteRepository,
    IKycVerificationRepository kycVerificationsReadRepository,
    IRepository<KycVerification> kycVerificationsWriteRepository,
    IAddressRepository addressesReadRepository,
    IRepository<Address> addressesWriteRepository,
    IProductRepository productsReadRepository,
    IProductWriteRepository productsWriteRepository,
    IMerchantRepository merchantsReadRepository,
    IRepository<Merchant> merchantsWriteRepository,
    ICartRepository cartsReadRepository,
    IRepository<Cart> cartsWriteRepository,
    IRepository<CartItem> cartItemsWriteRepository,
    IRepository<Order> ordersWriteRepository
    ) : IUnitOfWork, IAsyncDisposable
{
    private IDbContextTransaction? _transaction;
    public IDbConnection DbConnection => context.Database.GetDbConnection();
    public IDbTransaction? DbTransaction => _transaction?.GetDbTransaction();

    # region Repositories

    #region RefreshTokens

    public IRefreshTokenRepository RefreshTokensReadRepository => refreshTokensReadRepository;
    public IRepository<RefreshToken> RefreshTokensWriteRepository => refreshTokensWriteRepository;

    #endregion

    #region KycVerifications

    public IKycVerificationRepository KycVerificationsReadRepository => kycVerificationsReadRepository;
    public IRepository<KycVerification> KycVerificationsWriteRepository => kycVerificationsWriteRepository;

    #endregion

    #region Addresses

    public IAddressRepository AddressesReadRepository => addressesReadRepository;
    public IRepository<Address> AddressesWriteRepository => addressesWriteRepository;

    #endregion

    #region Products

    public IProductRepository ProductsReadRepository => productsReadRepository;
    public IProductWriteRepository ProductsWriteRepository => productsWriteRepository;

    #endregion

    #region Merchants

    public IMerchantRepository MerchantsReadRepository => merchantsReadRepository;
    public IRepository<Merchant> MerchantsWriteRepository => merchantsWriteRepository;

    #endregion

    #region Carts

    public ICartRepository CartsReadRepository => cartsReadRepository;
    public IRepository<Cart> CartsWriteRepository => cartsWriteRepository;

    #endregion
    
    #region CartIems

    public IRepository<CartItem> CartItemsWriteRepository => cartItemsWriteRepository;

    #endregion
    
    #region Order

    public IRepository<Order> OrdersWriteRepository => ordersWriteRepository;

    #endregion

    # endregion

    # region Transaction support (EF + Dapper can share the same transaction if needed)

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null) throw new InvalidOperationException("There is already an active transaction.");

        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) throw new InvalidOperationException("There is no active transaction.");
        await SaveChangesAsync(cancellationToken);
        await _transaction.CommitAsync(cancellationToken);
        await DisposeTransactionAsync();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) throw new InvalidOperationException("There is no active transaction.");
        await _transaction.RollbackAsync(cancellationToken);
        await DisposeTransactionAsync();
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);


    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null) await _transaction.DisposeAsync();
    }

    #endregion
}