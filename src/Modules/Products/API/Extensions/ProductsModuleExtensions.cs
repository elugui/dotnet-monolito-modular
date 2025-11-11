using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularMonolith.Modules.Products.Application.Commands;
using ModularMonolith.Modules.Products.Domain.Repositories;
using ModularMonolith.Modules.Products.Infrastructure.Persistence;
using ModularMonolith.Modules.Products.Infrastructure.Repositories;
using ModularMonolith.Shared.Application.Interfaces;

namespace ModularMonolith.Modules.Products.API.Extensions;

public static class ProductsModuleExtensions
{
    public static IServiceCollection AddProductsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ProductsDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ProductsDb") ?? 
                "Server=localhost;Database=ModularMonolith_Products;Trusted_Connection=true;TrustServerCertificate=true",
                b => b.MigrationsAssembly(typeof(ProductsDbContext).Assembly.FullName)));

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ProductsDbContext>());

        // Register MediatR handlers from Application assembly
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

        return services;
    }
}
