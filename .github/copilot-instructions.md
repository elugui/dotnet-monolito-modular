

 # Instruções do Copilot para dotnet-monolito-modular

Guia rápido, objetivo e prático para deixar agentes de IA produtivos neste repositório.

## 🏗️ Visão geral da arquitetura
- Projeto .NET 9 **Modular Monolith** com padrão de "vertical slices". Cada slice é um módulo vertical independente com superfície API, modelo de domínio, persistência (EF DbContext) e interface gRPC.
- Layout dos slices: `src/Slices/{Context}/{Slice}/MonolitoModular.Slices.{Context}.{Slice}/`.
- Pastas típicas em um slice:
  - `Domain/` — entidades e tipos de domínio
  - `Infrastructure/` — `DbContext`, repositórios e configurações (use `HasDefaultSchema` para isolar schema)
  - `Features/` — implementação CQRS: `Commands`, `Queries`, `Handlers` (MediatR)
  - `Grpc/` — implementação do serviço gRPC do slice
  - `Protos/` — arquivos `.proto` (compilam para tipos gerados)
  - `Api/` (opcional) — controllers específicos do slice (normalmente controllers ficam no `Host`)
  - `{Slice}Module.cs` e `GlobalUsings.cs` — registro de DI e usings globais

## ✅ Convenções e padrões importantes (específicos do projeto)
- Validação: handlers fazem validação de entrada e lançam `ArgumentException` para dados inválidos.
- Entidade não encontrada: handlers lançam `InvalidOperationException("X não encontrada.")` (há exemplos reais; siga esse padrão).
- Acesso ao banco: injete o `DbContext` do slice no construtor do handler e use `FindAsync(new object[] { key }, cancellationToken)` para recuperar por chave.
- Organização: Commands/Queries ficam em `Features/{FeatureName}/` com `XCommand.cs` e `XHandler.cs`.
- Comunicação entre slices: **não** usar referências diretas entre projetos; use gRPC clients (veja `docs/GRPC_USAGE_GUIDE.md`).
- Schema por slice: cada DbContext deve usar schema próprio (ex.: `HasDefaultSchema("Estruturas")`).

## 🔧 Como adicionar um novo slice (checklist rápido)
1. Criar o projeto classlib em `src/Slices/{Context}/{Slice}/` e seguir a estrutura de exemplo.
2. Adicionar `GlobalUsings.cs` e `{Slice}Module.cs` implementando `ISliceModule` e `RegisterServices(IServiceCollection, IConfiguration)`.
3. Adicionar o projeto à solução e um `<ProjectReference />` no `src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj`.
4. Registrar o módulo em `Program.cs` adicionando `new {Slice}Module()` ao array `sliceModules` e chamar `module.RegisterServices(...)`.
5. Criar migrations EF e aplicar: `dotnet ef migrations add <Name> --project <slice.csproj> --startup-project src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj --context <SliceDbContext> --output-dir Migrations`.
6. Aplicar migrations: `dotnet ef database update --project <slice.csproj> --startup-project src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj --context <SliceDbContext>`.
7. Adicionar testes: unitários para handlers e testes de integração usando `WebApplicationFactory<Program>` para gRPC/HTTP.
8. Atualizar documentação: `docs/ADDING_NEW_SLICE.md`, `PROJECT_SUMMARY.md`, e `docs/ENDPOINTS_ESTRUTURA.md` quando necessário.

## 🛠️ Fluxos de desenvolvimento (PowerShell)
- Build: `dotnet build` ou `dotnet build --configuration Release`
- Rodar host localmente (Kestrel configurado em `Program.cs`):
  - `dotnet run --project src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj`
- Testes: `dotnet test` ou `dotnet test tests/MonolitoModular.Slices.Cadastrados.Estruturas.Tests`
- Docker: `docker-compose up --build` (inclui SQL Server configurado no `docker-compose.yml`).
- Exemplos EF (usar `--startup-project` apontando para o Host):
  - Adicionar migration: `dotnet ef migrations add InitialCreate --project src/Slices/Users/MonolitoModular.Slices.Users/MonolitoModular.Slices.Users.csproj --startup-project src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj --context UsersDbContext --output-dir Migrations`
  - Gerar script idempotente: `dotnet ef migrations script --project <slice.csproj> --startup-project src/Host/... --context <SliceDbContext> --idempotent -o Scripts/Initial.sql`

## 🔁 gRPC e testes de integração
- gRPC são mapeados em `Program.cs` via `app.MapGrpcService<...>()` (ex.: `EstruturasGrpcService`).
- Kestrel é configurado para HTTP/1.1 e HTTP/2 — ver portas em `Program.cs` (HTTP/2 normalmente em 5000 no projeto).
- Testes unitários de gRPC usam TestServer; testes de integração podem usar `WebApplicationFactory<Program>` para rodar serviços reais.

## 🔎 Locais úteis (exemplos práticos)
- Módulo (exemplo): `src/Slices/Cadastrados/Estruturas/MonolitoModular.Slices.Cadastrados.Estruturas/EstruturasModule.cs`
- Controller: `src/Slices/Cadastrados/Estruturas/.../Api/EstruturaController.cs` (ou controllers no `Host` que delegam a MediatR)
- Handlers: `src/Slices/Cadastrados/Estruturas/.../Features/CreateEstrutura/CreateEstruturaHandler.cs`, `UpdateEstruturaHandler.cs` (veja validações e uso do DbContext)
- gRPC e proto: `src/Slices/Cadastrados/Estruturas/.../Grpc/EstruturasGrpcService.cs`, `Protos/estruturas.proto`

## ⚠️ Boas práticas e restrições
- NÃO crie referências de projeto cruzadas entre slices — utilize gRPC.
- NÃO altere convenções existentes (ex.: tipo de exceção para validação/nao-encontrado) sem uma boa razão.
- Atualize documentação sempre que adicionar um slice ou endpoints.

---
Se quiser, posso adicionar um **modelo (template)** de slice ou um **snippet de teste** na mesma documentação — quer que eu inclua um exemplo?
---
If any section is unclear or you want more examples (e.g., a template for a new slice or a sample handler test), tell me which part and I'll add a focused snippet.
