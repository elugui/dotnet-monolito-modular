# Análise e Estratégia de Implementação gRPC

## 📋 Resumo Executivo

Este documento apresenta uma análise completa da implementação de gRPC no monólito modular e propõe uma estratégia abrangente para comunicação inter-slice usando gRPC, priorizando **performance**, **baixo acoplamento** e **testabilidade**.

## 🔍 Análise da Situação Atual

### Estado Atual da Infraestrutura gRPC

#### ✅ O que já existe:
- Pacote `Grpc.AspNetCore` v2.71.0 instalado
- `services.AddGrpc()` chamado nos módulos Users e Products
- Estrutura de pastas preparada para serviços gRPC
- Domain entities bem definidas (User, Product)

#### ❌ O que está faltando:
- Arquivos `.proto` para definição de contratos
- Implementação de serviços gRPC
- Mapeamento de endpoints gRPC no Program.cs
- Clientes gRPC configurados para comunicação inter-slice
- Estratégia de versionamento de contratos
- Documentação de uso

### Dependências Atuais

```xml
<!-- MonolitoModular.Shared.Infrastructure.csproj -->
<PackageReference Include="Grpc.AspNetCore" Version="2.71.0" />
<PackageReference Include="MediatR" Version="13.1.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />

<!-- MonolitoModular.Host.csproj -->
<PackageReference Include="Grpc.AspNetCore" Version="2.71.0" />
```

### ⚙️ Requisito para Compilação de Arquivos .proto

Para que os arquivos `.proto` sejam corretamente compilados e os tipos gRPC gerados no build, é obrigatório instalar o pacote NuGet `Grpc.Tools` **no projeto onde está o arquivo `.proto`**:

```powershell
dotnet add <projeto-do-slice>.csproj package Grpc.Tools
```
> Atenção:
> O pacote Grpc.Tools é necessário apenas em projetos do tipo class library > que definem contratos gRPC. Ele não precisa ser instalado no projeto Host > (apenas nos slices que expõem serviços gRPC).


## 🎯 Estratégia Proposta

### 1. Organização de Contratos (.proto)

#### Estratégia de Localização
Cada slice terá sua própria pasta `Protos/` contendo:
- Definições de serviços gRPC
- Mensagens de request/response
- Enums e tipos compartilhados do slice

```
MonolitoModular.Slices.Users/
├── Protos/
│   └── users.proto          # Contrato do serviço de usuários
├── Grpc/
│   └── UsersGrpcService.cs  # Implementação do serviço
```

#### Nomenclatura e Versionamento
- Arquivos: `{slice_name}.proto` (ex: `users.proto`, `products.proto`)
- Pacotes: `monolitomodular.slices.{slice}.v1`
- Serviços: `{Slice}Service` (ex: `UsersService`, `ProductsService`)

### 2. Geração Automática a partir de Entidades de Domínio

#### Mapeamento Entidade → Proto

**Princípios:**
- Entidades de domínio são a **fonte da verdade**
- Contratos proto refletem as entidades mas são **DTOs simplificados**
- Evitar expor detalhes internos de implementação
- Incluir apenas dados necessários para comunicação inter-slice

**Exemplo de Mapeamento:**

```csharp
// Domain/User.cs
public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}
```

```protobuf
// Protos/users.proto
message UserDto {
    string id = 1;
    string name = 2;
    string email = 3;
    bool is_active = 4;
}
```

### 3. Arquitetura de Comunicação Inter-Slice

#### Modelo de Comunicação

```
┌─────────────────────────────────────────────────────────────┐
│                     Products Slice                          │
│                                                               │
│  ProductsController → MediatR Handler                        │
│                           ↓                                   │
│                    Precisa validar User                       │
│                           ↓                                   │
│              UsersService.UsersServiceClient ────────┐       │
│                                                       │       │
└───────────────────────────────────────────────────────│───────┘
                                                        │ gRPC
┌───────────────────────────────────────────────────────│───────┐
│                      Users Slice                      │       │
│                                                       ↓       │
│              Grpc/UsersGrpcService.cs ◄──────────────┘       │
│                           ↓                                   │
│                    UsersDbContext                             │
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

### 4. Implementação de Serviços gRPC

#### Template de Serviço

Cada serviço gRPC deve:
1. Herdar de `{Service}Base` gerado pelo proto
2. Injetar dependências necessárias (DbContext, MediatR)
3. Implementar métodos RPC
4. Tratar erros adequadamente
5. Usar logging estruturado

```csharp
public class UsersGrpcService : UsersService.UsersServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersGrpcService> _logger;

    public UsersGrpcService(IMediator mediator, ILogger<UsersGrpcService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task<GetUserResponse> GetUser(
        GetUserRequest request, 
        ServerCallContext context)
    {
        // Implementação
    }
}
```

### 5. Configuração de Clientes gRPC

#### Registro de Clientes

Clientes gRPC devem ser registrados usando `HttpClientFactory` pattern:

```csharp
// No módulo que CONSOME o serviço
services.AddGrpcClient<UsersService.UsersServiceClient>(options =>
{
    options.Address = new Uri("http://localhost:5000"); // In-process
});
```

#### Comunicação In-Process

Como estamos em um monólito:
- Usar `http://localhost` como endereço base
- Configurar Kestrel para servir gRPC na mesma porta
- Sem necessidade de service discovery
- Comunicação através de HTTP/2

### 6. Estrutura de Endpoints

#### Roteamento gRPC

```csharp
// Program.cs
app.MapGrpcService<UsersGrpcService>();
app.MapGrpcService<ProductsGrpcService>();

// Opcional: Habilitar gRPC-Web para browsers
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
```

## 🏗️ Plano de Implementação

### Fase 1: Infraestrutura Base
1. ✅ Criar estrutura de pastas `Protos/` e `Grpc/`
2. ✅ Adicionar pacotes necessários
3. ✅ Configurar geração de código a partir de .proto

### Fase 2: Slice Users (Servidor)
1. ✅ Criar `users.proto` com operações CRUD
2. ✅ Implementar `UsersGrpcService`
3. ✅ Mapear endpoint no Program.cs
4. ✅ Adicionar testes unitários

### Fase 3: Slice Products (Cliente)
1. ✅ Adicionar referência ao proto de Users
2. ✅ Configurar cliente gRPC para Users
3. ✅ Criar exemplo de comunicação inter-slice
4. ✅ Implementar `ProductsGrpcService` (servidor)

### Fase 4: Documentação e Boas Práticas
1. ✅ Documentar padrões de uso
2. ✅ Criar guia de adicionar novo serviço gRPC
3. ✅ Exemplos de testes de integração

## 🎨 Padrões e Boas Práticas

### 1. Isolamento e Baixo Acoplamento

**✅ Fazer:**
- Cada slice expõe apenas operações essenciais via gRPC
- Contratos proto são versionados
- Clientes dependem apenas de contratos, não de implementações

**❌ Evitar:**
- Compartilhar classes de domínio entre slices
- Expor detalhes internos de implementação
- Criar dependências circulares

### 2. Performance

**Otimizações:**
- Usar streaming quando apropriado
- Configurar compressão gRPC
- Implementar timeout e retry policies
- Cache de clientes gRPC

```csharp
services.AddGrpcClient<UsersService.UsersServiceClient>(options =>
{
    options.Address = new Uri("http://localhost:5000");
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    EnableMultipleHttp2Connections = true,
    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
    KeepAlivePingTimeout = TimeSpan.FromSeconds(30)
});
```

### 3. Tratamento de Erros

```csharp
public override async Task<GetUserResponse> GetUser(
    GetUserRequest request, 
    ServerCallContext context)
{
    try
    {
        var user = await _mediator.Send(new GetUserQuery(Guid.Parse(request.Id)));
        
        if (user is null)
        {
            throw new RpcException(new Status(
                StatusCode.NotFound, 
                $"User {request.Id} not found"));
        }
        
        return MapToResponse(user);
    }
    catch (Exception ex) when (ex is not RpcException)
    {
        _logger.LogError(ex, "Error getting user {UserId}", request.Id);
        throw new RpcException(new Status(
            StatusCode.Internal, 
            "An error occurred"));
    }
}
```

### 4. Testabilidade

**Testes de Unidade:**
```csharp
// Mockar o cliente gRPC
var mockClient = new Mock<UsersService.UsersServiceClient>();
mockClient.Setup(x => x.GetUserAsync(It.IsAny<GetUserRequest>(), ...))
    .ReturnsAsync(new GetUserResponse { ... });
```

**Testes de Integração:**
```csharp
// Usar TestServer do ASP.NET Core
var factory = new WebApplicationFactory<Program>();
var channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, ...);
var client = new UsersService.UsersServiceClient(channel);
```

## 📊 Comparação: REST vs gRPC

| Aspecto | REST/JSON | gRPC |
|---------|-----------|------|
| **Performance** | Moderada | Alta (binary protocol) |
| **Latência** | ~50-100ms | ~10-20ms (in-process) |
| **Tamanho Payload** | Maior (JSON) | Menor (Protobuf) |
| **Streaming** | Limitado | Excelente |
| **Tipagem** | Runtime | Compile-time |
| **Browser Support** | Nativo | Requer gRPC-Web |
| **Debugging** | Fácil | Requer ferramentas |

### Quando Usar Cada Um

**REST/JSON** (Externo):
- APIs públicas
- Integrações com terceiros
- Clientes web/mobile

**gRPC** (Interno):
- Comunicação entre slices
- Operações de alta frequência
- Quando performance é crítica

## 🔒 Considerações de Segurança

### 1. Autenticação e Autorização

```csharp
// Interceptor para validação de chamadas
public class AuthInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        // Validar token, permissões, etc.
        return await continuation(request, context);
    }
}
```

### 2. Rate Limiting

```csharp
services.AddGrpc(options =>
{
    options.Interceptors.Add<RateLimitInterceptor>();
});
```

## 🚀 Migração Futura para Microsserviços

Esta arquitetura facilita a extração para microsserviços:

1. **Hoje (Monólito):**
   - gRPC in-process
   - Endereço: `http://localhost:5000`

2. **Amanhã (Microsserviços):**
   - gRPC over network
   - Service discovery (Consul, Eureka)
   - Endereço: `http://users-service:5000`

**Mudança necessária:** Apenas configuração de endereço do cliente!

## 📚 Recursos Adicionais

- [gRPC for .NET Documentation](https://learn.microsoft.com/en-us/aspnet/core/grpc/)
- [Protocol Buffers Guide](https://protobuf.dev/programming-guides/proto3/)
- [gRPC Best Practices](https://learn.microsoft.com/en-us/aspnet/core/grpc/performance)

## 🎯 Próximos Passos

1. Implementar serviço gRPC para Users
2. Criar exemplo de comunicação Products → Users
3. Adicionar health checks para serviços gRPC
4. Documentar padrões em código de exemplo
5. Criar template para novos serviços gRPC

---

**Documento criado em:** 2025-11-12  
**Versão:** 1.0  
**Autor:** Backend .NET Slice Architect Agent
