# Frontend — Rocketq UI

## App Overview

Frontend do Roquet.q em Next.js 14 (App Router) + TypeScript + Tailwind CSS. Server Components por padrão, Client Components apenas onde há interatividade real. Autenticação via JWT armazenado em cookie httpOnly.

## Tech Stack

| Camada | Tecnologia |
|---|---|
| Framework | Next.js 14 (App Router) |
| Linguagem | TypeScript strict |
| Estilo | Tailwind CSS 3.x |
| Auth | JWT em cookie httpOnly |
| Fetch | `src/lib/api.ts` (wrapper sobre fetch nativo) |

## Directory Map

```
apps/ui/
├── src/
│   ├── app/                       # Rotas (App Router)
│   │   ├── layout.tsx             # Layout raiz
│   │   ├── page.tsx               # Página raiz (login/cadastro)
│   │   ├── rooms/
│   │   │   ├── page.tsx           # Lista de salas do usuário
│   │   │   ├── new/page.tsx       # Criar sala
│   │   │   └── [id]/
│   │   │       ├── page.tsx       # Feed da sala
│   │   │       ├── join/page.tsx  # Entrar na sala
│   │   │       └── posts/
│   │   │           ├── new/page.tsx
│   │   │           └── [postId]/page.tsx
│   │   └── profile/page.tsx
│   ├── components/                # Client Components reutilizáveis
│   └── lib/
│       └── api.ts                 # Fetch helper com auth
```

## Key Invariants

- Rotas autenticadas DEVEM verificar o token antes de renderizar — redirecionar para `/` se ausente
- Nenhum dado sensível (tokens, hashes) deve aparecer no client bundle ou em logs do browser
- Server Components fazem o fetch de dados por padrão — Client Component apenas para interatividade
- Senha de sala e senha de usuário NUNCA são exibidas ou armazenadas no estado do cliente

## Playbook Reference

- [PROJECT_PLAYBOOK](../../docs/ai/PROJECT_PLAYBOOK.md) — seção "Criar Página no Frontend"
