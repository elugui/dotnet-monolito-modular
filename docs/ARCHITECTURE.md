# Arquitetura - Monolito Modular com Slice Architecture

## Visão Geral

Este projeto implementa um **monólito modular** usando **Slice Architecture** (Arquitetura de Fatias Verticais). Cada slice representa um módulo de negócio completo, desde a camada de apresentação até a persistência.

## Princípios Fundamentais

### 1. Vertical Slicing

Ao contrário da arquitetura em camadas tradicional (horizontal), organizamos o código por funcionalidades de negócio (vertical):

```
Tradicional (Horizontal):          Slice Architecture (Vertical):
┌─────────────────────┐            ┌──────┐  ┌──────┐  ┌──────┐
│   Presentation      │            │Users │  │Orders│  │Products│
├─────────────────────┤            │      │  │      │  │      │
│   Business Logic    │            │ API  │  │ API  │  │ API  │
├─────────────────────┤            │ Logic│  │Logic │  │Logic │
│   Data Access       │            │ Data │  │Data  │  │Data  │
└─────────────────────┘            └──────┘  └──────┘  └──────┘
```

### 2. Isolamento entre Slices

Cada slice é isolado e auto-contido:

- **DbContext próprio**: Cada slice gerencia sua própria persistência
- **Schema separado**: Isolamento a nível de banco de dados
- **Sem referências diretas**: Slices se comunicam via gRPC
- **Independência**: Mudanças em um slice não afetam outros

### 3. CQRS (Command Query Responsibility Segregation)

Separamos operações de leitura e escrita usando MediatR.

## Estrutura de um Slice

Cada slice segue esta estrutura:

```
MonolitoModular.Slices.{SliceName}/
├── Domain/                      # Entidades de domínio
│   └── {Entity}.cs
├── Infrastructure/              # Persistência
│   └── {Slice}DbContext.cs
├── Features/                    # Casos de uso (CQRS)
│   ├── Create{Entity}/
│   │   └── Create{Entity}Command.cs
│   ├── Get{Entity}/
│   │   └── Get{Entity}Query.cs
│   └── List{Entity}/
│       └── List{Entity}Query.cs
├── Grpc/                        # Serviços gRPC
│   └── {Slice}GrpcService.cs
├── GlobalUsings.cs              # Using globais
└── {Slice}Module.cs             # Registro de serviços
```

## Comunicação entre Slices

### REST/JSON - APIs Externas

Para comunicação com clientes externos (frontend, mobile):

```
Cliente → REST/JSON → Controller → MediatR → Handler → DbContext
```

### gRPC - Comunicação Interna

Para comunicação entre slices (alta performance).

## Migração para Microsserviços

Uma das principais vantagens desta arquitetura é a facilidade de extrair slices para microsserviços quando necessário.

## Benefícios

✅ **Organização**: Código agrupado por funcionalidade de negócio
✅ **Manutenção**: Fácil encontrar e modificar código relacionado
✅ **Escalabilidade**: Prepare-se para microsserviços
✅ **Testabilidade**: Teste cada slice independentemente
✅ **Performance**: gRPC para comunicação interna eficiente
✅ **Simplicidade**: Deploy único, menor complexidade operacional
