# GameGrid — AI Agent Context File

> This file is automatically loaded by AI agents (Antigravity IDE, GitHub Copilot, Cursor, etc.)
> to provide instant project context without requiring prior conversation history.

---

## Project Identity

- **Project Name:** GameGrid (codename: Antigravity)
- **Type:** Full-stack web application — Game Collection Management System
- **Stack:** ASP.NET Core 10 (Clean Architecture) + React 19 + SQL Server

---

## Repository Layout

```
<project-root>/
├── docs/                        ← Obsidian vault — full project documentation
│   ├── Home.md                  ← Start here for documentation index
│   ├── Architecture/
│   ├── Backend/
│   ├── Frontend/
│   ├── Security/
│   ├── Database/
│   ├── Configuration/
│   └── Deployment/
├── backend/                     ← ASP.NET Core .NET 10 solution
│   ├── GameCollection.Domain/   ← Entities, Enums, Interfaces (no dependencies)
│   ├── GameCollection.Application/ ← CQRS (MediatR), DTOs, FluentValidation
│   ├── GameCollection.Infrastructure/ ← EF Core, Identity, JWT, File Storage
│   └── GameCollection.API/      ← Controllers, Auth Handlers, Program.cs
├── frontend/                    ← React 19 + Vite SPA
│   └── src/
│       ├── context/AuthContext.jsx  ← Session + permission helpers
│       ├── components/RouteGuards.jsx
│       └── pages/               ← Dashboard, Catalog, Library, Admin, etc.
├── docker-compose.yml
├── README.md
└── FEATURE_SPECIFICATION.md     ← Exhaustive technical spec (read this first)
```

---

## Architecture Rules (Critical)

- **Clean Architecture** with strict layer dependencies:
  `API → Infrastructure → Application → Domain`
  Domain has ZERO external dependencies.
- **CQRS via MediatR**: Commands (writes) and Queries (reads) are separate handlers
- **Generic RBAC**: Business logic uses `[HasPermission("Permission.Name")]` — never hardcoded role names
- **Soft Deletes**: EF Core global query filter `WHERE IsDeleted = 0` — never physical deletes
- **JWT Stateless Auth**: No server-side sessions; token contains roles and permissions claims

---

## Key Design Decisions

| Decision | Implementation |
|:---|:---|
| Central Catalog vs Personal Library | Two separate concepts — `Game` entity is authoritative; `UserLibraryEntry` is personal |
| Permission-based auth (not role-based) | `[HasPermission("Games.Create")]` attribute backed by `PermissionAuthorizationHandler` |
| Super Admin bypass | `PermissionService.IsSuperAdminAsync()` checked first in handler — no permission needed |
| Soft deletes everywhere | `IsDeleted = true` via `EfRepository.Delete()` — global query filter hides them |
| Idempotent seeder | `GameDbContextSeed.SeedAsync()` runs on every startup safely |

---

## Default Credentials (Development)

| Username | Password | Role |
|:---|:---|:---|
| `admin` | `Admin123!` | Super Admin, Admin |
| `user` | `User123!` | User |

---

## Local Dev Ports

| Service | URL |
|:---|:---|
| React Frontend | `http://localhost:5173` |
| API (HTTP) | `http://localhost:5139` |
| API (HTTPS) | `https://localhost:7214` |
| Swagger UI | `http://localhost:5139/swagger` |

---

## Where to Find Things

| Question | Answer |
|:---|:---|
| All API endpoints | `docs/Backend/API Endpoints.md` or `backend/GameCollection.API/Controllers/` |
| All permissions | `docs/Security/Permission Reference.md` |
| Database schema | `docs/Database/Entity Relationship Model.md` |
| CQRS handlers | `backend/GameCollection.Application/Features/` |
| Frontend routes | `frontend/src/App.jsx` |
| Seeded data | `backend/GameCollection.Infrastructure/Data/GameDbContextSeed.cs` |
| EF Core context | `backend/GameCollection.Infrastructure/Data/GameDbContext.cs` |

---

## Do NOT

- Add dependencies from `Domain` → `Application` or `Infrastructure`
- Add hardcoded role name checks in business logic (use permissions)
- Physically delete records (use soft delete via `entity.IsDeleted = true`)
- Store secrets in source control (rotate `JwtSettings:Secret` before production)

---

*This file is intentionally path-independent — it works regardless of where this project is moved or renamed.*
