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

# Crie o Module.cs
$moduleClass = @"
namespace MonolitoModular.Slices.$Contexto.$Slice;

public static class ${Slice}Module
{
    public static void Register${Slice}Slice(this IEndpointRouteBuilder endpoints)
    {
        // Mapear endpoints do slice aqui
    }
}
"@
$moduleFile = "$root\${Slice}Module.cs"
$moduleClass | Set-Content $moduleFile

# Crie o .csproj
@"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\..\..\Shared\MonolitoModular.Shared.Contracts\MonolitoModular.Shared.Contracts.csproj" />
    <ProjectReference Include="..\..\..\..\Shared\MonolitoModular.Shared.Infrastructure\MonolitoModular.Shared.Infrastructure.csproj" />
  </ItemGroup>
</Project>
"@ | Set-Content "$root\MonolitoModular.Slices.$Contexto.$Slice.csproj"

# Adiciona o novo projeto à solução
$csprojPath = "$root\MonolitoModular.Slices.$Contexto.$Slice.csproj"
dotnet sln MonolitoModular.sln add $csprojPath

Write-Host "Estrutura do slice '$Slice' criada em $root e adicionada à solução."