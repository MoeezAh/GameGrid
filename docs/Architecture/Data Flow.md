# Data Flow

tags: #architecture #data-flow #request-lifecycle

## Full Request Lifecycle

This traces a typical authenticated API call — e.g., **GET /api/games** — from the React frontend to the database and back.

```
Browser (React)
  │
  │  1. Axios adds "Authorization: Bearer <JWT>" header via interceptor
  ▼
ASP.NET Core Pipeline (GameCollection.API)
  │
  │  2. Serilog logs the incoming request
  │  3. ApiExceptionMiddleware wraps the handler in try/catch
  │  4. UseRouting() matches route → GamesController.GetAll()
  │  5. UseCors() validates origin header
  │  6. UseAuthentication() validates JWT → populates HttpContext.User
  │  7. UseAuthorization() evaluates [HasPermission("Games.View")]
  │     └── PermissionPolicyProvider builds PermissionRequirement
  │     └── PermissionAuthorizationHandler checks:
  │           a) Is user Super Admin? → immediate success
  │           b) Does user have "Games.View" permission? → via PermissionService
  │
  ▼
GamesController.GetAll(query)
  │
  │  8. Controller sends MediatR query: mediator.Send(new GetGamesQuery {...})
  ▼
GetGamesQueryHandler (Application Layer)
  │
  │  9. Calls IRepository<Game>.GetAsync(filter, includes, pagination)
  ▼
EfRepository<Game> (Infrastructure Layer)
  │
  │  10. Executes LINQ query against GameDbContext
  │      - Global query filter: WHERE IsDeleted = 0 (soft delete)
  │      - Applies search/filter predicates
  │      - Applies pagination (Skip/Take)
  ▼
SQL Server Database
  │
  │  11. Returns hydrated Game entities with navigation properties
  ▼
GetGamesQueryHandler
  │
  │  12. AutoMapper maps Game → GameDto
  │  13. Returns PagedResult<GameDto>
  ▼
GamesController
  │
  │  14. Returns Ok(result) → HTTP 200 JSON
  ▼
Axios (Frontend)
  │
  │  15. Response interceptor: on 401, clears localStorage and redirects to /login
  │  16. Component state updated → React re-renders with data
  ▼
User sees the catalog
```

---

## Authentication Token Flow

```
1. User submits credentials (POST /api/auth/login)
2. IdentityService validates username/password via ASP.NET Identity
3. PermissionService aggregates all permissions from user's assigned roles
4. JWT token is generated:
   - Claims: NameIdentifier (userId), roles[], permissions[]
   - Signed with HMAC-SHA256 using JwtSettings:Secret
   - Expires: JwtSettings:DurationInMinutes (default 1440 = 24h)
5. Response: { token, username, email, roles[], permissions[], isSuperAdmin }
6. Frontend stores token in localStorage
7. Axios interceptor attaches token to every subsequent request
```

---

## File Upload Flow

```
1. User selects image file in GameForm (cover art / banner / screenshot)
2. React submits multipart/form-data to POST /api/games/upload
3. GamesController receives IFormFile
4. Calls IFileStorageService.SaveFileAsync(file)
5. LocalFileStorageService saves to: wwwroot/uploads/<guid>.<ext>
6. Returns relative URL: /uploads/<guid>.<ext>
7. Frontend stores URL string in form state
8. URL is saved as part of the Game record in the database
9. Frontend renders image via: <img src={`${API_BASE_URL}${coverImagePath}`} />
```

---

## Navigation

← [[Architecture/Project Structure]] | [[Security/Authentication]] →
