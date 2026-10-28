# Plano de Implementação — Roquet.q

| Campo      | Valor                          |
|------------|-------------------------------|
| Projeto    | Roquet.q                       |
| Status     | Em planejamento                |
| Data       | 2026-06-03                     |
| Baseado em | RFC-0001 — Monorepo Standards  |

---

## Visão Geral

**Roquet.q** é uma plataforma de salas colaborativas: usuários se cadastram, criam ou entram em salas protegidas por senha, e dentro de cada sala publicam posts, curtem e comentam em tempo real.

**Stack:**
- Backend: .NET 9, C#, FastEndpoints (REPR), Entity Framework Core, PostgreSQL
- Frontend: Next.js 14 (App Router), TypeScript, Tailwind CSS
- Auth: JWT (Bearer token)
- Monorepo: estrutura canônica RFC-0001

---

## Estrutura do Monorepo

```
rocketq/
│
├── CLAUDE.md                              ✅ (existente)
├── RFC-0001-AI-Context-Architecture.md    ✅ (existente)
│
├── .claude/
│   ├── settings.json                      🔲 criar
│   └── commands/
│       ├── scaffold-module.md             🔲 criar (fase 3)
│       └── run-migrations.md             🔲 criar (fase 3)
│
├── apps/
│   ├── backend/
│   │   ├── CLAUDE.md                      🔲 criar
│   │   ├── src/
│   │   │   ├── Rocketq.API/               🔲 criar
│   │   │   └── Modules/
│   │   │       ├── Core/
│   │   │       │   ├── Rocketq.Identity/  🔲 criar
│   │   │       │   ├── Rocketq.Rooms/     🔲 criar
│   │   │       │   └── Rocketq.Feed/      🔲 criar
│   │   │       └── Shared/
│   │   │           └── Rocketq.Shared/    🔲 criar
│   │   └── tests/
│   │       └── Rocketq.Tests/             🔲 criar
│   └── ui/
│       ├── CLAUDE.md                      🔲 criar
│       └── src/
│           └── (Next.js App Router)       🔲 criar
│
├── docs/
│   ├── ai/
│   │   ├── PROJECT_CONTEXT.md             ✅ (existente)
│   │   ├── PROJECT_PLAYBOOK.md            ✅ (existente)
│   │   ├── PROJECT_RISK_REGISTER.md       ✅ (existente)
│   │   └── PROJECT_DOMAIN_MAP.md          🔲 criar (fase 3)
│   └── architecture/
│       └── overview.md                    🔲 criar (fase 2)
│
└── platform/
    ├── iac/                               🔲 criar (fase 3)
    └── runbooks/                          🔲 criar (fase 3)
```

---

## Módulos de Domínio

Todos os módulos são **Core** — são o diferencial competitivo da plataforma.

### `Rocketq.Identity`
Responsabilidade: Cadastro, login, perfil do usuário, emissão de JWT.

```
Rocketq.Identity/
├── User/
│   ├── User.cs                    # AggregateRoot
│   ├── UserEmail.cs               # Value Object
│   └── PasswordHash.cs            # Value Object
├── Endpoints/
│   ├── Register/
│   │   ├── Endpoint.cs
│   │   └── Models.cs
│   ├── Login/
│   │   ├── Endpoint.cs
│   │   └── Models.cs
│   └── GetProfile/
│       ├── Endpoint.cs
│       └── Models.cs
├── Persistence/
│   └── IdentityDbContext.cs
├── Migrations/
├── Infrastructure/
│   └── JwtService.cs
└── DependencyInjection.cs
```

### `Rocketq.Rooms`
Responsabilidade: Criação e listagem de salas, gestão de RoomMembership (entrada com senha).

```
Rocketq.Rooms/
├── Room/
│   ├── Room.cs                    # AggregateRoot
│   └── RoomPasswordHash.cs        # Value Object
├── RoomMembership/
│   └── RoomMembership.cs          # Entity
├── Endpoints/
│   ├── CreateRoom/
│   │   ├── Endpoint.cs
│   │   └── Models.cs
│   ├── ListMyRooms/
│   │   ├── Endpoint.cs
│   │   └── Models.cs
│   ├── JoinRoom/                  # INV-001 + INV-002 — crítico
│   │   ├── Endpoint.cs
│   │   └── Models.cs
│   └── LeaveRoom/
│       ├── Endpoint.cs
│       └── Models.cs
├── CommandHandlers/
│   └── CheckMembershipHandler.cs  # ICommand → consumido pelo Feed
├── Persistence/
│   └── RoomsDbContext.cs
├── Migrations/
└── DependencyInjection.cs
```

### `Rocketq.Feed`
Responsabilidade: Posts, curtidas e comentários dentro de uma sala. Todo endpoint verifica INV-001.

```
Rocketq.Feed/
├── Post/
│   ├── Post.cs                    # AggregateRoot
│   └── Like.cs                    # Entity (INV-005: unicidade)
├── Comment/
│   └── Comment.cs                 # AggregateRoot
├── Endpoints/
│   ├── CreatePost/
│   │   ├── Endpoint.cs
│   │   └── Models.cs
│   ├── ListPosts/
│   │   ├── Endpoint.cs
│   │   ├── Models.cs
│   │   └── Data.cs
│   ├── LikePost/                  # INV-005
│   │   ├── Endpoint.cs
│   │   └── Models.cs
│   ├── UnlikePost/
│   │   ├── Endpoint.cs
│   │   └── Models.cs
│   ├── AddComment/
│   │   ├── Endpoint.cs
│   │   └── Models.cs
│   ├── ListComments/
│   │   ├── Endpoint.cs
│   │   ├── Models.cs
│   │   └── Data.cs
│   ├── DeletePost/                # INV-004: só autor
│   │   ├── Endpoint.cs
│   │   └── Models.cs
│   └── DeleteComment/             # INV-004: só autor
│       ├── Endpoint.cs
│       └── Models.cs
├── Persistence/
│   └── FeedDbContext.cs
├── Migrations/
└── DependencyInjection.cs
```

### `Rocketq.Shared`
Responsabilidade: Primitivos de domínio, Authorization e IntegrationContracts.

```
Rocketq.Shared/
├── Primitives/
│   ├── Entity.cs
│   ├── AggregateRoot.cs
│   └── AuditableEntity.cs
├── Authorization/
│   ├── Roles.cs
│   └── Permissions.cs
└── IntegrationContracts/
    ├── Identity/
    │   └── GetUserQuery.cs         # Rooms e Feed consultam dados de usuário
    └── Rooms/
        └── CheckMembershipQuery.cs # Feed verifica se usuário é membro da sala
```

---

## IntegrationContracts — Fluxo Cross-Módulo

```
Feed.ListPosts.Endpoint
  └── await new CheckMembershipQuery { UserId, RoomId }.ExecuteAsync(ct)
        └── CheckMembershipHandler (em Rocketq.Rooms)
              └── acessa RoomsDbContext → retorna bool isMember
              
Feed.CreatePost.Endpoint
  └── mesma verificação de membership (INV-001)
```

**Regra:** `Data.cs` nunca chama IntegrationContracts — apenas `Endpoint.cs`.

---

## Frontend — Páginas (Next.js App Router)

| Rota                    | Componente          | Auth requerida | Endpoints usados                  |
|-------------------------|---------------------|----------------|-----------------------------------|
| `/`                     | Login / Cadastro    | Não            | POST /auth/register, POST /auth/login |
| `/rooms`                | Minhas Salas        | Sim            | GET /rooms                        |
| `/rooms/new`            | Criar Sala          | Sim            | POST /rooms                       |
| `/rooms/[id]`           | Feed da Sala        | Sim (+ member) | GET /rooms/{id}/posts             |
| `/rooms/[id]/join`      | Entrar na Sala      | Sim            | POST /rooms/{id}/join             |
| `/rooms/[id]/posts/new` | Criar Post          | Sim (+ member) | POST /rooms/{id}/posts            |
| `/rooms/[id]/posts/[postId]` | Post + Comentários | Sim (+ member) | GET /posts/{id}/comments     |
| `/profile`              | Perfil do Usuário   | Sim            | GET /users/me                     |

---

## Fases de Implementação

### Fase 1 — Foundation (Semana 1) ✅ CONCLUÍDA

Objetivo: Monorepo estruturado, contexto AI completo, backend compilando.

**Tasks:**

- [x] Inicializar repositório Git com estrutura canônica RFC-0001
- [x] Criar `.claude/settings.json` com plugins `bnp-code` e `bnp-gitops`
- [x] Criar `apps/backend/CLAUDE.md` com contexto do backend
- [x] Criar `apps/ui/CLAUDE.md` com contexto do frontend
- [x] Criar `docs/ai/PROJECT_DOMAIN_MAP.md` com bounded contexts iniciais
- [x] Scaffoldar solution .NET: `Rocketq.sln` com todos os `.csproj`
- [x] Configurar `Rocketq.Shared` (Primitives, Authorization)
- [x] Scaffoldar `Rocketq.API` (Program.cs, middleware, DI)
- [x] Configurar PostgreSQL + EF Core (connection string via env)
- [x] Configurar JWT (issuer, audience, secret via env)
- [x] Criar projeto `Rocketq.Tests` com `WebApplicationFactory`

**Entregável:** Estrutura compilando. ✅

---

### Fase 2 — Identity Module (Semana 1–2) 🔄 EM ANDAMENTO

Objetivo: Usuário se cadastra, faz login e recebe JWT.

**Tasks:**

- [x] Criar entidade `User` com `VerifyPassword` + `UserConfiguration` (EF Core, índice único em Email)
- [x] Criar `IdentityDbContext` + migration `Initial` (schema `identity`, tabela `Users`)
- [x] Criar `JwtService` em `Infrastructure/` — geração de token com claims userId/email/username
- [x] Endpoint `POST /auth/register` — criar usuário com senha hasheada (INV-003)
- [x] Endpoint `POST /auth/login` — validar credenciais, emitir JWT
- [x] Endpoint `GET /users/me` — retornar perfil do usuário autenticado
- [x] Atualizar `DependencyInjection.cs` — registrar `JwtService`
- [x] Atualizar `AppFactory.cs` — aplicar migrations automaticamente antes dos testes
- [x] Migrar projeto de testes de `xunit` v2 → `xunit.v3` v1 (compatível com FastEndpoints.Testing 5.35)
- [ ] Finalizar `IdentityTests.cs` — testes de integração (em correção de build)
- [ ] Frontend: página `/` com formulário de login e cadastro

**Bloqueio atual:** Arquivo de testes com erro de compilação — `TestBase<>` requer encadeamento explícito do construtor base e aliases de tipo para os endpoints.

**Invariantes a garantir:** INV-003 (senha sempre hasheada, nunca plaintext).

---

### Fase 3 — Rooms Module (Semana 2)

Objetivo: Usuário cria salas e entra em salas com senha.

**Tasks:**

- [ ] Criar entidade `Room` com `RoomPasswordHash` (BCrypt)
- [ ] Criar entidade `RoomMembership { UserId, RoomId, JoinedAt }`
- [ ] Criar `RoomsDbContext` + migrations
- [ ] IntegrationContract `CheckMembershipQuery` em `Rocketq.Shared`
- [ ] Handler `CheckMembershipHandler` em `Rocketq.Rooms`
- [ ] Endpoint `POST /rooms` — criar sala com nome + senha hasheada
- [ ] Endpoint `GET /rooms` — listar salas onde o usuário é membro
- [ ] Endpoint `POST /rooms/{id}/join` — validar senha + criar Membership (INV-001, INV-002)
- [ ] Endpoint `DELETE /rooms/{id}/membership` — sair da sala
- [ ] Testes de integração: criar sala, entrar com senha errada, entrar com senha correta, sair
- [ ] Frontend: `/rooms` (lista), `/rooms/new` (criar), `/rooms/[id]/join` (entrar)

**Invariantes a garantir:** INV-001, INV-002 (senha nunca retornada).

---

### Fase 4 — Feed Module (Semana 3)

Objetivo: Usuário publica posts, curte e comenta dentro de uma sala.

**Tasks:**

- [ ] Criar entidade `Post { Id, RoomId, AuthorId, Content, CreatedAt }`
- [ ] Criar entidade `Like { PostId, UserId }` com constraint de unicidade (INV-005)
- [ ] Criar entidade `Comment { Id, PostId, AuthorId, Content, CreatedAt }`
- [ ] Criar `FeedDbContext` + migrations
- [ ] Endpoint `GET /rooms/{id}/posts` — listar posts (verifica INV-001 via CheckMembershipQuery)
- [ ] Endpoint `POST /rooms/{id}/posts` — criar post (verifica membership)
- [ ] Endpoint `DELETE /posts/{id}` — deletar post (INV-004: só autor)
- [ ] Endpoint `POST /posts/{id}/likes` — curtir (INV-005: unicidade)
- [ ] Endpoint `DELETE /posts/{id}/likes` — descurtir
- [ ] Endpoint `GET /posts/{id}/comments` — listar comentários
- [ ] Endpoint `POST /posts/{id}/comments` — adicionar comentário
- [ ] Endpoint `DELETE /comments/{id}` — deletar comentário (INV-004: só autor)
- [ ] Testes de integração: post sem membership, curtida duplicada, delete de post alheio
- [ ] Frontend: `/rooms/[id]` (feed), `/rooms/[id]/posts/new`, `/rooms/[id]/posts/[postId]`

**Invariantes a garantir:** INV-001, INV-004, INV-005.

---

### Fase 5 — Polish & Infraestrutura (Semana 4)

Objetivo: Aplicação production-ready com CI/CD básico.

**Tasks:**

- [ ] Frontend: página `/profile` (perfil do usuário)
- [ ] Frontend: loading states, error boundaries, feedback visual
- [ ] Configurar CORS (allowlist explícita — nunca `AllowAnyOrigin()`)
- [ ] Criar `Dockerfile` para `Rocketq.API`
- [ ] Criar `Dockerfile` para `apps/ui`
- [ ] Criar `docker-compose.yml` para desenvolvimento local (API + UI + PostgreSQL)
- [ ] Criar `platform/runbooks/` com runbook de deploy e rollback
- [ ] Criar `docs/architecture/overview.md`
- [ ] Configurar CI: `dotnet build` + `dotnet test` + `docker build`
- [ ] Criar `.claude/commands/scaffold-module.md` e `run-migrations.md`

---

## Invariantes Críticas — Resumo Rápido

| ID      | Regra                                                        | Módulos afetados         |
|---------|--------------------------------------------------------------|--------------------------|
| INV-001 | Membership gate em todo endpoint do Feed                     | Rocketq.Feed             |
| INV-002 | Senha da sala nunca retornada pela API                       | Rocketq.Rooms            |
| INV-003 | Senha do usuário sempre hasheada (BCrypt/Argon2)             | Rocketq.Identity         |
| INV-004 | Só o autor pode editar/deletar seu post ou comentário        | Rocketq.Feed             |
| INV-005 | Curtida única por usuário por post                           | Rocketq.Feed             |
| INV-006 | Módulos nunca referenciam `.csproj` de outros módulos        | Todos                    |

---

## Decisões Técnicas

| Decisão | Escolha | Justificativa |
|---------|---------|---------------|
| ORM | Entity Framework Core 9 | Integração nativa com .NET, migrations gerenciadas |
| Banco de dados | PostgreSQL | Robusto, suporte a JSONB futuro, open source |
| Hash de senha | BCrypt | Padrão seguro, já presente no ecossistema .NET |
| API framework | FastEndpoints + REPR | Adotado no RFC-0001, CQRS natural por endpoint |
| Frontend routing | Next.js App Router | Server Components por padrão, SEO, performance |
| Autenticação frontend | Cookie httpOnly (JWT) | Mais seguro que localStorage |
| Estilo | Tailwind CSS | Design system flexível, baixo bundle |

---

## Referências

- [RFC-0001](RFC-0001-AI-Context-Architecture.md)
- [PROJECT_CONTEXT](docs/ai/PROJECT_CONTEXT.md)
- [PROJECT_PLAYBOOK](docs/ai/PROJECT_PLAYBOOK.md)
- [PROJECT_RISK_REGISTER](docs/ai/PROJECT_RISK_REGISTER.md)
- [Figma](https://www.figma.com/design/M8ftRZzitZEir0ATcmKlgm/Roquet.q--Community-?node-id=37-802)
