using MonolitoModular.Slices.Users.Infrastructure;

namespace MonolitoModular.Slices.Users;

public class UsersModule : ISliceModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("MonolitoModular.Host")));

        // Register MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UsersModule).Assembly));

        // Register gRPC services
        services.AddGrpc();
    }
}
