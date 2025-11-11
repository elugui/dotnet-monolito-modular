using ModularMonolith.Shared.Domain.Events;

namespace ModularMonolith.Modules.Products.Domain.Events;

public record ProductCreatedEvent(Guid ProductId, string Name, decimal Price) : DomainEventBase;
