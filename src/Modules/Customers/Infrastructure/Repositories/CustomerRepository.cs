using Microsoft.EntityFrameworkCore;
using ModularMonolith.Modules.Customers.Domain.Entities;
using ModularMonolith.Modules.Customers.Domain.Repositories;
using ModularMonolith.Modules.Customers.Infrastructure.Persistence;
using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Customers.Infrastructure.Repositories;

public class CustomerRepository : BaseRepository<Customer, Guid>, ICustomerRepository
{
    private readonly CustomersDbContext _context;

    public CustomerRepository(CustomersDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email.Value == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(c => c.IsActive)
            .ToListAsync(cancellationToken);
    }
}
