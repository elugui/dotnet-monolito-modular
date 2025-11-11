# Como Adicionar um Novo Slice

Este guia mostra como adicionar um novo slice ao monolito modular, seguindo o padrão de Slice Architecture.

## Exemplo: Adicionar Slice de "Orders" (Pedidos)

### Passo 1: Criar o Projeto

```bash
# Navegue até o diretório raiz
cd /path/to/dotnet-monolito-modular

# Crie a estrutura de diretórios
mkdir -p src/Slices/Orders

# Crie o projeto
dotnet new classlib -n MonolitoModular.Slices.Orders -o src/Slices/Orders/MonolitoModular.Slices.Orders

# Adicione ao solution
dotnet sln add src/Slices/Orders/MonolitoModular.Slices.Orders/MonolitoModular.Slices.Orders.csproj
```

### Passo 2: Adicionar Referências

```bash
# Referências aos projetos Shared
dotnet add src/Slices/Orders/MonolitoModular.Slices.Orders/MonolitoModular.Slices.Orders.csproj \
  reference src/Shared/MonolitoModular.Shared.Contracts/MonolitoModular.Shared.Contracts.csproj

dotnet add src/Slices/Orders/MonolitoModular.Slices.Orders/MonolitoModular.Slices.Orders.csproj \
  reference src/Shared/MonolitoModular.Shared.Infrastructure/MonolitoModular.Shared.Infrastructure.csproj

# Adicionar referência no Host
dotnet add src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj \
  reference src/Slices/Orders/MonolitoModular.Slices.Orders/MonolitoModular.Slices.Orders.csproj
```

### Passo 3: Criar Estrutura de Diretórios

```bash
cd src/Slices/Orders/MonolitoModular.Slices.Orders

# Criar diretórios
mkdir -p Domain
mkdir -p Infrastructure
mkdir -p Features/CreateOrder
mkdir -p Features/GetOrder
mkdir -p Features/ListOrders
mkdir -p Grpc

# Remover arquivo padrão
rm Class1.cs
```

### Passo 4: Criar GlobalUsings.cs

```csharp
// GlobalUsings.cs
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.EntityFrameworkCore;
global using MediatR;
global using MonolitoModular.Shared.Contracts;
global using MonolitoModular.Shared.Infrastructure;
```

### Passo 5: Criar Entidade de Domínio

```csharp
// Domain/Order.cs
namespace MonolitoModular.Slices.Orders.Domain;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}
```

### Passo 6: Criar DbContext

```csharp
// Infrastructure/OrdersDbContext.cs
using MonolitoModular.Slices.Orders.Domain;

namespace MonolitoModular.Slices.Orders.Infrastructure;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Schema isolado
        modelBuilder.HasDefaultSchema("orders");

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            
            // Configurar OrderItems como owned entity
            entity.OwnsMany(e => e.Items, items =>
            {
                items.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
                items.Property(i => i.UnitPrice).HasPrecision(18, 2);
            });
        });
    }
}
```

### Passo 7: Criar Features (CQRS)

```csharp
// Features/CreateOrder/CreateOrderCommand.cs
using MonolitoModular.Slices.Orders.Domain;
using MonolitoModular.Slices.Orders.Infrastructure;

namespace MonolitoModular.Slices.Orders.Features.CreateOrder;

public record CreateOrderCommand(
    Guid CustomerId,
    List<OrderItemDto> Items) : IRequest<Guid>;

public record OrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly OrdersDbContext _context;

    public CreateOrderCommandHandler(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = request.CustomerId,
            Items = request.Items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        order.TotalAmount = order.Items.Sum(i => i.TotalPrice);

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }

    private string GenerateOrderNumber() => 
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}
```

```csharp
// Features/GetOrder/GetOrderQuery.cs
using MonolitoModular.Slices.Orders.Domain;
using MonolitoModular.Slices.Orders.Infrastructure;

namespace MonolitoModular.Slices.Orders.Features.GetOrder;

public record GetOrderQuery(Guid Id) : IRequest<Order?>;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Order?>
{
    private readonly OrdersDbContext _context;

    public GetOrderQueryHandler(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        return await _context.Orders.FindAsync(new object[] { request.Id }, cancellationToken);
    }
}
```

### Passo 8: Criar Módulo

```csharp
// OrdersModule.cs
using MonolitoModular.Slices.Orders.Infrastructure;

namespace MonolitoModular.Slices.Orders;

public class OrdersModule : ISliceModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("MonolitoModular.Host")));

        // Register MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrdersModule).Assembly));

        // Register gRPC services (se necessário)
        services.AddGrpc();
    }
}
```

### Passo 9: Registrar no Host

```csharp
// src/Host/MonolitoModular.Host/Program.cs
using MonolitoModular.Slices.Orders; // Adicionar using

// Na seção de registro de módulos
var sliceModules = new ISliceModule[]
{
    new UsersModule(),
    new ProductsModule(),
    new OrdersModule() // Adicionar aqui
};
```

### Passo 10: Criar Controller

```csharp
// src/Host/MonolitoModular.Host/Controllers/OrdersController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonolitoModular.Slices.Orders.Features.CreateOrder;
using MonolitoModular.Slices.Orders.Features.GetOrder;

namespace MonolitoModular.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _mediator.Send(new GetOrderQuery(id));
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var orderId = await _mediator.Send(new CreateOrderCommand(
            request.CustomerId,
            request.Items));
        
        return CreatedAtAction(nameof(GetById), new { id = orderId }, new { id = orderId });
    }
}

public record CreateOrderRequest(Guid CustomerId, List<OrderItemDto> Items);
```

### Passo 11: Build e Testar

```bash
# Build
dotnet build

# Executar
dotnet run --project src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj

# Testar
curl -X POST https://localhost:5001/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "items": [
      {
        "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
        "productName": "Notebook",
        "quantity": 2,
        "unitPrice": 3500.00
      }
    ]
  }'
```

## Checklist para Novo Slice

- [ ] Criar projeto classlib
- [ ] Adicionar ao solution
- [ ] Adicionar referências aos projetos Shared
- [ ] Criar estrutura de diretórios (Domain, Infrastructure, Features, Grpc)
- [ ] Criar GlobalUsings.cs
- [ ] Criar entidades de domínio
- [ ] Criar DbContext com schema isolado
- [ ] Criar Commands e Queries (Features)
- [ ] Criar Module para registro de serviços
- [ ] Adicionar referência no Host
- [ ] Registrar módulo no Program.cs
- [ ] Criar Controller REST
- [ ] Testar endpoints

## Boas Práticas

1. **Isolamento**: Cada slice deve ser independente
2. **Schema Separado**: Use `HasDefaultSchema()` para isolamento de banco
3. **CQRS**: Separe Commands (escrita) de Queries (leitura)
4. **Feature Folders**: Organize código por funcionalidade, não por tipo técnico
5. **gRPC**: Use para comunicação inter-slice quando necessário
6. **Sem Referências Cruzadas**: Slices não devem referenciar outros slices diretamente

## Comunicação Inter-Slice

Se precisar comunicação entre slices, use gRPC:

```csharp
// Em OrdersModule, para buscar dados de Users
services.AddGrpcClient<UsersService.UsersServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:5001");
});
```

Consulte a documentação de gRPC para mais detalhes.
