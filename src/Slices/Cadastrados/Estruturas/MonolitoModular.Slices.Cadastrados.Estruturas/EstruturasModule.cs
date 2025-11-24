using MonolitoModular.Slices.Estruturas.Infrastructure;
// Exemplo: usando cliente gRPC de outro slice
// using MonolitoModular.Slices.Users.Grpc;

namespace MonolitoModular.Slices.Cadastrados.Estruturas;

public class EstruturasModule : ISliceModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {        
        // Configuração do DbContext (descomente e ajuste conforme necessário)
        // services.AddDbContext<EstruturasDbContext>(options =>
        //    options.UseSqlServer(
        //        configuration.GetConnectionString("DefaultConnection"),
        //        b => b.MigrationsAssembly("MonolitoModular.Host")));
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(EstruturasModule).Assembly));

        services.AddGrpc();

        // Exemplo: usando cliente gRPC de outro slice
        // services.AddGrpcClient<UsersService.UsersServiceClient>(options =>
        // {
        //    options.Address = new Uri(configuration["GrpcSettings:UsersServiceUrl"] ?? "http://localhost:5000");
        // });
    }
}
