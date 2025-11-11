using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularMonolith.Modules.Customers.Application.Commands;
using ModularMonolith.Modules.Customers.Domain.Repositories;
using ModularMonolith.Modules.Customers.Infrastructure.Persistence;
using ModularMonolith.Modules.Customers.Infrastructure.Repositories;
using ModularMonolith.Shared.Application.Interfaces;

namespace ModularMonolith.Modules.Customers.API.Extensions;

public static class CustomersModuleExtensions
{
    public static IServiceCollection AddCustomersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<CustomersDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("CustomersDb") ?? 
                "Server=localhost;Database=ModularMonolith_Customers;Trusted_Connection=true;TrustServerCertificate=true",
                b => b.MigrationsAssembly(typeof(CustomersDbContext).Assembly.FullName)));

        // Register repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CustomersDbContext>());

        // Register MediatR handlers from Application assembly
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(CreateCustomerCommand).Assembly));

        return services;
    }
}
