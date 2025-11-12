using MonolitoModular.Slices.Products.Domain;
using MonolitoModular.Slices.Products.Infrastructure;
using MonolitoModular.Slices.Users.Grpc;
using Microsoft.Extensions.Logging;

namespace MonolitoModular.Slices.Products.Features.CreateProductWithUserValidation;

/// <summary>
/// Example of inter-slice communication using gRPC
/// This command validates the user via gRPC before creating a product
/// </summary>
public record CreateProductWithUserValidationCommand(
    string Name, 
    string Description, 
    decimal Price, 
    int Stock,
    string CreatedByUserId) : IRequest<Guid>;

public class CreateProductWithUserValidationHandler : IRequestHandler<CreateProductWithUserValidationCommand, Guid>
{
    private readonly ProductsDbContext _context;
    private readonly UsersService.UsersServiceClient _usersClient;
    private readonly ILogger<CreateProductWithUserValidationHandler> _logger;

    public CreateProductWithUserValidationHandler(
        ProductsDbContext context, 
        UsersService.UsersServiceClient usersClient,
        ILogger<CreateProductWithUserValidationHandler> logger)
    {
        _context = context;
        _usersClient = usersClient;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateProductWithUserValidationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product with user validation. User ID: {UserId}", request.CreatedByUserId);

        // INTER-SLICE COMMUNICATION: Validate user via gRPC
        try
        {
            var validationResponse = await _usersClient.ValidateUserAsync(
                new ValidateUserRequest { Id = request.CreatedByUserId },
                cancellationToken: cancellationToken);

            if (!validationResponse.IsValid)
            {
                _logger.LogWarning("User validation failed: {Reason}", validationResponse.Reason);
                throw new InvalidOperationException($"Cannot create product: {validationResponse.Reason}");
            }

            _logger.LogInformation("User {UserId} validated successfully via gRPC", request.CreatedByUserId);
        }
        catch (global::Grpc.Core.RpcException ex)
        {
            _logger.LogError(ex, "gRPC call to Users service failed");
            throw new InvalidOperationException("Failed to validate user via gRPC", ex);
        }

        // Create product after successful validation
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} created successfully by user {UserId}", 
            product.Id, request.CreatedByUserId);

        return product.Id;
    }
}
