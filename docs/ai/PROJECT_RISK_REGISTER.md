# PROJECT_RISK_REGISTER — Roquet.q

> Documento de governança. Define o que a IA NÃO pode modificar sem revisão humana e quais invariantes nunca devem ser violadas.
> Atualizar ao adicionar/alterar invariantes de negócio ou state machines (RFC-0001 §5.8).

---

## Critical Invariants

### INV-001: Membership Gate
```
Um usuário só pode ler ou escrever em uma sala se existir um registro ativo
de RoomMembership { UserId, RoomId } no banco de dados.

Violation = exposição de conteúdo privado.
```
**Enforcement:** Todo endpoint do módulo `Rocketq.Feed` DEVE verificar membership antes de qualquer query. Nunca remover essa verificação.

---

### INV-002: Senha da Sala
```
A senha de uma sala é armazenada com hash (BCrypt).
A API NUNCA retorna o valor da senha em nenhuma resposta.
A validação ocorre apenas no momento do JOIN — nunca em listagens.

Violation = vazamento de credencial de sala.
```

---

### INV-003: Senha do Usuário
```
Senhas de usuário são sempre armazenadas com hash (BCrypt ou Argon2).
Nunca armazenar, logar ou retornar senha em plaintext.

Violation = comprometimento de credenciais.
```

---

### INV-004: Ownership de Post/Comentário
```
Apenas o autor (UserId == post.AuthorId) pode editar ou deletar
seu próprio post ou comentário.

Violation = escalada de privilégio.
```

---

### INV-005: Unicidade de Curtida
```
Um usuário só pode ter uma curtida ativa por post.
Tentar curtir novamente deve retornar erro de validação, não criar duplicata.

Violation = dados inconsistentes de engajamento.
```

---

### INV-006: Fronteira de Módulo
```
Um .csproj de módulo NUNCA referencia outro .csproj de módulo.
A única dependência cross-módulo permitida é Rocketq.Shared.
Comunicação entre módulos exclusivamente via IntegrationContracts.

Violation = acoplamento que impede extração para microserviços.
```

---

## High-Impact Modules

Módulos que exigem revisão humana antes de qualquer alteração:

| Módulo / Área | Risco | Ação Requerida |
|---|---|---|
| `Rocketq.Identity` (auth flow) | Comprometimento de contas | Revisão humana obrigatória |
| `Rocketq.Shared/IntegrationContracts/` | Breaking change cross-módulo | Revisão de todos os consumidores |
| `Rocketq.Rooms` (RoomMembership) | Acesso indevido a salas | Revisão humana obrigatória |
| Migrations de banco de dados | Irreversível em produção | Revisão humana + backup confirmado |
| JWT configuration (secret, expiry) | Sessões comprometidas | Revisão humana obrigatória |
| `CLAUDE.md` raiz / `docs/ai/` | Contexto da IA corrompido | Revisão do Engineering Lead |

---

## State Machine Rules

### Room Join Flow
```
Estados possíveis de uma sala para o usuário:
  NOT_MEMBER → PENDING_JOIN → MEMBER → LEFT

Transições válidas:
  NOT_MEMBER  → MEMBER       (via POST /rooms/{id}/join com senha correta)
  MEMBER      → LEFT         (via DELETE /rooms/{id}/membership)
  LEFT        → MEMBER       (re-entrada com senha)

Transições PROIBIDAS:
  NOT_MEMBER → LEFT          (não pode sair sem ter entrado)
  PENDING_JOIN → LEFT        (não há estado intermediário — join é atômico)
```

---

## Dependency Risks

| Dependência | Risco | Mitigação |
|---|---|---|
| FastEndpoints | Breaking changes em major versions | Fixar versão no `.csproj`; atualizar com testes de integração completos |
| Entity Framework Core + Migrations | Migration inválida corrompe schema | Sempre revisar SQL gerado antes de aplicar em staging/prod |
| JWT (Microsoft.AspNetCore.Authentication.JwtBearer) | Configuração incorreta expõe endpoints | Testar com token expirado, token inválido e ausência de token |
| Next.js App Router | Mudanças de API entre minor versions | Testar rotas protegidas após atualizações de dependência |

---

## Technical Debt

> Preencher conforme débitos forem identificados durante o desenvolvimento.

| ID | Área | Descrição | Impacto | Prioridade |
|---|---|---|---|---|
| TD-001 | — | — | — | — |

---

## Security Considerations

| Área | Regra | Nunca Fazer |
|---|---|---|
| Autenticação | JWT com expiração curta (15-60min) + refresh token | JWT sem expiração ou com secret hardcoded |
| Autorização | Verificar membership em CADA request ao Feed | Cache de membership sem invalidação |
| Dados sensíveis | Secrets via env vars / Key Vault | Connection strings ou JWT secret em código |
| Inputs do usuário | Validar via FluentValidation no `Models.cs` | Confiar em input sem validação server-side |
| Logs | Logar erros e eventos de negócio | Logar senhas, tokens ou dados pessoais |
| CORS | Configurar allowlist explícita de origens | `AllowAnyOrigin()` em produção |
