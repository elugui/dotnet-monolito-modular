using ModularMonolith.Shared.Domain.Events;

namespace ModularMonolith.Modules.Customers.Domain.Events;

public record CustomerDeactivatedEvent(Guid CustomerId) : DomainEventBase;
