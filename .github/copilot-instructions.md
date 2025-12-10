

# Copilot Instructions for dotnet-monolito-modular

## 🏗️ Arquitetura Modular
- Projeto .NET com **Slice Architecture**: cada slice é um módulo vertical autossuficiente (API, domínio, persistência, gRPC).
- Slices ficam em `src/Slices/{ContextoMacro}/{SliceName}/MonolitoModular.Slices.{ContextoMacro}.{SliceName}/`.
- Estrutura típica de slice:
  - `Domain/`: entidades e lógica de negócio
  - `Infrastructure/`: DbContext, persistência, schema isolado (`HasDefaultSchema` obrigatório)
  - `Features/`: casos de uso CQRS (Commands/Queries via MediatR)
  - `Grpc/`: serviços gRPC internos
  - `Protos/`: contratos gRPC
  - `GlobalUsings.cs` e `{Slice}Module.cs`: registro de serviços e usings globais
- **Slices nunca se referenciam diretamente**: comunicação entre slices é sempre via gRPC client (veja exemplos em `docs/GRPC_USAGE_GUIDE.md`).
- Shared Kernel/Building Blocks: apenas para contratos/utilitários comuns (`src/Shared/Contracts`, `src/Shared/Infrastructure`).

## 🛠️ Workflows Essenciais
- **Build:** `dotnet build` ou `dotnet build --configuration Release`
- **Testes:** `dotnet test`
- **Rodar local:** `dotnet run --project src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj`
- **Docker:** `docker-compose up --build` (usa SQL Server e API)
- **Migrations:**
  - Exemplo: `dotnet ef database update --project src/Slices/Users/MonolitoModular.Slices.Users/MonolitoModular.Slices.Users.csproj --startup-project src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj --context UsersDbContext`
- **Adicionar novo slice:**
  1. Crie a estrutura de pastas e projeto classlib
  2. Adicione referências aos Shared
  3. Implemente Domain, Infrastructure, Features, Grpc
  4. Registre o módulo no Host
  5. Siga exemplos em `docs/ADDING_NEW_SLICE.md`

## 🧩 Padrões e Convenções
- **CQRS:** Commands/Queries em subpastas de `Features/`, handlers usam MediatR.
- **DbContext:** Cada slice tem seu próprio contexto e schema.
- **gRPC:**
  - Contratos em `Protos/`, serviços em `Grpc/`
  - Comunicação entre slices sempre via gRPC client (nunca referência direta)
  - Siga `docs/ADDING_GRPC_SERVICE.md` para padrões de proto, versionamento e DTOs
- **Controllers REST:** Ficam no Host, delegam para MediatR.
- **GlobalUsings:** Cada slice tem seu próprio arquivo, importando apenas o necessário.
- **Testes:** Cada slice pode ter testes unitários e de integração em `tests/`
- **Scripts:** Automação em `ia-scripts/` (ex: criar-slice-estruturas.ps1)

## 🔗 Exemplos e Referências
- Exemplos completos: `src/Slices/Users/`, `src/Slices/Products/`, `src/Slices/Cadastrados/Estruturas/`
- Adição de slices: `docs/ADDING_NEW_SLICE.md`
- Padrões gRPC: `docs/ADDING_GRPC_SERVICE.md`, `docs/GRPC_USAGE_GUIDE.md`, `docs/GRPC_ANALYSIS.md`, `docs/GRPC_IMPLEMENTATION_SUMMARY.md`
- Arquitetura detalhada: `docs/ARCHITECTURE.md`
- Documentação endpoints: `docs/ENDPOINTS_ESTRUTURA.md`

## ⚠️ Atenção
- Nunca crie dependências cruzadas entre slices.
- Sempre use gRPC para comunicação inter-slice.
- Mantenha cada slice isolado, testável e com schema próprio.
- Atualize a documentação ao criar novos slices ou serviços.
- Siga exemplos reais dos slices existentes para novos desenvolvimentos.
