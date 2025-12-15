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
    AppDbContext context) : IUnitOfWork, IAsyncDisposable
{
    private IDbContextTransaction? _transaction;
    public IDbConnection DbConnection => context.Database.GetDbConnection();
    public IDbTransaction? DbTransaction => _transaction?.GetDbTransaction();

    # region Repositories

    #region RefreshTokens

    public IRefreshTokenRepository RefreshTokensReadRepository =>
        new RefreshTokenRepository(DbConnection, DbTransaction);

    public IRepository<RefreshToken> RefreshTokensWriteRepository => new Repository<RefreshToken>(context);

    #endregion

    #region KycVerifications

    public IKycVerificationRepository KycVerificationsReadRepository =>
        new KycVerificationRepository(DbConnection, DbTransaction);

    public IRepository<KycVerification> KycVerificationsWriteRepository => new Repository<KycVerification>(context);

    #endregion

    #region Addresses

    public IAddressRepository AddressesReadRepository => new AddressRepository(DbConnection, DbTransaction);
    public IRepository<Address> AddressesWriteRepository => new Repository<Address>(context);

    #endregion

    #region Products

    public IProductRepository ProductsReadRepository => new ProductRepository(DbConnection, DbTransaction);
    public IProductWriteRepository ProductsWriteRepository => new ProductWriteRepository(context);

    #endregion

    #region Merchants

    public IMerchantRepository MerchantsReadRepository => new MerchantRepository(DbConnection, DbTransaction);
    public IRepository<Merchant> MerchantsWriteRepository => new Repository<Merchant>(context);

    #endregion

    #region Carts

    public ICartRepository CartsReadRepository => new CartRepository(DbConnection, DbTransaction);
    public IRepository<Cart> CartsWriteRepository => new Repository<Cart>(context);

    #endregion

    #region CartIems

    public IRepository<CartItem> CartItemsWriteRepository => new Repository<CartItem>(context);

    #endregion

    #region Order

    public IRepository<Order> OrdersWriteRepository => new Repository<Order>(context);

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