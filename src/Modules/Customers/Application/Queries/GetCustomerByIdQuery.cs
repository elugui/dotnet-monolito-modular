using ModularMonolith.Modules.Customers.Application.DTOs;
using ModularMonolith.Shared.Application.Queries;

namespace ModularMonolith.Modules.Customers.Application.Queries;

public record GetCustomerByIdQuery(Guid CustomerId) : IQuery<CustomerDto?>;
