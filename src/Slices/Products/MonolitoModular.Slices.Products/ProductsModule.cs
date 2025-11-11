using MonolitoModular.Slices.Products.Infrastructure;

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
    }
}
