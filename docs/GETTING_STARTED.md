# Getting Started

This guide will help you get the Modular Monolith project up and running on your local machine.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (or Docker)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional, for containerized deployment)

## Installation Steps

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/dotnet-monolito-modular.git
cd dotnet-monolito-modular
```

### 2. Build the Solution

```bash
dotnet restore
dotnet build
```

### 3. Configure Database

#### Option A: Using Local SQL Server

Update the connection strings in `src/Gateway/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "CustomersDb": "Server=localhost;Database=ModularMonolith_Customers;Integrated Security=true;",
    "ProductsDb": "Server=localhost;Database=ModularMonolith_Products;Integrated Security=true;"
  }
}
```

#### Option B: Using Docker SQL Server

Start SQL Server in Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

### 4. Run Database Migrations

```bash
# For Customers module
dotnet ef migrations add InitialCreate --project src/Modules/Customers/Infrastructure --startup-project src/Gateway --context CustomersDbContext
dotnet ef database update --project src/Modules/Customers/Infrastructure --startup-project src/Gateway --context CustomersDbContext

# For Products module
dotnet ef migrations add InitialCreate --project src/Modules/Products/Infrastructure --startup-project src/Gateway --context ProductsDbContext
dotnet ef database update --project src/Modules/Products/Infrastructure --startup-project src/Gateway --context ProductsDbContext
```

### 5. Run the Application

```bash
dotnet run --project src/Gateway
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger`

## Using Docker Compose

For a complete containerized setup:

```bash
docker-compose up -d
```

This will start:
- SQL Server container
- API Gateway container

Access the API at `http://localhost:5000`

## Testing the API

### Create a Customer

```bash
curl -X POST "http://localhost:5000/api/customers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890"
  }'
```

### Get All Customers

```bash
curl -X GET "http://localhost:5000/api/customers"
```

### Create a Product

```bash
curl -X POST "http://localhost:5000/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop",
    "description": "High-performance laptop",
    "price": 999.99,
    "stockQuantity": 50,
    "currency": "USD"
  }'
```

### Get Product by ID

```bash
curl -X GET "http://localhost:5000/api/products/{id}"
```

## Development Tools

### Visual Studio 2022

1. Open `ModularMonolith.sln`
2. Set `ModularMonolith.Gateway` as startup project
3. Press F5 to run

### VS Code

1. Open the project folder
2. Install recommended extensions:
   - C# Dev Kit
   - REST Client
3. Press F5 to run

### Recommended VS Code Extensions

- **C# Dev Kit**: Official C# extension
- **REST Client**: Test API endpoints
- **Docker**: Manage containers
- **GitLens**: Enhanced Git integration

## Project Structure Overview

```
├── src/
│   ├── Gateway/                 # Entry point, API Gateway
│   ├── Shared/                  # Shared kernel
│   │   ├── Domain/              # Base entities, value objects
│   │   ├── Application/         # CQRS patterns, behaviors
│   │   └── Infrastructure/      # Base repository, DbContext
│   └── Modules/
│       ├── Customers/           # Customer management module
│       └── Products/            # Product management module
├── docs/                        # Documentation
├── Dockerfile                   # Docker image definition
├── docker-compose.yml           # Multi-container setup
└── ModularMonolith.sln          # Solution file
```

## Next Steps

- Review the [Architecture Documentation](./ARCHITECTURE.md)
- Explore the [API Documentation](./API_REFERENCE.md)
- Learn about [Adding New Modules](./ADDING_MODULES.md)
- Check [Development Guidelines](./DEVELOPMENT_GUIDELINES.md)

## Troubleshooting

### Port Already in Use

If port 5000 or 5001 is already in use, update `launchSettings.json` in the Gateway project.

### Database Connection Failed

1. Check SQL Server is running
2. Verify connection string
3. Ensure firewall allows connection

### Build Errors

```bash
dotnet clean
dotnet restore
dotnet build
```

## Getting Help

- Check existing issues on GitHub
- Read the documentation in `/docs`
- Contact the development team

## Contributing

Please read `CONTRIBUTING.md` for guidelines on contributing to this project.
