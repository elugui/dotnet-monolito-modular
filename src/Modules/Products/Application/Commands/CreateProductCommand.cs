using ModularMonolith.Modules.Products.Application.DTOs;
using ModularMonolith.Shared.Application.Commands;

namespace ModularMonolith.Modules.Products.Application.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string Currency = "USD"
) : ICommand<ProductDto>;
