# Endpoints Estrutura

## REST

### Criar Estrutura
- **POST** `/api/estruturas`
- **Body:**
```json
{
  "nome": "Filial SP",
  "estruturaTipoCodigo": 1,
  "codigoExterno": "SP01",
  "inicioVigencia": "2025-01-01T00:00:00Z",
  "terminoVigencia": "2025-12-31T23:59:59Z",
  "versao": 1,
  "status": 1
}
```
- **Response:** `201 Created` `{ "id": "<guid>" }`

### Consultar Estrutura
- **GET** `/api/estruturas/{id}`
- **Response:**
```json
{
  "codigo": "<guid>",
  "nome": "Filial SP",
  ...
}
```

### Listar Estruturas
- **GET** `/api/estruturas`
- **Response:**
```json
[
  { "codigo": "<guid>", "nome": "Filial SP", ... },
  ...
]
```

### Atualizar Estrutura
- **PUT** `/api/estruturas/{id}`
- **Body:** igual ao POST
- **Response:** `200 OK`

### Remover Estrutura
- **DELETE** `/api/estruturas/{id}`
- **Response:** `200 OK`

---

## gRPC

### Service: EstruturasService

#### GetEstrutura
```proto
rpc GetEstrutura (GetEstruturaRequest) returns (GetEstruturaResponse);
```
- **Request:**
```json
{ "codigo": "<guid>" }
```
- **Response:**
```json
{ "estrutura": { "codigo": "<guid>", "nome": "Filial SP", ... } }
```

#### ListEstruturas
```proto
rpc ListEstruturas (ListEstruturasRequest) returns (ListEstruturasResponse);
```
- **Request:** `{}`
- **Response:**
```json
{ "estruturas": [ { "codigo": "<guid>", "nome": "Filial SP", ... }, ... ] }
```

---

## Observações
- Validações obrigatórias: Nome, EstruturaTipoCodigo, TerminoVigencia > InicioVigencia, Status válido.
- Status: 1=Ativo, 2=Inativo, 3=Edicao
- Para integração gRPC, utilize o contrato proto em `Protos/estruturas.proto`.
