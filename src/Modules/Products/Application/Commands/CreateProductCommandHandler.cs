using ModularMonolith.Modules.Products.Application.DTOs;
using ModularMonolith.Modules.Products.Domain.Entities;
using ModularMonolith.Modules.Products.Domain.Repositories;
using ModularMonolith.Shared.Application.Commands;
using ModularMonolith.Shared.Application.Interfaces;

namespace ModularMonolith.Modules.Products.Application.Commands;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Name, 
            request.Description, 
            request.Price, 
            request.StockQuantity, 
            request.Currency);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
