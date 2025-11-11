using ModularMonolith.Modules.Products.Domain.Entities;
using ModularMonolith.Shared.Domain.Repositories;

namespace ModularMonolith.Modules.Products.Domain.Repositories;

/// <summary>
/// Repository interface for Product aggregate
/// </summary>
public interface IProductRepository : IRepository<Product, Guid>
{
    Task<IEnumerable<Product>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold, CancellationToken cancellationToken = default);
}
