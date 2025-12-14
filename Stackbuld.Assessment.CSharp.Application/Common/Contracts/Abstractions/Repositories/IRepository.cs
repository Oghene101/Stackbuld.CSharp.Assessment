using System.Linq.Expressions;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions.Repositories;

public interface IRepository<TEntity> where TEntity : EntityBase
{
    Task<TEntity?> FindAsync(object id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity, params Expression<Func<TEntity, object>>[] updatedProperties);
    void Remove(TEntity entity);

    void RemoveRange(IEnumerable<TEntity> entities);
}