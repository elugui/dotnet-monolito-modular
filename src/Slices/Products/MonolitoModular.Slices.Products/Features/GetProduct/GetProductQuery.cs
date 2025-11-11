using MonolitoModular.Slices.Products.Domain;
using MonolitoModular.Slices.Products.Infrastructure;

namespace MonolitoModular.Slices.Products.Features.GetProduct;

public record GetProductQuery(Guid Id) : IRequest<Product?>;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Product?>
{
    private readonly ProductsDbContext _context;

    public GetProductQueryHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
    }
}
