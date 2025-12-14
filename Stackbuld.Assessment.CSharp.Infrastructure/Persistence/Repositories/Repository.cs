using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;
using Stackbuld.Assessment.CSharp.Domain.Entities;
using Stackbuld.Assessment.CSharp.Infrastructure.Persistence.DbContexts;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Repositories;

public class Repository<TEntity>(
    AppDbContext context) : IRepository<TEntity> where TEntity : EntityBase
{
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public async Task<TEntity?> FindAsync(object id, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync([id], cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);

    public void Update(TEntity entity, params Expression<Func<TEntity, object>>[] updatedProperties)
    {
        foreach (var property in updatedProperties)
        {
            context.Entry(entity).Property(property).IsModified = true;
        }
    }

    public void Remove(TEntity entity)
        => DbSet.Remove(entity);
    
    public void RemoveRange(IEnumerable<TEntity> entities)
        => DbSet.RemoveRange(entities);
}