# Clean Architecture

tags: #architecture #clean-architecture #ddd

## Overview

GameGrid follows **Clean Architecture** with **Domain-Driven Design (DDD)** principles. The codebase is split into four distinct .NET projects, each representing a layer with strict dependency rules.

```
┌─────────────────────────────────────────────┐
│              GameCollection.API              │  ← Entry point, HTTP, Auth handlers
│         (Presentation / API Layer)           │
└──────────────────────┬──────────────────────┘
                       │ depends on
┌──────────────────────▼──────────────────────┐
│          GameCollection.Infrastructure       │  ← DB, Identity, File Storage
│           (Infrastructure Layer)             │
└──────────────────────┬──────────────────────┘
                       │ depends on
┌──────────────────────▼──────────────────────┐
│          GameCollection.Application          │  ← CQRS, DTOs, Validators, Mappings
│            (Application Layer)               │
└──────────────────────┬──────────────────────┘
                       │ depends on
┌──────────────────────▼──────────────────────┐
│            GameCollection.Domain             │  ← Entities, Enums, Interfaces
│              (Domain Layer)                  │
│         *** ZERO external dependencies ***   │
└─────────────────────────────────────────────┘
```

---

## Layer Responsibilities

### GameCollection.Domain (Innermost)
- **Contains:** Domain Entities, Enums, Domain Interfaces (`IRepository<T>`, `IUnitOfWork`)
- **Rule:** No NuGet packages, no EF Core, no ASP.NET — pure C# models only
- **Key files:**
  - `Entities/Game.cs` — Central catalog game
  - `Entities/UserLibraryEntry.cs` — Personal library record
  - `Entities/GameRequest.cs` — Community submission
  - `Entities/ApplicationRole.cs`, `Permission.cs` — RBAC models
  - `Enums/CompletionStatus.cs`, `GameRequestStatus.cs`
  - `Interfaces/IRepository.cs`, `IUnitOfWork.cs`

### GameCollection.Application
- **Contains:** CQRS Commands & Queries, DTOs, AutoMapper Profiles, FluentValidation Validators, Service Interfaces
- **Key patterns:**
  - `Features/Games/Commands/` — CreateGame, UpdateGame, DeleteGame
  - `Features/Games/Queries/` — GetGames, GetGameById
  - `Features/Libraries/`, `Features/GameRequests/`, `Features/Dashboard/`
  - `DTOs/` — Request/Response transfer objects
  - `Mappings/` — AutoMapper profile classes
  - `Common/Interfaces/` — `IIdentityService`, `IPermissionService`, `ICurrentUserService`, `IFileStorageService`

### GameCollection.Infrastructure
- **Contains:** EF Core `DbContext`, Identity integration, Repository implementations, Migrations, File storage
- **Key files:**
  - `Data/GameDbContext.cs` — EF Core context with soft-delete global filters
  - `Data/GameDbContextSeed.cs` — Initial data seeder (roles, permissions, users)
  - `Repositories/EfRepository.cs` — Generic repository implementation
  - `Identity/IdentityService.cs` — Register, Login, JWT generation
  - `Identity/PermissionService.cs` — Permission lookup and Super Admin check
  - `Files/LocalFileStorageService.cs` — Handles image/media uploads to `wwwroot/uploads`

### GameCollection.API (Outermost)
- **Contains:** Controllers, Middleware, Custom Authorization Handlers, Program.cs
- **Key files:**
  - `Program.cs` — DI composition root, middleware pipeline, DB migration on startup
  - `Controllers/` — 8 controllers (Auth, Games, Libraries, GameRequests, Roles, Users, Metadata, Dashboard)
  - `Authorization/` — `PermissionPolicyProvider`, `PermissionAuthorizationHandler`, `HasPermissionAttribute`
  - `Middleware/ApiExceptionMiddleware.cs` — Global RFC 7807 error handler

---

## Dependency Rules

```
API    → can reference Application, Infrastructure, Domain
Infra  → can reference Application, Domain
App    → can reference Domain only
Domain → references nothing
```

> [!IMPORTANT]
> **Never** add a reference from Domain → Application, Domain → Infrastructure, or Application → Infrastructure. This would break the architecture.

---

## CQRS Pattern

All read/write operations go through MediatR:

```csharp
// Command (write operation)
var result = await _mediator.Send(new CreateGameCommand { Title = "...", ... });

// Query (read operation)
var games = await _mediator.Send(new GetGamesQuery { Page = 1, PageSize = 20 });
```

Each handler lives in `GameCollection.Application/Features/<Feature>/Commands/` or `.../Queries/`.

---

## Navigation

← [[Home]] | [[Architecture/Project Structure]] →
