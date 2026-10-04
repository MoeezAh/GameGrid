# Tech Stack

tags: #tech-stack #frameworks #dependencies

## Backend

| Category | Technology | Version | Purpose |
|:---|:---|:---|:---|
| Runtime | .NET / ASP.NET Core | 10.0 | Web API host and HTTP pipeline |
| ORM | Entity Framework Core | 10.0 | Database access and code-first migrations |
| CQRS | MediatR | latest | Separates Commands (writes) from Queries (reads) |
| Validation | FluentValidation | latest | Pipeline behaviors for automatic request validation |
| Mapping | AutoMapper | latest | DTO ↔ Domain entity mapping profiles |
| Auth | JWT Bearer (System.IdentityModel) | latest | Stateless token authentication |
| Identity | ASP.NET Core Identity | built-in | User/role management, password hashing |
| Logging | Serilog | latest | Structured console & debug logging |
| API Docs | Swashbuckle (Swagger) | latest | OpenAPI spec and Swagger UI |
| Error Handling | RFC 7807 ProblemDetails | custom middleware | Consistent JSON error responses |
| Database | SQL Server / LocalDB | 2022 | Relational data persistence |

### Backend NuGet Packages (key)

```xml
<!-- GameCollection.Application -->
MediatR
FluentValidation.AspNetCore
AutoMapper

<!-- GameCollection.Infrastructure -->
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.AspNetCore.Authentication.JwtBearer
System.IdentityModel.Tokens.Jwt

<!-- GameCollection.API -->
Swashbuckle.AspNetCore
Serilog.AspNetCore
```

---

## Frontend

| Category | Technology | Version | Purpose |
|:---|:---|:---|:---|
| UI Framework | React | 19.3.0 | Component-based SPA |
| Build Tool | Vite | 8.3.0 | HMR dev server & production bundler |
| CSS Framework | Bootstrap | 5.3.8 | Responsive grid and utility classes |
| Icons | Bootstrap Icons | 1.13.1 | SVG icon library |
| Charting | Recharts | 3.10.1 | Responsive SVG analytics charts |
| Routing | React Router DOM | 7.18.3 | Client-side SPA routing |
| HTTP Client | Axios | 1.20.0 | API calls with request/response interceptors |
| Forms | React Hook Form | 7.88.0 | Performant form state management |

### Frontend npm Scripts

```bash
npm run dev       # Start Vite dev server (http://localhost:5173)
npm run build     # Production build to /dist
npm run lint      # ESLint check
npm run preview   # Preview production build locally
```

---

## Infrastructure & DevOps

| Tool | Purpose |
|:---|:---|
| Docker | Container runtime |
| Docker Compose | Multi-service orchestration (DB + API + Frontend) |
| SQL Server 2022 (container) | Production database image |
| Nginx | Frontend production static file server |
| VS Code launch.json | Simultaneous API + frontend debug configuration |

---

## Navigation

← [[Overview]] | [[Architecture/Clean Architecture]] →
