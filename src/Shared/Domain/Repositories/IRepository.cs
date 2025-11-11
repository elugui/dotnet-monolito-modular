using ModularMonolith.Shared.Domain.Entities;

namespace ModularMonolith.Shared.Domain.Repositories;

/// <summary>
/// Generic repository interface for aggregate roots
/// </summary>
public interface IRepository<TEntity, TId> where TEntity : AggregateRoot<TId> where TId : notnull
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
