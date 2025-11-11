using MonolitoModular.Slices.Products.Domain;
using MonolitoModular.Slices.Products.Infrastructure;

namespace MonolitoModular.Slices.Products.Features.ListProducts;

public record ListProductsQuery : IRequest<List<Product>>;

public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, List<Product>>
{
    private readonly ProductsDbContext _context;

    public ListProductsQueryHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }
}
