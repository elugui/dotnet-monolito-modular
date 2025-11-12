using MonolitoModular.Slices.Products.Infrastructure;
using MonolitoModular.Slices.Users.Grpc;

namespace MonolitoModular.Slices.Products;

public class ProductsModule : ISliceModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ProductsDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("MonolitoModular.Host")));

        // Register MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProductsModule).Assembly));

        // Register gRPC services
        services.AddGrpc();

        // Register gRPC client for Users slice (inter-slice communication)
        services.AddGrpcClient<UsersService.UsersServiceClient>(options =>
        {
            // In-process communication - same host
            options.Address = new Uri(configuration["GrpcSettings:UsersServiceUrl"] ?? "http://localhost:5000");
        });
    }
}
