# Project Structure

tags: #architecture #project-structure #file-layout

## Repository Root

```
Antigravity/
├── docs/                          ← 📖 This Obsidian vault
├── backend/                       ← ASP.NET Core solution
├── frontend/                      ← React SPA
├── docker-compose.yml             ← Full-stack container orchestration
├── .gitignore
├── README.md                      ← Quick-start guide
└── FEATURE_SPECIFICATION.md       ← Detailed feature & technical spec
```

---

## Backend Structure

```
backend/
└── GameCollection.slnx            ← Modern XML solution file

GameCollection.Domain/
├── Entities/
│   ├── Game.cs                    ← Central catalog game entity
│   ├── UserLibraryEntry.cs        ← Personal library entry
│   ├── GameRequest.cs             ← Community request submission
│   ├── ApplicationRole.cs         ← RBAC role model
│   ├── ApplicationRolePermission.cs
│   ├── ApplicationUserRole.cs
│   ├── Permission.cs              ← Individual permission record
│   ├── Platform.cs
│   ├── DigitalService.cs
│   ├── Developer.cs
│   ├── Publisher.cs
│   ├── Genre.cs
│   ├── Tag.cs
│   ├── Theme.cs
│   ├── Franchise.cs
│   └── Series.cs
├── Enums/
│   ├── CompletionStatus.cs        ← NotStarted(0)…Replaying(6)
│   └── GameRequestStatus.cs       ← Pending(0), Approved(1), Rejected(2)
├── Interfaces/
│   ├── IRepository.cs             ← Generic repository contract
│   └── IUnitOfWork.cs             ← Transaction management contract
└── Common/
    └── BaseAuditableEntity.cs     ← Id, CreatedAt, UpdatedAt, IsDeleted

GameCollection.Application/
├── Features/
│   ├── Games/
│   │   ├── Commands/              ← CreateGame, UpdateGame, DeleteGame
│   │   └── Queries/               ← GetGames, GetGameById
│   ├── Libraries/
│   │   ├── Commands/
│   │   └── Queries/
│   ├── GameRequests/
│   │   ├── Commands/
│   │   └── Queries/
│   └── Dashboard/
│       └── Queries/
├── DTOs/
│   ├── Auth/
│   ├── Game/
│   ├── Library/
│   ├── GameRequest/
│   ├── Dashboard/
│   ├── Metadata/
│   └── RBAC/
├── Mappings/                      ← AutoMapper profiles
├── Common/
│   ├── Interfaces/                ← IIdentityService, IPermissionService, etc.
│   └── Security/                  ← PermissionConstants
└── ConfigureServices.cs           ← Application layer DI registration

GameCollection.Infrastructure/
├── Data/
│   ├── GameDbContext.cs           ← EF Core context, soft-delete filters
│   └── GameDbContextSeed.cs       ← Seed: roles, permissions, admin/user accounts
├── Repositories/
│   ├── EfRepository.cs            ← Generic IRepository<T> via EF Core
│   └── UnitOfWork.cs
├── Identity/
│   ├── IdentityService.cs         ← Register, Login, JWT token generation
│   └── PermissionService.cs       ← HasPermissionAsync, IsSuperAdminAsync
├── Files/
│   └── LocalFileStorageService.cs ← Saves uploads to wwwroot/uploads/
├── Migrations/                    ← EF Core generated migration files
└── ConfigureServices.cs           ← Infrastructure DI registration

GameCollection.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── GamesController.cs
│   ├── LibrariesController.cs
│   ├── GameRequestsController.cs
│   ├── RolesController.cs
│   ├── UsersController.cs
│   ├── MetadataController.cs
│   └── DashboardController.cs
├── Authorization/
│   ├── HasPermissionAttribute.cs
│   ├── PermissionRequirement.cs
│   ├── PermissionPolicyProvider.cs
│   └── PermissionAuthorizationHandler.cs
├── Middleware/
│   └── ApiExceptionMiddleware.cs  ← RFC 7807 global error handler
├── Services/
│   └── CurrentUserService.cs      ← ICurrentUserService (reads claims)
├── Properties/
│   └── launchSettings.json
├── Program.cs                     ← Composition root, pipeline config
├── appsettings.json
├── appsettings.Development.json
└── Dockerfile
```

---

## Frontend Structure

```
frontend/
├── index.html                     ← Vite HTML entry point
├── vite.config.js
├── package.json
├── eslint.config.js
├── Dockerfile
├── nginx.conf                     ← Production Nginx config
└── src/
    ├── main.jsx                   ← React app mount point
    ├── App.jsx                    ← Router + route definitions
    ├── index.css                  ← HSL Gaming Theme CSS variables & global styles
    ├── App.css
    ├── context/
    │   └── AuthContext.jsx        ← Session state, login/logout, permission helpers
    ├── components/
    │   ├── Layout.jsx             ← App shell (sidebar nav + header + outlet)
    │   └── RouteGuards.jsx        ← ProtectedRoute, PermissionRoute, SuperAdminRoute, etc.
    ├── pages/
    │   ├── Dashboard.jsx          ← KPI cards + Recharts analytics
    │   ├── Catalog.jsx            ← Central game catalog (Grid/List views + filters)
    │   ├── Library.jsx            ← Personal library management
    │   ├── GameDetail.jsx         ← Full game detail page (hero, media, trailer)
    │   ├── GameForm.jsx           ← Add/Edit catalog game (3-tab form)
    │   ├── Login.jsx
    │   ├── Register.jsx
    │   ├── Profile.jsx
    │   ├── GameRequests/
    │   │   └── MyGameRequests.jsx ← User-facing request submission & status tracker
    │   └── Admin/
    │       ├── AdminGameRequests.jsx ← Reviewer dashboard (approve/reject)
    │       ├── AdminRoles.jsx        ← RBAC role & permission matrix editor
    │       ├── AdminUsers.jsx        ← User directory & role assignment
    │       └── AdminMetadata.jsx     ← Taxonomy CRUD (platforms, genres, etc.)
    └── services/
        └── api.js                 ← Axios instance + JWT interceptor
```

---

## Navigation

← [[Architecture/Clean Architecture]] | [[Architecture/Data Flow]] →
