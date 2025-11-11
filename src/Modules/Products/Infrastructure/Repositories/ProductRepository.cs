using Microsoft.EntityFrameworkCore;
using ModularMonolith.Modules.Products.Domain.Entities;
using ModularMonolith.Modules.Products.Domain.Repositories;
using ModularMonolith.Modules.Products.Infrastructure.Persistence;
using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Products.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product, Guid>, IProductRepository
{
    private readonly ProductsDbContext _context;

    public ProductRepository(ProductsDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.StockQuantity <= threshold && p.IsActive)
            .ToListAsync(cancellationToken);
    }
}
