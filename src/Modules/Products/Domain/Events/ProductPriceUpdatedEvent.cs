using ModularMonolith.Shared.Domain.Events;

namespace ModularMonolith.Modules.Products.Domain.Events;

public record ProductPriceUpdatedEvent(Guid ProductId, decimal NewPrice, string Currency) : DomainEventBase;
