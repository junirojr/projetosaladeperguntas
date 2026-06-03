# PROJECT_DOMAIN_MAP — Roquet.q

> Documento de contexto estruturado para IA. Não misturar com documentação para humanos.
> Atualizar ao criar novos módulos, agregados, IntegrationContracts ou eventos de domínio (ver RFC-0001 §5.8).

## Ubiquitous Language

Glossário canônico do domínio. Ao gerar código, usar EXATAMENTE estes termos — nunca sinônimos ou traduções.

| Termo | Definição | Módulo |
|---|---|---|
| **User** | Pessoa registrada na plataforma. Tem email único e senha com hash. | Identity |
| **Room** | Sala colaborativa criada por um User. Protegida por senha. Tem nome e descrição. | Rooms |
| **RoomPassword** | Senha da sala. Nunca retornada pela API — apenas validada no momento de entrada. | Rooms |
| **RoomMembership** | Vínculo ativo entre um User e uma Room. Pré-requisito para acessar Feed. | Rooms |
| **Post** | Publicação de um membro dentro de uma Room. Tem texto e pertence a um único Author. | Feed |
| **Comment** | Resposta de um membro a um Post. Pertence a um único Author. | Feed |
| **Like** | Curtida de um User em um Post. Única por User por Post (unicidade enforçada no banco). | Feed |
| **Author** | User que criou um Post ou Comment. Único com permissão de editar/deletar o conteúdo. | Feed |
| **JWT** | Token Bearer emitido no login. Identifica o User autenticado em todas as requisições. | Identity |
| **Member** | User com RoomMembership ativa em uma Room. Pode ver e publicar no Feed. | Rooms |

---

## Bounded Contexts

| Bounded Context | Módulo .csproj | Responsabilidade |
|---|---|---|
| **Identity** | `Rocketq.Identity` | Cadastro de usuários, login, geração de JWT, perfil |
| **Rooms** | `Rocketq.Rooms` | Criação e listagem de salas, controle de RoomMembership, validação de senha |
| **Feed** | `Rocketq.Feed` | Posts, curtidas e comentários dentro de uma sala (exige RoomMembership) |
| **Shared** | `Rocketq.Shared` | Primitives, Authorization, IntegrationContracts (Shared Kernel — não é bounded context de negócio) |

---

## Context Relationships

Relacionamentos upstream → downstream. O upstream define o contrato; o downstream consome.

```
Identity (upstream)
    ↓ GetUserQuery
  Feed (downstream) — precisa do UserId para validar authorship
  Rooms (downstream) — precisa do UserId para criar sala e registrar membership

Rooms (upstream)
    ↓ GetRoomMembershipQuery
  Feed (downstream) — verifica se o User é membro antes de expor posts/comentários
```

| IntegrationContract | Produtor | Consumidor | Localização |
|---|---|---|---|
| `GetUserQuery` | Identity | Feed, Rooms | `Shared/IntegrationContracts/Identity/` |
| `GetRoomMembershipQuery` | Rooms | Feed | `Shared/IntegrationContracts/Rooms/` |

> Ao criar novos IntegrationContracts, adicionar entrada nesta tabela e na seção de Contexto do `PROJECT_CONTEXT.md`.

---

## Aggregates

Principais agregados e suas regras de consistência.

### Identity — Agregado `User`

| Campo | Tipo | Invariante |
|---|---|---|
| Id | Guid | Imutável após criação |
| Email | string | Único no sistema — validar antes de persistir |
| PasswordHash | string | NUNCA plaintext. Hash BCrypt obrigatório |
| DisplayName | string | — |

**Comportamentos:**
- `Register(email, password)` — valida unicidade do email, aplica hash na senha
- `Login(email, password)` — valida credenciais, emite JWT

---

### Rooms — Agregado `Room`

| Campo | Tipo | Invariante |
|---|---|---|
| Id | Guid | Imutável após criação |
| Name | string | — |
| Description | string | — |
| PasswordHash | string | NUNCA retornar em response da API |
| OwnerId | Guid | UserId do criador |

**Comportamentos:**
- `Create(name, description, password, ownerId)` — aplica hash na senha
- `ValidatePassword(plainPassword)` — compara com hash; retorna bool

---

### Rooms — Agregado `RoomMembership`

| Campo | Tipo | Invariante |
|---|---|---|
| Id | Guid | Imutável |
| RoomId | Guid | FK para Room |
| UserId | Guid | FK para User (via IntegrationContract) |
| JoinedAt | DateTime | Imutável após criação |

**Comportamentos:**
- `Join(roomId, userId, providedPassword)` — valida senha, cria membership se não existir
- Membership é idempotente: entrar numa sala onde já é membro não cria duplicata

---

### Feed — Agregado `Post`

| Campo | Tipo | Invariante |
|---|---|---|
| Id | Guid | Imutável |
| RoomId | Guid | Post pertence a uma sala |
| AuthorId | Guid | UserId do autor |
| Content | string | — |
| CreatedAt | DateTime | Imutável |

**Comportamentos:**
- `Publish(roomId, authorId, content)` — cria post; exige que authorId seja membro da sala
- `Edit(requesterId, newContent)` — só permite se `requesterId == AuthorId`
- `Delete(requesterId)` — só permite se `requesterId == AuthorId`

---

### Feed — Entidade `Comment`

| Campo | Tipo | Invariante |
|---|---|---|
| Id | Guid | Imutável |
| PostId | Guid | FK para Post |
| AuthorId | Guid | UserId do autor |
| Content | string | — |

**Comportamentos:**
- `Add(postId, authorId, content)` — cria comentário
- `Edit(requesterId, newContent)` — só permite se `requesterId == AuthorId`
- `Delete(requesterId)` — só permite se `requesterId == AuthorId`

---

### Feed — Entidade `Like`

| Campo | Tipo | Invariante |
|---|---|---|
| PostId | Guid | Parte da chave composta |
| UserId | Guid | Parte da chave composta |

**Invariante:** Unicidade composta `(PostId, UserId)` — um user não pode curtir o mesmo post duas vezes. Enforçado por constraint no banco.

---

## Domain Events

Eventos publicados e consumidos por cada contexto. Atualmente todos in-process via FastEndpoints event notifications.

| Evento | Publicado por | Consumido por | Quando |
|---|---|---|---|
| `UserRegistered` | Identity | — | Após cadastro bem-sucedido |
| `RoomCreated` | Rooms | — | Após criação de sala |
| `RoomJoined` | Rooms | — | Após RoomMembership criada com sucesso |
| `PostPublished` | Feed | — | Após publicação de post |
| `CommentAdded` | Feed | — | Após adição de comentário |

> Ao adicionar novos eventos de domínio, incluir nesta tabela.
