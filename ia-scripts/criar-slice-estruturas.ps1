param(
    [Parameter(Mandatory = $true)]
    [string]$Contexto,
    [Parameter(Mandatory = $true)]
    [string]$Slice
)

$root = "src\Slices\$Contexto\$Slice\MonolitoModular.Slices.$Contexto.$Slice"

# Crie as pastas principais
New-Item -ItemType Directory -Force -Path "$root\Api"
New-Item -ItemType Directory -Force -Path "$root\Domain"
New-Item -ItemType Directory -Force -Path "$root\Features"
New-Item -ItemType Directory -Force -Path "$root\Grpc"
New-Item -ItemType Directory -Force -Path "$root\Infrastructure"
New-Item -ItemType Directory -Force -Path "$root\Protos"

# Crie GlobalUsings.cs
@"
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using Microsoft.EntityFrameworkCore;
global using MediatR;
global using MonolitoModular.Shared.Contracts;
global using MonolitoModular.Shared.Infrastructure;
"@ | Set-Content "$root\GlobalUsings.cs"

# Crie o Module.cs (padrão completo)
$moduleClass = @"
// using MonolitoModular.Slices.$Slice.Infrastructure; 
// Exemplo: usando cliente gRPC de outro slice
// using MonolitoModular.Slices.Users.Grpc;

namespace MonolitoModular.Slices.$Contexto.$Slice;

public class ${Slice}Module : ISliceModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {        
        // Configuração do DbContext (descomente e ajuste conforme necessário)
        // services.AddDbContext<${Slice}DbContext>(options =>
        //    options.UseSqlServer(
        //        configuration.GetConnectionString("DefaultConnection"),
        //        b => b.MigrationsAssembly("MonolitoModular.Host")));
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(${Slice}Module).Assembly));

        services.AddGrpc();

        // Exemplo: usando cliente gRPC de outro slice
        // services.AddGrpcClient<UsersService.UsersServiceClient>(options =>
        // {
        //    options.Address = new Uri(configuration["GrpcSettings:UsersServiceUrl"] ?? "http://localhost:5000");
        // });
    }
}
"@
$moduleFile = "$root\${Slice}Module.cs"
$moduleClass | Set-Content $moduleFile

# Crie o .csproj (corrigido: aspas simples)
@"
<Project Sdk='Microsoft.NET.Sdk'>
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include='..\..\..\..\Shared\MonolitoModular.Shared.Contracts\MonolitoModular.Shared.Contracts.csproj' />
    <ProjectReference Include='..\..\..\..\Shared\MonolitoModular.Shared.Infrastructure\MonolitoModular.Shared.Infrastructure.csproj' />
  </ItemGroup>
</Project>
"@ | Set-Content "$root\MonolitoModular.Slices.$Contexto.$Slice.csproj"

# Adiciona o novo projeto à solução
$csprojPath = "$root\MonolitoModular.Slices.$Contexto.$Slice.csproj"
dotnet sln MonolitoModular.sln add $csprojPath

Write-Host "Estrutura do slice '$Slice' criada em $root e adicionada à solução."