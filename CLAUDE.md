# Roquet.q — Sala de Perguntas

## O que é este repositório

Monorepo do sistema **Roquet.q**: plataforma de salas colaborativas onde usuários criam e entram em salas protegidas por senha, publicam posts, curtem e comentam em tempo real. O backend é uma API .NET com Monólito Modular (FastEndpoints + REPR), o frontend é Next.js + Tailwind.

## Estrutura do Monorepo

- `apps/backend/` — API .NET com módulos DDD auto-contidos → [contexto](apps/backend/CLAUDE.md)
- `apps/ui/` — Frontend Next.js + Tailwind → [contexto](apps/ui/CLAUDE.md)
- `platform/` — IaC (Terraform), runbooks operacionais
- `docs/ai/` — Documentação de contexto estruturada para IA

## Context Loading

| Tipo de tarefa | Documentos a carregar |
|---|---|
| Modificar/criar código backend | `apps/backend/CLAUDE.md` + `docs/ai/PROJECT_PLAYBOOK.md` + `docs/ai/PROJECT_RISK_REGISTER.md` |
| Modificar/criar código frontend | `apps/ui/CLAUDE.md` + `docs/ai/PROJECT_PLAYBOOK.md` |
| Modelagem de domínio | `docs/ai/PROJECT_CONTEXT.md` + `docs/ai/PROJECT_DOMAIN_MAP.md` |
| Deploy / operações | `platform/runbooks/` |
| Qualquer alteração | `docs/ai/PROJECT_RISK_REGISTER.md` SEMPRE antes de modificar |

**Regra:** Nunca carregar todos os documentos ao mesmo tempo. Carregar apenas o contexto relevante para a tarefa atual.

## Risk Awareness

Módulos e áreas que exigem revisão humana antes de qualquer modificação:

- **Autenticação e autorização** — JWT, hashing de senha, controle de acesso a salas
- **RoomMembership** — lógica de entrada/saída de salas com senha
- **Módulo Shared / IntegrationContracts** — contratos cross-módulo são breaking changes
- **Migrations de banco de dados** — irreversíveis em produção

## Security Reminders

- Senhas de sala NUNCA devem ser retornadas em respostas de API — apenas validadas server-side
- Senhas de usuário SEMPRE com hash (BCrypt ou Argon2) — nunca plaintext
- Posts e comentários pertencem ao usuário autenticado — validar ownership antes de editar/deletar
- Usuário só acessa sala se for membro — verificar RoomMembership em todo endpoint de sala
- Segredos (connection strings, JWT secret) NUNCA em código — apenas em variáveis de ambiente ou Key Vault

## References

- [PROJECT_CONTEXT](docs/ai/PROJECT_CONTEXT.md)
- [PROJECT_PLAYBOOK](docs/ai/PROJECT_PLAYBOOK.md)
- [PROJECT_RISK_REGISTER](docs/ai/PROJECT_RISK_REGISTER.md)
- [RFC-0001](RFC-0001-AI-Context-Architecture.md)
