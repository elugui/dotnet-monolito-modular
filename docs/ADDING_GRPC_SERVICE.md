# Como Adicionar um Novo Serviço gRPC

Guia passo a passo para adicionar um novo serviço gRPC a um slice.

## 📋 Pré-requisitos

- Slice já criado e configurado
- Entidades de domínio definidas
- Handlers MediatR implementados

## 🔧 Passo a Passo

### 1. Criar o Arquivo .proto

Crie o arquivo de contrato gRPC em `Protos/`:

```bash
# Exemplo para Orders slice
touch src/Slices/Orders/MonolitoModular.Slices.Orders/Protos/orders.proto
```

**Conteúdo do arquivo `orders.proto`:**

```protobuf
syntax = "proto3";

option csharp_namespace = "MonolitoModular.Slices.Orders.Grpc";

package monolitomodular.slices.orders.v1;

// Service definition
service OrdersService {
  rpc GetOrder (GetOrderRequest) returns (GetOrderResponse);
  rpc ListOrders (ListOrdersRequest) returns (ListOrdersResponse);
  rpc CreateOrder (CreateOrderRequest) returns (CreateOrderResponse);
  rpc CancelOrder (CancelOrderRequest) returns (CancelOrderResponse);
}

// Request messages
message GetOrderRequest {
  string id = 1;
}

message ListOrdersRequest {
  int32 page_number = 1;
  int32 page_size = 2;
  string user_id = 3;
  string status = 4;
}

message CreateOrderRequest {
  string user_id = 1;
  repeated OrderItemDto items = 2;
}

message CancelOrderRequest {
  string id = 1;
  string reason = 2;
}

// Response messages
message GetOrderResponse {
  OrderDto order = 1;
}

message ListOrdersResponse {
  repeated OrderDto orders = 1;
  int32 total_count = 2;
}

message CreateOrderResponse {
  string order_id = 1;
  string status = 2;
}

message CancelOrderResponse {
  bool success = 1;
  string message = 2;
}

// DTOs
message OrderDto {
  string id = 1;
  string user_id = 2;
  repeated OrderItemDto items = 3;
  string status = 4;
  double total_amount = 5;
  string created_at = 6;
}

message OrderItemDto {
  string product_id = 1;
  int32 quantity = 2;
  double unit_price = 3;
}
```

### 2. Configurar o .csproj

Adicione a referência ao .proto no arquivo `.csproj` do slice:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  
  <!-- Propriedades existentes -->
  
  <ItemGroup>
    <!-- Adicionar Grpc.Tools se ainda não existir -->
    <PackageReference Include="Grpc.Tools" Version="2.71.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <!-- Configurar como servidor gRPC -->
    <Protobuf Include="Protos\orders.proto" GrpcServices="Server" />
  </ItemGroup>

</Project>
```

### 3. Criar o Diretório Grpc

```bash
mkdir -p src/Slices/Orders/MonolitoModular.Slices.Orders/Grpc
```

### 4. Implementar o Serviço gRPC

Crie `OrdersGrpcService.cs` em `Grpc/`:

```csharp
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MonolitoModular.Slices.Orders.Features;

namespace MonolitoModular.Slices.Orders.Grpc;

public class OrdersGrpcService : OrdersService.OrdersServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersGrpcService> _logger;

    public OrdersGrpcService(IMediator mediator, ILogger<OrdersGrpcService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task<GetOrderResponse> GetOrder(
        GetOrderRequest request, 
        ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC GetOrder called for ID: {OrderId}", request.Id);

            if (!Guid.TryParse(request.Id, out var orderId))
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument, 
                    "Invalid order ID format"));
            }

            var order = await _mediator.Send(
                new GetOrderQuery(orderId), 
                context.CancellationToken);

            if (order is null)
            {
                throw new RpcException(new Status(
                    StatusCode.NotFound, 
                    $"Order {request.Id} not found"));
            }

            return new GetOrderResponse
            {
                Order = MapToDto(order)
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", request.Id);
            throw new RpcException(new Status(
                StatusCode.Internal, 
                "An error occurred while getting order"));
        }
    }

    public override async Task<ListOrdersResponse> ListOrders(
        ListOrdersRequest request, 
        ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC ListOrders called");

            var query = new ListOrdersQuery
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                UserId = !string.IsNullOrEmpty(request.UserId) 
                    ? Guid.Parse(request.UserId) 
                    : null,
                Status = request.Status
            };

            var orders = await _mediator.Send(query, context.CancellationToken);

            var response = new ListOrdersResponse
            {
                TotalCount = orders.TotalCount
            };

            response.Orders.AddRange(orders.Items.Select(MapToDto));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing orders");
            throw new RpcException(new Status(
                StatusCode.Internal, 
                "An error occurred while listing orders"));
        }
    }

    private static OrderDto MapToDto(Domain.Order order)
    {
        var dto = new OrderDto
        {
            Id = order.Id.ToString(),
            UserId = order.UserId.ToString(),
            Status = order.Status.ToString(),
            TotalAmount = (double)order.TotalAmount,
            CreatedAt = order.CreatedAt.ToString("O")
        };

        dto.Items.AddRange(order.Items.Select(item => new OrderItemDto
        {
            ProductId = item.ProductId.ToString(),
            Quantity = item.Quantity,
            UnitPrice = (double)item.UnitPrice
        }));

        return dto;
    }
}
```

### 5. Registrar o Serviço no Módulo

Atualize `OrdersModule.cs`:

```csharp
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
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(OrdersModule).Assembly));

        // Register gRPC services
        services.AddGrpc();
    }
}
```

### 6. Mapear o Endpoint gRPC no Host

Atualize `Program.cs`:

```csharp
using MonolitoModular.Slices.Orders.Grpc;

// ... código existente ...

app.MapGrpcService<OrdersGrpcService>();
```

### 7. Build e Teste

```bash
# Build para gerar código a partir do .proto
dotnet build

# Verificar se os arquivos foram gerados
ls -la src/Slices/Orders/MonolitoModular.Slices.Orders/obj/Debug/net9.0/Protos/
```

## 🔌 Consumindo o Serviço de Outro Slice

### 1. Adicionar Referência ao .proto no Slice Consumidor

No `.csproj` do slice que vai **consumir** o serviço:

```xml
<ItemGroup>
  <!-- Adicionar como Client -->
  <Protobuf Include="..\..\..\Slices\Orders\MonolitoModular.Slices.Orders\Protos\orders.proto" 
            GrpcServices="Client" 
            Link="Protos\orders.proto" />
</ItemGroup>
```

### 2. Registrar o Cliente no Módulo Consumidor

```csharp
using MonolitoModular.Slices.Orders.Grpc;

public class PaymentsModule : ISliceModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // ... outras configurações ...

        // Registrar cliente gRPC para Orders
        services.AddGrpcClient<OrdersService.OrdersServiceClient>(options =>
        {
            options.Address = new Uri(
                configuration["GrpcSettings:OrdersServiceUrl"] 
                ?? "http://localhost:5000");
        });
    }
}
```

### 3. Usar o Cliente

```csharp
public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, Result>
{
    private readonly OrdersService.OrdersServiceClient _ordersClient;

    public ProcessPaymentHandler(OrdersService.OrdersServiceClient ordersClient)
    {
        _ordersClient = ordersClient;
    }

    public async Task<Result> Handle(
        ProcessPaymentCommand request, 
        CancellationToken cancellationToken)
    {
        // Obter detalhes do pedido via gRPC
        var order = await _ordersClient.GetOrderAsync(
            new GetOrderRequest { Id = request.OrderId },
            cancellationToken: cancellationToken);

        // Processar pagamento...
    }
}
```

## ✅ Checklist

- [ ] Arquivo `.proto` criado em `Protos/`
- [ ] `.csproj` atualizado com referência ao proto
- [ ] Serviço gRPC implementado em `Grpc/`
- [ ] Mapeamento de DTOs implementado
- [ ] Tratamento de erros adequado
- [ ] Logging implementado
- [ ] Endpoint mapeado no `Program.cs`
- [ ] Build bem-sucedido
- [ ] Testes criados (unitários e integração)
- [ ] Documentação atualizada

## 📝 Padrões de Nomenclatura

### Arquivos
- Proto: `{slice-name}.proto` (minúsculo)
- Serviço: `{Slice}GrpcService.cs` (PascalCase)

### Proto Definitions
- Service: `{Slice}Service`
- Messages: `{Action}{Entity}Request/Response`
- DTOs: `{Entity}Dto`

### Namespaces
- Proto: `monolitomodular.slices.{slice}.v1`
- C# Namespace: `MonolitoModular.Slices.{Slice}.Grpc`

## 🚨 Erros Comuns

### 1. Código não é gerado após adicionar .proto

**Solução:** Execute `dotnet build` ou limpe e rebuilde:
```bash
dotnet clean
dotnet build
```

### 2. StatusCode.Unimplemented

**Solução:** Verifique se o endpoint foi mapeado no `Program.cs`:
```csharp
app.MapGrpcService<YourGrpcService>();
```

### 3. Namespace conflitos

**Solução:** Use `option csharp_namespace` no .proto:
```protobuf
option csharp_namespace = "MonolitoModular.Slices.{Slice}.Grpc";
```

## 💡 Dicas

1. **Versione seus contratos**: Use sufixos como `v1`, `v2` no package
2. **Documente os RPC**: Use comentários no .proto
3. **Valide inputs**: Sempre valide antes de processar
4. **Use DTOs**: Não exponha entidades de domínio diretamente
5. **Implemente idempotência**: Especialmente para operações de escrita

## 🔗 Links Úteis

- [Protocol Buffers Language Guide](https://protobuf.dev/programming-guides/proto3/)
- [gRPC for .NET](https://learn.microsoft.com/en-us/aspnet/core/grpc/)
- [gRPC Best Practices](https://learn.microsoft.com/en-us/aspnet/core/grpc/performance)

---

**Última atualização:** 2025-11-12
