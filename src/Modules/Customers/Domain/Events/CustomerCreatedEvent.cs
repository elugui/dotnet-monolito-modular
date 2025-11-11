using ModularMonolith.Shared.Domain.Events;

namespace ModularMonolith.Modules.Customers.Domain.Events;

public record CustomerCreatedEvent(Guid CustomerId, string Name, string Email) : DomainEventBase;
