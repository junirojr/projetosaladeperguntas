# Backend — Rocketq.API

## App Overview

API .NET 10 do Roquet.q. Monólito Modular com Physical Boundaries via FastEndpoints (REPR). Cada módulo é um `.csproj` auto-contido com DbContext isolado. Comunicação cross-módulo exclusivamente via IntegrationContracts.

## Tech Stack

| Camada | Tecnologia |
|---|---|
| Framework | .NET 10, C# |
| API | FastEndpoints 5.x (REPR) |
| ORM | Entity Framework Core 10 |
| Banco | PostgreSQL (Npgsql) |
| Auth | JWT Bearer (FastEndpoints.Security) |
| Hash | BCrypt.Net-Next |
| Testes | xUnit + FastEndpoints.Testing + Testcontainers |

## Architecture Pattern

REPR (Request-Endpoint-Response) com Monólito Modular:

- Um use-case = um endpoint = um slice (`Endpoint.cs` + `Models.cs` + `Data.cs` opcional)
- Módulos como bounded contexts DDD — cada um com `.csproj` e `DbContext` próprios
- Sem camada de serviço — endpoint é a unidade mínima de lógica
- Comunicação cross-módulo via `ICommand<TResult>` (FastEndpoints)

## Directory Map

```
apps/backend/
├── src/
│   ├── Rocketq.API/                         # Host fino — Program.cs, configuração, DI
│   └── Modules/
│       ├── Core/
│       │   ├── Rocketq.Identity/            # Cadastro, login, JWT, perfil
│       │   ├── Rocketq.Rooms/               # Salas, membership, entrada com senha
│       │   └── Rocketq.Feed/                # Posts, curtidas, comentários
│       └── Shared/
│           └── Rocketq.Shared/              # Primitives, IntegrationContracts
└── tests/
    └── Rocketq.Tests/                       # Integration tests via WebApplicationFactory
```

### Estrutura interna de cada módulo

```
Rocketq.{Módulo}/
├── {Agregado}/                    # Entidade raiz + comportamentos
├── Endpoints/
│   └── {Ação}/
│       ├── Endpoint.cs            # Obrigatório
│       ├── Models.cs              # Obrigatório (Request + Validator + Response)
│       └── Data.cs                # Opcional (queries complexas, classe estática)
├── CommandHandlers/               # Handlers de IntegrationContracts (se módulo produtor)
├── Persistence/
│   └── {Módulo}DbContext.cs
├── Migrations/
└── DependencyInjection.cs
```

## Key Invariants

Consultar sempre `docs/ai/PROJECT_RISK_REGISTER.md` antes de modificar. Resumo:

| INV | Regra | Onde enforçar |
|-----|-------|---------------|
| INV-001 | Usuário só acessa Feed se tiver RoomMembership | Todos os endpoints do Rocketq.Feed |
| INV-002 | Senha da sala nunca retornada pela API | Rocketq.Rooms — nunca incluir PasswordHash na response |
| INV-003 | Senha do usuário sempre com hash BCrypt | Rocketq.Identity — nunca salvar plaintext |
| INV-004 | Só o autor pode editar/deletar post ou comentário | Rocketq.Feed — verificar AuthorId == UserId do JWT |
| INV-005 | Curtida única por usuário por post | Rocketq.Feed — constraint de unicidade no banco |
| INV-006 | Módulos não referenciam .csproj de outros módulos | Estrutura do .sln — apenas Rocketq.Shared é permitido |

## Auto-documentação

Ao modificar a estrutura do backend (novos módulos, novos agregados, novos IntegrationContracts,
alterações em state machines ou invariantes), atualizar os documentos em `docs/ai/`.
Consultar tabela de auto-documentação no RFC-0001 §5.8.

Nunca alterar documentos de contexto sem antes lê-los por completo.

## Playbook Reference

- [PROJECT_PLAYBOOK](../../docs/ai/PROJECT_PLAYBOOK.md)
- [PROJECT_RISK_REGISTER](../../docs/ai/PROJECT_RISK_REGISTER.md)
