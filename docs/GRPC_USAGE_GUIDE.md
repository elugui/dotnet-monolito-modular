# Guia de Uso: gRPC Inter-Slice

Este guia fornece exemplos práticos de como usar gRPC para comunicação entre slices no monólito modular.

## 📚 Índice

1. [Visão Geral](#visão-geral)
2. [Estrutura de Arquivos](#estrutura-de-arquivos)
3. [Como Consumir um Serviço gRPC](#como-consumir-um-serviço-grpc)
4. [Exemplos Práticos](#exemplos-práticos)
5. [Testes](#testes)
6. [Troubleshooting](#troubleshooting)

## 🎯 Visão Geral

### Serviços gRPC Disponíveis

#### UsersService
**Localização:** `MonolitoModular.Slices.Users/Protos/users.proto`

**Operações:**
- `GetUser` - Obter usuário por ID
- `GetUserByEmail` - Obter usuário por email
- `UserExists` - Verificar se usuário existe
- `ValidateUser` - Validar se usuário é válido (existe e está ativo)
- `ListUsers` - Listar usuários com paginação

#### ProductsService
**Localização:** `MonolitoModular.Slices.Products/Protos/products.proto`

**Operações:**
- `GetProduct` - Obter produto por ID
- `CheckAvailability` - Verificar disponibilidade de estoque
- `ReserveStock` - Reservar estoque (operação idempotente)
- `ListProducts` - Listar produtos com filtros

## 📁 Estrutura de Arquivos

```
MonolitoModular.Slices.Users/
├── Protos/
│   └── users.proto              # Contrato gRPC
├── Grpc/
│   └── UsersGrpcService.cs      # Implementação do servidor
├── Features/                     # Handlers MediatR
└── UsersModule.cs               # Registro do serviço

MonolitoModular.Slices.Products/
├── Protos/
│   ├── products.proto           # Contrato próprio
│   └── users.proto (link)       # Referência ao contrato Users
├── Grpc/
│   └── ProductsGrpcService.cs   # Implementação do servidor
├── Features/
│   └── CreateProductWithUserValidation/
│       └── ...Command.cs        # Exemplo de uso do cliente
└── ProductsModule.cs            # Registro do serviço e cliente
```

## 🔧 Como Consumir um Serviço gRPC

### Passo 1: Adicionar Referência ao .proto

No arquivo `.csproj` do slice **consumidor**:

```xml
<ItemGroup>
  <Protobuf Include="..\..\..\Slices\Users\MonolitoModular.Slices.Users\Protos\users.proto" 
            GrpcServices="Client" 
            Link="Protos\users.proto" />
</ItemGroup>
```

### Passo 2: Registrar o Cliente gRPC

No módulo do slice consumidor:

```csharp
// ProductsModule.cs
using MonolitoModular.Slices.Users.Grpc;

public class ProductsModule : ISliceModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // ... outras configurações
        
        // Registrar cliente gRPC
        services.AddGrpcClient<UsersService.UsersServiceClient>(options =>
        {
            options.Address = new Uri(configuration["GrpcSettings:UsersServiceUrl"] 
                ?? "http://localhost:5000");
        });
    }
}
```

### Passo 3: Injetar e Usar o Cliente

No handler ou serviço:

```csharp
public class MeuHandler : IRequestHandler<MeuCommand, Resultado>
{
    private readonly UsersService.UsersServiceClient _usersClient;

    public MeuHandler(UsersService.UsersServiceClient usersClient)
    {
        _usersClient = usersClient;
    }

    public async Task<Resultado> Handle(MeuCommand request, CancellationToken cancellationToken)
    {
        // Fazer chamada gRPC
        var response = await _usersClient.GetUserAsync(
            new GetUserRequest { Id = userId },
            cancellationToken: cancellationToken);
            
        // Usar os dados retornados
        var userName = response.User.Name;
        // ...
    }
}
```

## 💡 Exemplos Práticos

### Exemplo 1: Validar Usuário Antes de Criar Produto

```csharp
public class CreateProductWithUserValidationHandler 
    : IRequestHandler<CreateProductWithUserValidationCommand, Guid>
{
    private readonly ProductsDbContext _context;
    private readonly UsersService.UsersServiceClient _usersClient;
    private readonly ILogger<CreateProductWithUserValidationHandler> _logger;

    public CreateProductWithUserValidationHandler(
        ProductsDbContext context,
        UsersService.UsersServiceClient usersClient,
        ILogger<CreateProductWithUserValidationHandler> logger)
    {
        _context = context;
        _usersClient = usersClient;
        _logger = logger;
    }

    public async Task<Guid> Handle(
        CreateProductWithUserValidationCommand request, 
        CancellationToken cancellationToken)
    {
        // 1. Validar usuário via gRPC
        var validationResponse = await _usersClient.ValidateUserAsync(
            new ValidateUserRequest { Id = request.CreatedByUserId },
            cancellationToken: cancellationToken);

        if (!validationResponse.IsValid)
        {
            throw new InvalidOperationException(
                $"Cannot create product: {validationResponse.Reason}");
        }

        // 2. Criar produto
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            // ...
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
```

### Exemplo 2: Verificar Existência de Usuário

```csharp
public async Task<bool> CheckUserExists(string userId)
{
    try
    {
        var response = await _usersClient.UserExistsAsync(
            new UserExistsRequest { Id = userId });
        
        return response.Exists;
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
    {
        return false;
    }
}
```

### Exemplo 3: Listar Usuários com Paginação

```csharp
public async Task<List<UserDto>> GetActiveUsers(int page, int pageSize)
{
    var response = await _usersClient.ListUsersAsync(
        new ListUsersRequest
        {
            PageNumber = page,
            PageSize = pageSize,
            ActiveOnly = true
        });

    return response.Users.ToList();
}
```

### Exemplo 4: Tratamento de Erros

```csharp
public async Task<UserDto?> GetUserSafely(string userId)
{
    try
    {
        var response = await _usersClient.GetUserAsync(
            new GetUserRequest { Id = userId });
        
        return response.User;
    }
    catch (RpcException ex)
    {
        switch (ex.StatusCode)
        {
            case StatusCode.NotFound:
                _logger.LogWarning("User {UserId} not found", userId);
                return null;
                
            case StatusCode.InvalidArgument:
                _logger.LogError("Invalid user ID format: {UserId}", userId);
                throw new ArgumentException("Invalid user ID", nameof(userId));
                
            case StatusCode.Unavailable:
                _logger.LogError("Users service unavailable");
                throw new ServiceUnavailableException("Users service is unavailable");
                
            default:
                _logger.LogError(ex, "Unexpected gRPC error");
                throw;
        }
    }
}
```

## 🧪 Testes

### Testar Serviço gRPC (Server)

```csharp
public class UsersGrpcServiceTests
{
    [Fact]
    public async Task GetUser_ValidId_ReturnsUser()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var loggerMock = new Mock<ILogger<UsersGrpcService>>();
        
        var user = new User 
        { 
            Id = Guid.NewGuid(), 
            Name = "Test User",
            Email = "test@example.com"
        };
        
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetUserQuery>(), default))
            .ReturnsAsync(user);

        var service = new UsersGrpcService(mediatorMock.Object, loggerMock.Object);
        var context = TestServerCallContext.Create();

        // Act
        var response = await service.GetUser(
            new GetUserRequest { Id = user.Id.ToString() }, 
            context);

        // Assert
        Assert.NotNull(response.User);
        Assert.Equal(user.Name, response.User.Name);
    }
}
```

### Testar Cliente gRPC (Client)

```csharp
public class ProductHandlerTests
{
    [Fact]
    public async Task CreateProduct_ValidUser_Success()
    {
        // Arrange
        var clientMock = new Mock<UsersService.UsersServiceClient>();
        
        clientMock
            .Setup(c => c.ValidateUserAsync(
                It.IsAny<ValidateUserRequest>(), 
                null, null, default))
            .Returns(new AsyncUnaryCall<ValidateUserResponse>(
                Task.FromResult(new ValidateUserResponse 
                { 
                    IsValid = true 
                }),
                null, null, null, null));

        var handler = new CreateProductWithUserValidationHandler(
            contextMock.Object,
            clientMock.Object,
            loggerMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        clientMock.Verify(c => c.ValidateUserAsync(
            It.IsAny<ValidateUserRequest>(), 
            null, null, default), Times.Once);
    }
}
```

### Teste de Integração

```csharp
public class GrpcIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GrpcIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetUser_Integration_Success()
    {
        // Arrange
        var channel = GrpcChannel.ForAddress(
            _factory.Server.BaseAddress,
            new GrpcChannelOptions { HttpHandler = _factory.Server.CreateHandler() });
        
        var client = new UsersService.UsersServiceClient(channel);

        // Act
        var response = await client.GetUserAsync(
            new GetUserRequest { Id = knownUserId.ToString() });

        // Assert
        Assert.NotNull(response.User);
    }
}
```

## 🔍 Troubleshooting

### Problema: "Service is unimplemented"

**Causa:** Endpoint gRPC não foi mapeado no Program.cs

**Solução:**
```csharp
// Program.cs
app.MapGrpcService<UsersGrpcService>();
```

### Problema: "Connection refused"

**Causa:** URL do cliente gRPC incorreta

**Solução:** Verificar configuração no módulo:
```csharp
services.AddGrpcClient<UsersService.UsersServiceClient>(options =>
{
    options.Address = new Uri("http://localhost:5000"); // Porta correta
});
```

### Problema: "Proto file not found during build"

**Causa:** Caminho do .proto no .csproj está incorreto

**Solução:**
```xml
<Protobuf Include="..\..\..\Slices\Users\MonolitoModular.Slices.Users\Protos\users.proto" 
          GrpcServices="Client" 
          Link="Protos\users.proto" />
```

### Problema: StatusCode.Internal em produção

**Causa:** Exceção não tratada no servidor

**Solução:** Sempre tratar exceções e retornar status codes apropriados:
```csharp
try
{
    // lógica
}
catch (NotFoundException)
{
    throw new RpcException(new Status(StatusCode.NotFound, "Resource not found"));
}
catch (ValidationException ex)
{
    throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
}
catch (Exception)
{
    throw new RpcException(new Status(StatusCode.Internal, "An error occurred"));
}
```

## 📝 Boas Práticas

1. **Sempre usar cancellation tokens**
   ```csharp
   await _client.GetUserAsync(request, cancellationToken: cancellationToken);
   ```

2. **Implementar retry policies** (opcional, via Polly)
   ```csharp
   services.AddGrpcClient<UsersService.UsersServiceClient>(...)
       .AddPolicyHandler(GetRetryPolicy());
   ```

3. **Logging estruturado**
   ```csharp
   _logger.LogInformation("gRPC call to GetUser for ID: {UserId}", userId);
   ```

4. **Validar inputs**
   ```csharp
   if (!Guid.TryParse(request.Id, out var id))
   {
       throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID"));
   }
   ```

5. **Usar metadata para contexto**
   ```csharp
   var headers = new Metadata
   {
       { "correlation-id", correlationId },
       { "user-agent", "products-service" }
   };
   await _client.GetUserAsync(request, headers);
   ```

## 🚀 Próximos Passos

- Implementar health checks para serviços gRPC
- Adicionar telemetria (OpenTelemetry)
- Configurar compressão gRPC
- Implementar streaming quando apropriado
- Adicionar autenticação/autorização

---

**Última atualização:** 2025-11-12
