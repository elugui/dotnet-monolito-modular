namespace ModularMonolith.Modules.Products.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsActive
);
