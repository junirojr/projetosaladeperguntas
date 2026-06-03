# PROJECT_PLAYBOOK — Roquet.q

> Guias de execução para tarefas comuns. Presença obrigatória desde o scaffold; conteúdo evolutivo.
> Adicionar novos playbooks conforme o time identifica padrões recorrentes (RFC-0001 §5.6).

---

## Playbook: Criar Novo Módulo

### Pré-condições
- [ ] Classificação definida pelo produto (Core / Supporting / Generic)
- [ ] Nome do módulo segue convenção `Rocketq.{NomeDoMódulo}`

### Steps
1. Criar `.csproj` em `apps/backend/src/Modules/{Core|Supporting|Generic}/Rocketq.{Módulo}/`
2. Adicionar referência **apenas** a `Rocketq.Shared.csproj`
3. Criar `Persistence/{Módulo}DbContext.cs` exclusivo do módulo
4. Criar `DependencyInjection.cs` para registro de serviços
5. Registrar o módulo no `Program.cs` do host (`Rocketq.API`)
6. Criar pasta do primeiro agregado com entidade raiz
7. Criar primeiro endpoint em `Endpoints/`

### Checklist Pós-Execução
- [ ] `.csproj` referencia APENAS `Rocketq.Shared`
- [ ] `DbContext` é exclusivo do módulo — não acessa tabelas de outros módulos
- [ ] `DependencyInjection.cs` registra apenas serviços do módulo
- [ ] `docs/ai/PROJECT_CONTEXT.md` atualizado (Module Map)
- [ ] `apps/backend/CLAUDE.md` atualizado (Directory Map)

---

## Playbook: Adicionar Endpoint

### Pré-condições
- [ ] `apps/backend/CLAUDE.md` carregado
- [ ] `docs/ai/PROJECT_RISK_REGISTER.md` verificado para o módulo alvo

### Steps
1. Identificar o módulo correto em `Modules/{Core|Supporting|Generic}/`
2. Criar pasta em `Endpoints/{NomeDaAção}/` dentro do módulo
3. Criar `Models.cs` com: `Request` (com atributos de binding), `Validator` (FluentValidation inline) e `Response`
4. Criar `Endpoint.cs` com rota, método HTTP, autorização e `HandleAsync`
5. Se a query for complexa, extrair para `Data.cs` (classe estática — acessa apenas DbContext do módulo)
6. Se o endpoint precisa de dados de outro módulo, chamar `IntegrationContract` no `HandleAsync` (nunca no `Data.cs`)
7. Rodar testes de integração (`apps/backend/tests/Rocketq.Tests/`)

### Checklist Pós-Execução
- [ ] `Data.cs` não referencia `IntegrationContracts`
- [ ] Autorização configurada no endpoint (não deixar endpoint público por acidente)
- [ ] Nenhuma invariante do `PROJECT_RISK_REGISTER.md` violada
- [ ] Testes de integração passando
- [ ] Commit segue Conventional Commits (`feat:`, `fix:`, etc.)

---

## Playbook: Entrar em uma Sala (Join Room)

### Contexto
O fluxo de entrada em sala é crítico — ver INV-001 e INV-002 no Risk Register.

### Steps
1. Receber `POST /rooms/{roomId}/join` com `{ password: string }` no body
2. Buscar a sala pelo `roomId` — retornar 404 se não existir
3. Validar senha: comparar `password` com hash armazenado via BCrypt — retornar 401 se inválida
4. Verificar se já existe `RoomMembership` ativa — retornar 409 se já for membro
5. Criar registro de `RoomMembership { UserId, RoomId, JoinedAt }`
6. Retornar 200 com dados básicos da sala (SEM a senha)

### Checklist de Segurança
- [ ] Senha nunca retornada na resposta
- [ ] Hash comparado com BCrypt (nunca comparação direta de string)
- [ ] UserId extraído do JWT — não aceitar UserId do body

---

## Playbook: Adicionar IntegrationContract

### Quando usar
Quando um módulo precisar disparar uma ação ou consultar dados de outro módulo.

### Steps
1. Criar o contrato em `apps/backend/src/Modules/Shared/Rocketq.Shared/IntegrationContracts/{MóduloProdutor}/`
2. Naming: `{Verbo}{Entidade}Command.cs` para ações, `Get{Entidade}Query.cs` para consultas
3. Implementar o handler `ICommandHandler<TCommand, TResult>` no módulo produtor em `CommandHandlers/`
4. Consumir via `await new {Command} { ... }.ExecuteAsync(ct)` no `HandleAsync` do endpoint consumidor

### Checklist Pós-Execução
- [ ] Contrato mora em `Shared/IntegrationContracts/{MóduloProdutor}/`
- [ ] Handler mora no módulo produtor (dono do contrato)
- [ ] `docs/ai/PROJECT_CONTEXT.md` atualizado se novo contexto de integração emergiu
- [ ] Revisão humana solicitada (IntegrationContracts são breaking changes)

---

## Playbook: Adicionar Migration de Banco

### Steps
1. Garantir que o `DbContext` correto está configurado
2. Rodar: `dotnet ef migrations add {NomeDaMigration} --project Rocketq.{Módulo} --startup-project Rocketq.API`
3. Revisar o SQL gerado com `dotnet ef migrations script`
4. Testar em banco de dev antes de commitar
5. Nunca aplicar migration em produção sem backup confirmado

### Checklist
- [ ] SQL gerado revisado — sem `DROP TABLE` ou `DROP COLUMN` acidental
- [ ] Migration testada em dev
- [ ] Nome da migration descreve a mudança (`AddRoomMembershipTable`, não `Migration1`)

---

## Playbook: Debug de Falha em Endpoint

### Steps
1. Verificar logs de request/response (middleware de logging)
2. Checar se o problema é validação (`400`), autenticação (`401`), autorização (`403`) ou lógica (`500`)
3. Para `500`: verificar stack trace nos logs — identificar se é no `HandleAsync`, `Data.cs` ou no `DbContext`
4. Reproduzir com teste de integração em `Rocketq.Tests/`
5. Verificar se a falha viola alguma invariante do `PROJECT_RISK_REGISTER.md`

---

## Playbook: Criar Página no Frontend (Next.js)

### Pré-condições
- [ ] Endpoint de backend correspondente já existe e está testado
- [ ] `apps/ui/CLAUDE.md` carregado

### Steps
1. Criar rota em `apps/ui/src/app/{caminho}/page.tsx`
2. Se a página requer autenticação, usar o middleware de proteção de rota
3. Criar Client Component em `components/` apenas se houver interatividade
4. Usar Server Components por padrão para fetching de dados
5. Estilizar com Tailwind — seguir design system definido (ver Figma no `Criando uma aplicação em C# com Next.JS e Tailwind.md`)
6. Testar com usuário autenticado E não autenticado

### Checklist
- [ ] Rota protegida se exige autenticação
- [ ] Nenhum dado sensível (tokens, senhas) exposto no client bundle
- [ ] Loading state e erro tratados na UI
