using ModularMonolith.Modules.Customers.Application.DTOs;
using ModularMonolith.Shared.Application.Queries;

namespace ModularMonolith.Modules.Customers.Application.Queries;

public record GetAllCustomersQuery : IQuery<IEnumerable<CustomerDto>>;
