namespace ModularMonolith.Modules.Customers.Application.DTOs;

public record CustomerDto(
    Guid Id,
    string Name,
    string Email,
    string? PhoneNumber,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsActive
);
