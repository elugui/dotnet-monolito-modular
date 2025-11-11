using Microsoft.EntityFrameworkCore;
using ModularMonolith.Shared.Domain.Entities;
using ModularMonolith.Shared.Application.Interfaces;

namespace ModularMonolith.Shared.Infrastructure.Persistence;

/// <summary>
/// Base DbContext with common functionality for all modules
/// </summary>
public abstract class BaseDbContext : DbContext, IUnitOfWork
{
    protected BaseDbContext(DbContextOptions options) : base(options)
    {
    }

    public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.RollbackTransactionAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries<AggregateRoot<object>>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        // Domain events can be published here using MediatR
        // This requires IMediator injection which should be handled by derived classes
        await Task.CompletedTask;
    }
}
