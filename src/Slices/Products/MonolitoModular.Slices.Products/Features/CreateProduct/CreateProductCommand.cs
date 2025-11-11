using MonolitoModular.Slices.Products.Domain;
using MonolitoModular.Slices.Products.Infrastructure;

namespace MonolitoModular.Slices.Products.Features.CreateProduct;

public record CreateProductCommand(string Name, string Description, decimal Price, int Stock) : IRequest<Guid>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly ProductsDbContext _context;

    public CreateProductCommandHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
