# PROJECT_CONTEXT — Roquet.q

> Documento de contexto estruturado para IA. Não misturar com documentação para humanos.
> Atualizar conforme o código evolui (ver tabela de auto-documentação no RFC-0001 §5.8).

## System Overview

**Roquet.q** é uma plataforma de salas colaborativas. Usuários se cadastram, criam ou entram em salas protegidas por senha, e dentro de cada sala publicam posts, curtem e comentam. O acesso a uma sala é controlado por membership — um usuário só vê o conteúdo da sala se for membro.

## Tech Stack

| Camada | Tecnologia |
|---|---|
| Backend | .NET 10, C#, FastEndpoints, Entity Framework Core 10 |
| Frontend | Next.js 14 (App Router), Tailwind CSS, TypeScript |
| Banco de dados | A definir (PostgreSQL recomendado) |
| Autenticação | JWT (Bearer token) |
| Cloud / Infra | A definir |

## Architecture Pattern

- **Monólito Modular com Physical Boundaries** — cada módulo é um `.csproj` auto-contido
- **REPR (Request-Endpoint-Response)** via FastEndpoints — um use-case = um endpoint
- **DDD** — módulos como bounded contexts, agregados com invariantes explícitas
- **Monorepo** — `apps/backend/` (API) + `apps/ui/` (Next.js)
- **Comunicação cross-módulo** exclusivamente via `IntegrationContracts` (`ICommand<TResult>`)

## Key Invariants

1. Um usuário só pode acessar posts e comentários de uma sala se tiver `RoomMembership` ativa nessa sala
2. A senha da sala é validada no momento de entrada e NUNCA devolvida pela API
3. Um post ou comentário só pode ser editado/deletado pelo próprio autor
4. Curtidas são únicas por usuário por post — não é possível curtir o mesmo post duas vezes
5. Um módulo NUNCA referencia o `.csproj` de outro módulo — apenas `Rocketq.Shared`

## Module Map

> Atualizar ao criar novos módulos (ver Playbook: Criar Novo Módulo).

| Módulo | Tipo | Responsabilidade |
|---|---|---|
| `Rocketq.Identity` | Core | Cadastro, login, perfil do usuário, JWT |
| `Rocketq.Rooms` | Core | Criação, listagem e gestão de salas; RoomMembership (entrada com senha) |
| `Rocketq.Feed` | Core | Posts, curtidas e comentários dentro de uma sala |
| `Rocketq.Shared` | Shared | Primitives, Authorization, IntegrationContracts |

## Navigation References

- [PROJECT_PLAYBOOK](PROJECT_PLAYBOOK.md) — como executar tarefas comuns com segurança
- [PROJECT_RISK_REGISTER](PROJECT_RISK_REGISTER.md) — invariantes críticas e módulos de alto risco
- [RFC-0001](../../RFC-0001-AI-Context-Architecture.md) — padrões de estrutura do monorepo
