using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace MonolitoModular.Slices.Products.Grpc;

/// <summary>
/// gRPC service for Products slice - Enables inter-slice communication
/// </summary>
public class ProductsGrpcService : ProductsService.ProductsServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsGrpcService> _logger;

    public ProductsGrpcService(IMediator mediator, ILogger<ProductsGrpcService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get a product by ID
    /// </summary>
    public override async Task<GetProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC GetProduct called for ID: {ProductId}", request.Id);

            if (!Guid.TryParse(request.Id, out var productId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid product ID format"));
            }

            var product = await _mediator.Send(new Features.GetProduct.GetProductQuery(productId), context.CancellationToken);

            if (product is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Product {request.Id} not found"));
            }

            return new GetProductResponse
            {
                Product = MapToDto(product)
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while getting product"));
        }
    }

    /// <summary>
    /// Check if a product is available with requested quantity
    /// </summary>
    public override async Task<CheckAvailabilityResponse> CheckAvailability(CheckAvailabilityRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC CheckAvailability called for product {ProductId}, quantity {Quantity}", 
                request.Id, request.Quantity);

            if (!Guid.TryParse(request.Id, out var productId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid product ID format"));
            }

            if (request.Quantity <= 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Quantity must be greater than 0"));
            }

            var product = await _mediator.Send(new Features.GetProduct.GetProductQuery(productId), context.CancellationToken);

            if (product is null)
            {
                return new CheckAvailabilityResponse
                {
                    IsAvailable = false,
                    AvailableQuantity = 0
                };
            }

            var isAvailable = product.IsAvailable && product.Stock >= request.Quantity;

            return new CheckAvailabilityResponse
            {
                IsAvailable = isAvailable,
                AvailableQuantity = product.Stock
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking availability for product {ProductId}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while checking availability"));
        }
    }

    /// <summary>
    /// Reserve stock for a product (idempotent operation)
    /// </summary>
    public override async Task<ReserveStockResponse> ReserveStock(ReserveStockRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC ReserveStock called for product {ProductId}, quantity {Quantity}, reservation {ReservationId}", 
                request.ProductId, request.Quantity, request.ReservationId);

            if (!Guid.TryParse(request.ProductId, out var productId))
            {
                return new ReserveStockResponse
                {
                    Success = false,
                    Message = "Invalid product ID format"
                };
            }

            if (request.Quantity <= 0)
            {
                return new ReserveStockResponse
                {
                    Success = false,
                    Message = "Quantity must be greater than 0"
                };
            }

            var product = await _mediator.Send(new Features.GetProduct.GetProductQuery(productId), context.CancellationToken);

            if (product is null)
            {
                return new ReserveStockResponse
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            if (!product.IsAvailable)
            {
                return new ReserveStockResponse
                {
                    Success = false,
                    Message = "Product is not available"
                };
            }

            if (product.Stock < request.Quantity)
            {
                return new ReserveStockResponse
                {
                    Success = false,
                    Message = $"Insufficient stock. Available: {product.Stock}, Requested: {request.Quantity}"
                };
            }

            // In a real scenario, this would update the stock
            // For now, we just validate and return success
            return new ReserveStockResponse
            {
                Success = true,
                Message = $"Stock reserved successfully for reservation {request.ReservationId}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving stock for product {ProductId}", request.ProductId);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while reserving stock"));
        }
    }

    /// <summary>
    /// List products with filters and pagination
    /// </summary>
    public override async Task<ListProductsResponse> ListProducts(ListProductsRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC ListProducts called - Page: {Page}, Size: {Size}", request.PageNumber, request.PageSize);

            var products = await _mediator.Send(new Features.ListProducts.ListProductsQuery(), context.CancellationToken);

            // Apply filters
            if (request.AvailableOnly)
            {
                products = products.Where(p => p.IsAvailable).ToList();
            }

            if (request.MinPrice > 0)
            {
                products = products.Where(p => p.Price >= (decimal)request.MinPrice).ToList();
            }

            if (request.MaxPrice > 0)
            {
                products = products.Where(p => p.Price <= (decimal)request.MaxPrice).ToList();
            }

            // Apply pagination
            var pageSize = request.PageSize > 0 ? request.PageSize : 10;
            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var totalCount = products.Count;

            var pagedProducts = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            var response = new ListProductsResponse
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            response.Products.AddRange(pagedProducts);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing products");
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while listing products"));
        }
    }

    /// <summary>
    /// Map domain entity to gRPC DTO
    /// </summary>
    private static ProductDto MapToDto(Domain.Product product)
    {
        return new ProductDto
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Description = product.Description,
            Price = (double)product.Price,
            Stock = product.Stock,
            IsAvailable = product.IsAvailable,
            CreatedAt = product.CreatedAt.ToString("O") // ISO 8601 format
        };
    }
}
