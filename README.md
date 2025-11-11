# 🏗️ .NET Modular Monolith Template

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A complete **Modular Monolith** template built with **.NET 9.0**, implementing **Clean Architecture**, **Domain-Driven Design (DDD)**, and **CQRS** patterns.

## ✨ Features

- 🏛️ **Clean Architecture** with clear separation of concerns
- 📦 **Modular Design** for easy maintenance and scalability
- 🎯 **Domain-Driven Design** with rich domain models
- 🔄 **CQRS Pattern** using MediatR
- 🗄️ **Database per Module** strategy with EF Core
- 🔌 **gRPC Support** for inter-module communication
- 📝 **OpenAPI/Swagger** documentation
- 🐳 **Docker Support** for containerization
- 🧪 **Testable Architecture** with dependency injection
- 📊 **Health Checks** for monitoring

## 🚀 Quick Start

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) or [Docker](https://www.docker.com/)

### Run Locally

```bash
# Clone the repository
git clone https://github.com/elugui/dotnet-monolito-modular.git
cd dotnet-monolito-modular

# Build the solution
dotnet build

# Run the application
dotnet run --project src/Gateway
```

Access the API:
- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health

### Run with Docker

```bash
docker-compose up -d
```

## 📂 Project Structure

```
ModularMonolith/
├── src/
│   ├── Gateway/                          # API Gateway
│   ├── Shared/                           # Shared Kernel
│   │   ├── Domain/                       # Base entities, value objects
│   │   ├── Application/                  # CQRS patterns, behaviors
│   │   └── Infrastructure/               # Base repository, DbContext
│   └── Modules/                          # Business Modules
│       ├── Customers/                    # Customer Management
│       │   ├── Domain/                   # Entities, Value Objects, Events
│       │   ├── Application/              # Commands, Queries, Handlers
│       │   ├── Infrastructure/           # Repositories, DbContext
│       │   └── API/                      # Controllers, Extensions
│       └── Products/                     # Product Management
│           ├── Domain/
│           ├── Application/
│           ├── Infrastructure/
│           └── API/
├── docs/                                 # Documentation
├── Dockerfile                            # Container definition
└── docker-compose.yml                    # Multi-container setup
```

## 🎯 Architecture Highlights

### Clean Architecture Layers

1. **Domain Layer**: Pure business logic, entities, and value objects
2. **Application Layer**: Use cases, CQRS handlers, DTOs
3. **Infrastructure Layer**: Data access, external services
4. **API Layer**: Controllers, dependency injection setup

### Module Communication

- **Intra-Module**: Direct method calls, domain events
- **Inter-Module**: gRPC, message bus (future)

### Database Strategy

- Each module has its own database schema
- Supports migration to microservices
- Customers: `customers` schema
- Products: `products` schema

## 📚 Available Modules

### Customers Module
Manages customer information with CRUD operations.

**Endpoints:**
- `GET /api/customers` - List all customers
- `GET /api/customers/{id}` - Get customer by ID
- `POST /api/customers` - Create new customer

### Products Module
Manages product catalog and inventory.

**Endpoints:**
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product

## 🛠️ Technology Stack

- **Framework**: .NET 9.0
- **Web Framework**: ASP.NET Core
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server
- **Mediator**: MediatR
- **Validation**: FluentValidation
- **API Documentation**: Swashbuckle (Swagger)
- **RPC**: gRPC
- **Containerization**: Docker

## 📖 Documentation

- [Architecture Overview](docs/ARCHITECTURE.md)
- [Getting Started Guide](docs/GETTING_STARTED.md)

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 🔧 Development

### Adding a New Module

1. Create module structure following the template
2. Implement Domain, Application, Infrastructure, and API layers
3. Register module in Gateway's `Program.cs`
4. Add database migrations

### Code Style

- Follow C# coding conventions
- Use meaningful names
- Write self-documenting code
- Add XML comments for public APIs

## 🚢 Deployment

### Docker Deployment

```bash
docker build -t modular-monolith:latest .
docker run -p 5000:8080 modular-monolith:latest
```

### Cloud Deployment

- Azure App Service
- AWS Elastic Beanstalk
- Google Cloud Run
- Kubernetes

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

- 📧 Email: your.email@example.com
- 🐛 Issues: [GitHub Issues](https://github.com/elugui/dotnet-monolito-modular/issues)
- 💬 Discussions: [GitHub Discussions](https://github.com/elugui/dotnet-monolito-modular/discussions)

## 🙏 Acknowledgments

- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans
- Modular Monolith concepts by Kamil Grzybek

---

**Built with ❤️ using .NET 9.0**
