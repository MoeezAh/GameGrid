# API Layer

tags: #backend #api #controllers #middleware #authorization

## Overview

The API layer (`GameCollection.API`) is the **outermost layer** — it wires everything together and handles HTTP concerns. It contains controllers, middleware, custom authorization handlers, and the `Program.cs` composition root.

---

## Program.cs — Application Bootstrap

```csharp
// Serilog structured logging
Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.Debug().CreateLogger();
builder.Host.UseSerilog();

// Core services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddApplicationServices();       // Application layer DI
builder.Services.AddInfrastructureServices(...); // Infrastructure layer DI

// Dynamic Permission Authorization
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// CORS
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", policy =>
    policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

// Swagger with JWT support
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", ...);
    c.AddSecurityDefinition("Bearer", ...);
    c.AddSecurityRequirement(...);
});
```

### Middleware Pipeline Order

```
1. Serilog request logging
2. ApiExceptionMiddleware (global error handler)
3. Swagger UI (Development only)
4. Static files (wwwroot/uploads)
5. Routing
6. CORS
7. Authentication (JWT validation)
8. Authorization (permission checks)
9. Controllers
```

---

## Controllers

### `AuthController` — `/api/auth`

| Method | Route | Auth | Description |
|:---|:---|:---|:---|
| POST | `/register` | None | Register new user |
| POST | `/login` | None | Login, receive JWT |
| GET | `/me` | Bearer | Get current user profile |
| POST | `/change-password` | Bearer | Update password |

### `GamesController` — `/api/games`

| Method | Route | Permission | Description |
|:---|:---|:---|:---|
| GET | `/` | `Games.View` | Paginated catalog search |
| GET | `/{id}` | `Games.View` | Game detail |
| POST | `/` | `Games.Create` | Create catalog game |
| PUT | `/{id}` | `Games.Edit` | Update catalog game |
| DELETE | `/{id}` | `Games.Delete` | Soft-delete game |
| POST | `/upload` | Bearer | Upload media file |

### `LibrariesController` — `/api/libraries`

| Method | Route | Permission | Description |
|:---|:---|:---|:---|
| GET | `/` | `Libraries.View` | My library |
| GET | `/{id}` | `Libraries.View` | Single entry |
| POST | `/` | `Libraries.Manage` | Add game to library |
| PUT | `/{id}` | `Libraries.Manage` | Update entry |
| DELETE | `/{id}` | `Libraries.Manage` | Remove from library |

### `GameRequestsController` — `/api/gamerequests`

| Method | Route | Permission | Description |
|:---|:---|:---|:---|
| GET | `/my` | `GameRequests.ViewMine` | My submissions |
| POST | `/` | `GameRequests.Submit` | New request |
| GET | `/` | `GameRequests.Review` | All requests (admin) |
| GET | `/check-duplicate` | `GameRequests.Review` | Catalog duplicate check |
| POST | `/{id}/approve` | `GameRequests.Approve` | Approve request |
| POST | `/{id}/reject` | `GameRequests.Reject` | Reject with notes |

### `RolesController` — `/api/roles`

| Method | Route | Permission | Description |
|:---|:---|:---|:---|
| GET | `/` | `Roles.View` | All roles + permissions |
| GET | `/permissions` | `Roles.View` | All system permissions |
| POST | `/` | Super Admin | Create role |
| PUT | `/{id}` | Super Admin | Update role |
| DELETE | `/{id}` | Super Admin | Delete role |

### `UsersController` — `/api/users`

| Method | Route | Permission | Description |
|:---|:---|:---|:---|
| GET | `/` | `Users.View` | User directory |
| PUT | `/{id}/roles` | `Users.ManageRoles` | Assign roles |

### `MetadataController` — `/api/metadata`

| Method | Route | Permission | Description |
|:---|:---|:---|:---|
| GET | `/{category}/list` | Bearer | Dropdown select list |
| GET | `/{category}` | `Metadata.View` | Paged table |
| POST | `/{category}` | `Metadata.Manage` | Create item |
| PUT | `/{category}/{id}` | `Metadata.Manage` | Update item |
| DELETE | `/{category}/{id}` | `Metadata.Manage` | Delete item |

Categories: `platforms`, `services`, `developers`, `publishers`, `genres`, `tags`, `themes`, `franchises`, `series`

### `DashboardController` — `/api/dashboard`

| Method | Route | Auth | Description |
|:---|:---|:---|:---|
| GET | `/stats` | Bearer | KPI metrics + chart datasets |

---

## Custom Authorization Components

### `HasPermissionAttribute`

```csharp
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) 
        : base(permission) { }
}
```

### `PermissionPolicyProvider`

Dynamically creates a `IAuthorizationPolicy` for any policy name that looks like a permission string. This means no permissions need to be registered at startup.

```csharp
public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
{
    var policy = new AuthorizationPolicyBuilder()
        .AddRequirements(new PermissionRequirement(policyName))
        .Build();
    return Task.FromResult<AuthorizationPolicy?>(policy);
}
```

### `PermissionAuthorizationHandler`

Evaluates `PermissionRequirement` by:
1. Checking if user is Super Admin → succeed immediately
2. Checking `IPermissionService.HasPermissionAsync(userId, permission)` → succeed if true

---

## Global Exception Middleware

**File:** `Middleware/ApiExceptionMiddleware.cs`

Returns consistent RFC 7807 `ProblemDetails` JSON for all unhandled exceptions:

```json
// Validation error (400)
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Validation Failed",
  "status": 400,
  "errors": ["Title is required.", "At least one platform is required."]
}

// Not found (404)
{
  "type": "...",
  "title": "Not Found",
  "status": 404,
  "detail": "Game with ID 999 was not found."
}

// Server error (500)
{
  "type": "...",
  "title": "An unexpected error occurred.",
  "status": 500
}
```

---

## `CurrentUserService`

**File:** `Services/CurrentUserService.cs`

```csharp
public class CurrentUserService : ICurrentUserService
{
    public string? UserId => 
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
```

Used in CQRS handlers to scope operations to the authenticated user (e.g., library queries only return the current user's entries).

---

## Navigation

← [[Backend/Infrastructure Layer]] | [[Backend/API Endpoints]] →
