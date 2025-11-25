# Guia de Início Rápido

## Pré-requisitos

- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server** - LocalDB, Express ou Docker
- **IDE** (Opcional): Visual Studio 2022, VS Code ou Rider

## Passo 1: Clonar o Repositório

```bash
git clone https://github.com/elugui/dotnet-monolito-modular.git
cd dotnet-monolito-modular
```

## Passo 2: Restaurar Dependências

```bash
dotnet restore
```

## Passo 3: Configurar Banco de Dados

### Opção A: SQL Server LocalDB (Windows)

A string de conexão padrão em `appsettings.json` já está configurada para LocalDB:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MonolitoModular;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### Opção B: Docker (Linux/Mac/Windows)

Use o docker-compose incluído:

```bash
docker-compose up -d sqlserver
```

Atualize a connection string em `appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=MonolitoModular;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
}
```

## Passo 4: Executar o Projeto

```bash
cd src/Host/MonolitoModular.Host
dotnet run
```

A API estará disponível em:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

## Passo 5: Testar a API

### Usando cURL

**Criar um usuário:**
```bash
curl -X POST https://localhost:5033/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"João Silva","email":"joao@example.com"}'
```

**Listar usuários:**
```bash
curl https://localhost:5001/api/users
```

**Criar um produto:**
```bash
curl -X POST https://localhost:5001/api/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Notebook","description":"Laptop Dell","price":3500.00,"stock":10}'
```

**Listar produtos:**
```bash
curl https://localhost:5001/api/products
```

## Passo 6: Explorar a Documentação da API

Acesse a documentação OpenAPI (em desenvolvimento):
```
https://localhost:5033/openapi/v1.json
```
```
https://localhost:5033/openapi/v1.json
```

## Executar com Docker

Para executar toda a aplicação em containers:

```bash
docker-compose up --build
```

A API estará disponível em `http://localhost:5033`

## Estrutura de Diretórios

```
MonolitoModular/
├── src/
│   ├── Host/                    # 🚀 Ponto de entrada da aplicação
│   ├── Shared/                  # 📦 Código compartilhado
│   │   ├── Contracts/          # Interface e contratos
│   │   └── Infrastructure/     # Classes base
│   └── Slices/                 # 🍕 Slices verticais
│       ├── Users/              # Slice de Usuários
│       └── Products/           # Slice de Produtos
├── docs/                       # 📚 Documentação
├── Dockerfile                  # 🐳 Configuração Docker
└── docker-compose.yml          # 🐳 Orquestração
```

## Próximos Passos

1. **Ler a Arquitetura**: Veja [ARCHITECTURE.md](./ARCHITECTURE.md) para entender o design
2. **Adicionar um Novo Slice**: Siga o padrão existente em `src/Slices/`
3. **Adicionar Autenticação**: Implemente JWT no Host
4. **Adicionar Testes**: Crie testes unitários e de integração
5. **Configurar CI/CD**: Use GitHub Actions ou Azure DevOps

## Comandos Úteis

```bash
# Build
dotnet build

# Build em Release
dotnet build --configuration Release

# Executar testes (quando implementados)
dotnet test

# Limpar artifacts
dotnet clean

# Publicar para produção
dotnet publish -c Release -o ./publish
```

## Troubleshooting

### Erro de Conexão com Banco de Dados

Verifique se o SQL Server está rodando:
```bash
# Para Docker
docker ps | grep sqlserver

# Para LocalDB (Windows)
sqllocaldb info
```

### Porta já em uso

Altere as portas em `Properties/launchSettings.json` ou use:
```bash
dotnet run --urls "https://localhost:6001;http://localhost:6000"
```

### Erro de certificado SSL

Em desenvolvimento, confie no certificado:
```bash
dotnet dev-certs https --trust
```

## Suporte

- 📖 [Documentação Completa](../README.md)
- 🏗️ [Guia de Arquitetura](./ARCHITECTURE.md)
- 🐛 [Reportar Issues](https://github.com/elugui/dotnet-monolito-modular/issues)
