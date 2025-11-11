# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["MonolitoModular.sln", "./"]
COPY ["src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj", "src/Host/MonolitoModular.Host/"]
COPY ["src/Shared/MonolitoModular.Shared.Contracts/MonolitoModular.Shared.Contracts.csproj", "src/Shared/MonolitoModular.Shared.Contracts/"]
COPY ["src/Shared/MonolitoModular.Shared.Infrastructure/MonolitoModular.Shared.Infrastructure.csproj", "src/Shared/MonolitoModular.Shared.Infrastructure/"]
COPY ["src/Slices/Users/MonolitoModular.Slices.Users/MonolitoModular.Slices.Users.csproj", "src/Slices/Users/MonolitoModular.Slices.Users/"]
COPY ["src/Slices/Products/MonolitoModular.Slices.Products/MonolitoModular.Slices.Products.csproj", "src/Slices/Products/MonolitoModular.Slices.Products/"]

# Restore dependencies
RUN dotnet restore "MonolitoModular.sln"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/src/Host/MonolitoModular.Host"
RUN dotnet build "MonolitoModular.Host.csproj" -c Release -o /app/build
RUN dotnet publish "MonolitoModular.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MonolitoModular.Host.dll"]
