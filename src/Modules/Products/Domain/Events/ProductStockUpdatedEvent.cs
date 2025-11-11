using ModularMonolith.Shared.Domain.Events;

namespace ModularMonolith.Modules.Products.Domain.Events;

public record ProductStockUpdatedEvent(Guid ProductId, int NewStockQuantity) : DomainEventBase;
