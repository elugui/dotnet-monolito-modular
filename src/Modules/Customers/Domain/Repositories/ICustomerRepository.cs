using ModularMonolith.Modules.Customers.Domain.Entities;
using ModularMonolith.Shared.Domain.Repositories;

namespace ModularMonolith.Modules.Customers.Domain.Repositories;

/// <summary>
/// Repository interface for Customer aggregate
/// </summary>
public interface ICustomerRepository : IRepository<Customer, Guid>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);
}
