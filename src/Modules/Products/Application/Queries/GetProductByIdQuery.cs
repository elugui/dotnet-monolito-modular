using ModularMonolith.Modules.Products.Application.DTOs;
using ModularMonolith.Shared.Application.Queries;

namespace ModularMonolith.Modules.Products.Application.Queries;

public record GetProductByIdQuery(Guid ProductId) : IQuery<ProductDto?>;
