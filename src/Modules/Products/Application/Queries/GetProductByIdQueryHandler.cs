using ModularMonolith.Modules.Products.Application.DTOs;
using ModularMonolith.Modules.Products.Domain.Repositories;
using ModularMonolith.Shared.Application.Queries;

namespace ModularMonolith.Modules.Products.Application.Queries;

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
            return null;
        }

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price.Amount,
            product.Price.Currency,
            product.StockQuantity,
            product.CreatedAt,
            product.UpdatedAt,
            product.IsActive
        );
    }
}
