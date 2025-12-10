using MonolitoModular.Slices.Cadastrados.Estruturas.Infrastructure;
// Exemplo: usando cliente gRPC de outro slice
// using MonolitoModular.Slices.Users.Grpc;

namespace MonolitoModular.Slices.Cadastrados.Estruturas;

public class EstruturasModule : ISliceModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {        
        services.AddDbContext<EstruturasDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("MonolitoModular.Slices.Cadastrados.Estruturas")));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(EstruturasModule).Assembly));

        services.AddGrpc();
        services.AddScoped<Grpc.EstruturasGrpcService>();

        // Exemplo: usando cliente gRPC de outro slice
        // services.AddGrpcClient<UsersService.UsersServiceClient>(options =>
        // {
        //    options.Address = new Uri(configuration["GrpcSettings:UsersServiceUrl"] ?? "http://localhost:5000");
        // });
    }
}
