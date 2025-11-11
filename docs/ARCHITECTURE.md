# Architecture Documentation

## 📐 Modular Monolith Architecture

This project implements a **Modular Monolithic Architecture** using .NET 9.0, following Clean Architecture and Domain-Driven Design (DDD) principles.

### 🎯 Key Principles

1. **Modularity**: Each module is independent with its own domain, application, infrastructure, and API layers
2. **Clean Architecture**: Clear separation of concerns with dependency rules flowing inward
3. **Domain-Driven Design**: Rich domain models with business logic encapsulated in entities and value objects
4. **CQRS Pattern**: Separation of Command and Query responsibilities using MediatR
5. **Event-Driven**: Domain events for loose coupling between modules

## 🏗️ Project Structure

```
ModularMonolith/
├── src/
│   ├── Gateway/                          # API Gateway (Entry point)
│   ├── Shared/                           # Shared kernel
│   │   ├── Domain/                       # Shared domain abstractions
│   │   ├── Application/                  # Shared application patterns
│   │   └── Infrastructure/               # Shared infrastructure
│   └── Modules/                          # Business modules
│       ├── Customers/                    # Customer module
│       │   ├── Domain/                   # Entities, Value Objects, Events
│       │   ├── Application/              # Commands, Queries, DTOs
│       │   ├── Infrastructure/           # Data access, External services
│       │   └── API/                      # Controllers, Module registration
│       └── Products/                     # Product module
│           ├── Domain/
│           ├── Application/
│           ├── Infrastructure/
│           └── API/
```

## 📦 Module Structure

Each module follows Clean Architecture with 4 layers:

### 1. Domain Layer
- **Entities**: Business objects with identity (Customer, Product)
- **Value Objects**: Immutable objects without identity (Email, Money)
- **Domain Events**: Events that represent business facts
- **Repository Interfaces**: Contracts for data access
- **Business Rules**: Domain logic and validation

**Dependencies**: None (Pure domain logic)

### 2. Application Layer
- **Commands**: Write operations (CreateCustomer, UpdateProduct)
- **Queries**: Read operations (GetCustomerById, GetAllProducts)
- **Command/Query Handlers**: Business logic orchestration
- **DTOs**: Data transfer objects
- **Validators**: Input validation using FluentValidation

**Dependencies**: Domain Layer

### 3. Infrastructure Layer
- **DbContext**: Entity Framework Core database context
- **Repositories**: Implementation of repository interfaces
- **Persistence Configuration**: Entity mappings and schema
- **External Services**: Integration with external systems
- **gRPC Services**: Inter-module communication

**Dependencies**: Domain, Application

### 4. API Layer
- **Controllers**: HTTP endpoints
- **Module Extensions**: Dependency injection configuration
- **Filters**: Cross-cutting concerns

**Dependencies**: Application, Infrastructure

## 🔄 Communication Patterns

### Intra-Module Communication
- Direct method calls within the same module
- Domain events for asynchronous processing

### Inter-Module Communication
- **gRPC**: For synchronous module-to-module calls
- **Domain Events**: For asynchronous, decoupled communication
- **Mediator Pattern**: Using MediatR for request handling

## 🗄️ Data Access Strategy

### Database per Module
- Each module has its own database schema
- Customers module: `customers` schema
- Products module: `products` schema

### Benefits
- **Loose Coupling**: Modules can evolve independently
- **Scalability**: Can migrate to separate databases easily
- **Security**: Fine-grained access control

## 🔧 Technology Stack

- **.NET 9.0**: Runtime and framework
- **ASP.NET Core**: Web API
- **Entity Framework Core 9.0**: ORM
- **MediatR**: Mediator pattern implementation
- **FluentValidation**: Input validation
- **Swashbuckle**: OpenAPI/Swagger documentation
- **gRPC**: Inter-module communication
- **SQL Server**: Database

## 🚀 Running the Application

### Local Development
```bash
dotnet build
dotnet run --project src/Gateway
```

### With Docker
```bash
docker-compose up -d
```

### Access Points
- API: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`
- Health Check: `http://localhost:5000/health`

## 📚 API Endpoints

### Customers Module
- `GET /api/customers` - Get all customers
- `GET /api/customers/{id}` - Get customer by ID
- `POST /api/customers` - Create new customer

### Products Module
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product

## 🔐 Security Considerations

- Connection strings should be stored in Azure Key Vault or similar
- Add authentication/authorization middleware
- Implement rate limiting
- Add request validation
- Enable CORS appropriately

## 📈 Scalability Path

This modular monolith can evolve:

1. **Current**: Single deployment unit
2. **Next**: Extract modules as microservices
3. **Future**: Full microservices architecture

The modular boundaries make this transition smooth when needed.

## 🧪 Testing Strategy

- **Unit Tests**: Domain logic and value objects
- **Integration Tests**: Repository and database operations
- **API Tests**: Controller endpoints
- **Architecture Tests**: Verify module boundaries and dependencies

## 🔄 CI/CD Pipeline

Recommended pipeline:
1. Build & Test
2. Code Quality Checks (SonarQube)
3. Security Scanning
4. Docker Image Build
5. Deploy to Environment

## 📖 Additional Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/tags/domain%20driven%20design.html)
- [Modular Monoliths](https://www.kamilgrzybek.com/blog/posts/modular-monolith-primer)
