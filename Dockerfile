# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ModularMonolith.sln .

# Copy project files
COPY src/Gateway/*.csproj src/Gateway/
COPY src/Shared/Domain/*.csproj src/Shared/Domain/
COPY src/Shared/Application/*.csproj src/Shared/Application/
COPY src/Shared/Infrastructure/*.csproj src/Shared/Infrastructure/
COPY src/Modules/Customers/Domain/*.csproj src/Modules/Customers/Domain/
COPY src/Modules/Customers/Application/*.csproj src/Modules/Customers/Application/
COPY src/Modules/Customers/Infrastructure/*.csproj src/Modules/Customers/Infrastructure/
COPY src/Modules/Customers/API/*.csproj src/Modules/Customers/API/
COPY src/Modules/Products/Domain/*.csproj src/Modules/Products/Domain/
COPY src/Modules/Products/Application/*.csproj src/Modules/Products/Application/
COPY src/Modules/Products/Infrastructure/*.csproj src/Modules/Products/Infrastructure/
COPY src/Modules/Products/API/*.csproj src/Modules/Products/API/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build and publish
WORKDIR /src/src/Gateway
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "ModularMonolith.Gateway.dll"]
