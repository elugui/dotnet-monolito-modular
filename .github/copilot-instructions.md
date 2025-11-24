# Copilot Instructions for dotnet-monolito-modular

## 🏗️ Arquitetura e Organização

- O projeto segue o padrão **Monólito Modular** com **Slice Architecture**: cada slice é um módulo vertical auto-contido (API, lógica, persistência, gRPC).
- Slices ficam em `src/Slices/{ContextoMacro}/{SliceName}/MonolitoModular.Slices.{ContextoMacro}.{SliceName}/`.
- Cada slice tem:  
  - `Domain/` (entidades e lógica de negócio)  
  - `Infrastructure/` (DbContext, persistência, schema isolado)  
  - `Features/` (casos de uso CQRS via MediatR)  
  - `Grpc/` (serviços gRPC para comunicação interna)  
  - `Protos/` (contratos gRPC)  
  - `GlobalUsings.cs` e `{Slice}Module.cs` (registro de serviços)
- Slices não se referenciam diretamente: comunicação entre slices é feita via gRPC, nunca por referência de projeto.

## 🛠️ Workflows Essenciais

- **Build:**  
  `dotnet build` (ou `dotnet build --configuration Release`)
- **Testes:**  
  `dotnet test`
- **Rodar local:**  
  `dotnet run --project src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj`
- **Docker:**  
  `docker-compose up --build`
- **Adicionar novo slice:**  
  1. Crie a estrutura de pastas e projeto classlib  
  2. Adicione referências aos projetos Shared  
  3. Implemente Domain, Infrastructure, Features, Grpc  
  4. Registre o módulo no Host  
  5. Siga exemplos em `docs/ADDING_NEW_SLICE.md`

## 🧩 Padrões e Convenções

- **CQRS:** Commands e Queries organizados em subpastas de `Features/`, handlers usam MediatR.
- **DbContext:** Cada slice tem seu próprio DbContext e schema (use `HasDefaultSchema`).
- **gRPC:**  
  - Contratos em `Protos/`, serviços em `Grpc/`  
  - Consumo entre slices via cliente gRPC, nunca por referência direta  
  - Veja exemplos em `docs/GRPC_USAGE_GUIDE.md` e `docs/ADDING_GRPC_SERVICE.md`
- **Controllers REST:** Ficam no Host, delegam para MediatR.
- **Shared Kernel:** Código compartilhado em `src/Shared/Contracts` e `src/Shared/Infrastructure`.

## 🔗 Exemplos e Referências

- Veja exemplos completos de slices em `src/Slices/Users/` e `src/Slices/Products/`.
- Para adicionar slices, siga o checklist e exemplos em `docs/ADDING_NEW_SLICE.md`.
- Para gRPC, siga os padrões de `docs/ADDING_GRPC_SERVICE.md` e `docs/GRPC_USAGE_GUIDE.md`.
- Arquitetura detalhada em `docs/ARCHITECTURE.md`.

## ⚠️ Atenção

- Não crie dependências cruzadas entre slices.
- Sempre use gRPC para comunicação inter-slice.
- Mantenha cada slice isolado e testável.
- Atualize a documentação ao criar novos slices ou serviços.
