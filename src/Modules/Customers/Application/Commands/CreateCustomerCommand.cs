using ModularMonolith.Modules.Customers.Application.DTOs;
using ModularMonolith.Shared.Application.Commands;

namespace ModularMonolith.Modules.Customers.Application.Commands;

public record CreateCustomerCommand(
    string Name,
    string Email,
    string? PhoneNumber = null
) : ICommand<CustomerDto>;
