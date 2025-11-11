namespace ModularMonolith.Shared.Domain.Events;

/// <summary>
/// Base implementation for domain events
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
