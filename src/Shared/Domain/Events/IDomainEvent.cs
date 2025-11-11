using MediatR;

namespace ModularMonolith.Shared.Domain.Events;

/// <summary>
/// Marker interface for domain events that can be handled by MediatR
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
