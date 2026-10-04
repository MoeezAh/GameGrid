# Infrastructure Layer

tags: #backend #infrastructure #efcore #identity #database

## Overview

The Infrastructure layer (`GameCollection.Infrastructure`) implements all external concerns: database access via EF Core, ASP.NET Identity, JWT generation, and local file storage.

---

## EF Core: `GameDbContext`

**File:** `Data/GameDbContext.cs`

### Key Configuration

```csharp
public class GameDbContext : IdentityDbContext<IdentityUser>
{
    // Domain entity tables
    public DbSet<Game> Games { get; set; }
    public DbSet<UserLibraryEntry> UserLibraryEntries { get; set; }
    public DbSet<GameRequest> GameRequests { get; set; }
    public DbSet<ApplicationRole> ApplicationRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    // + all taxonomy tables: Platforms, Genres, Developers, etc.
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Global soft-delete query filter — applies to ALL queries automatically
        builder.Entity<Game>().HasQueryFilter(g => !g.IsDeleted);
        builder.Entity<GameRequest>().HasQueryFilter(gr => !gr.IsDeleted);
        
        // Many-to-many join table configurations
        builder.Entity<Game>()
            .HasMany(g => g.Platforms)
            .WithMany()
            .UsingEntity("GamePlatforms");
        
        // ... similar for Genres, Tags, Developers, etc.
    }
}
```

### Soft Delete

All deletes are **logical** — `IsDeleted = true` is set instead of executing a `DELETE` statement. The global query filter ensures soft-deleted records are invisible to all application queries without any extra code.

---

## Database Seeder: `GameDbContextSeed`

**File:** `Data/GameDbContextSeed.cs`

Runs automatically on application startup (called from `Program.cs`). Seeds:

1. **Permissions** — All 20 permission identifiers
2. **Roles** — Super Admin, Admin, Game Curator, User
3. **Role-Permission assignments** — Default capability matrix
4. **Users** — `admin` (Super Admin + Admin) and `user` (User)
5. **Sample taxonomy data** — Platforms, genres, developers, publishers
6. **Sample games** — A few catalog entries for demonstration

> [!NOTE]
> The seeder is **idempotent** — it checks for existing records before inserting, so it's safe to run on every startup.

---

## Generic Repository Pattern

**File:** `Repositories/EfRepository.cs`

```csharp
public class EfRepository<T> : IRepository<T> where T : BaseAuditableEntity
{
    private readonly GameDbContext _context;
    
    public async Task<T?> GetByIdAsync(int id) 
        => await _context.Set<T>().FindAsync(id);
    
    public async Task AddAsync(T entity) 
        => await _context.Set<T>().AddAsync(entity);
    
    public void Delete(T entity) 
        => entity.IsDeleted = true;  // Soft delete
    
    // ... GetAllAsync with optional filter expressions
}
```

---

## Identity Service

**File:** `Identity/IdentityService.cs`

Wraps ASP.NET Core Identity's `UserManager<IdentityUser>` and `SignInManager`.

Key methods:
- `RegisterAsync(username, email, password)` — creates Identity user, assigns default "User" role
- `LoginAsync(usernameOrEmail, password)` — validates credentials, generates JWT
- `ChangePasswordAsync(userId, current, new)` — verifies current password before updating
- `GetUserProfileAsync(userId)` — returns user info + current role/permission list

### JWT Token Generation

```csharp
private string GenerateJwtToken(IdentityUser user, List<string> roles, List<string> permissions, bool isSuperAdmin)
{
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Name, user.UserName!),
        new("isSuperAdmin", isSuperAdmin.ToString()),
    };
    
    claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
    claims.AddRange(permissions.Select(p => new Claim("permission", p)));
    
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(_durationMinutes),
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(_key), 
            SecurityAlgorithms.HmacSha256Signature)
    };
    
    // ...
}
```

---

## Permission Service

**File:** `Identity/PermissionService.cs`

```csharp
public class PermissionService : IPermissionService
{
    public async Task<bool> IsSuperAdminAsync(string userId)
    {
        // Check if user is assigned the "Super Admin" role
    }
    
    public async Task<bool> HasPermissionAsync(string userId, string permission)
    {
        // Load all roles assigned to user
        // For each active role, load its permissions
        // Return true if any role has the requested permission
    }
}
```

---

## File Storage Service

**File:** `Files/LocalFileStorageService.cs`

```csharp
public class LocalFileStorageService : IFileStorageService
{
    // Saves uploaded file to: wwwroot/uploads/<guid>.<ext>
    // Returns relative URL: /uploads/<guid>.<ext>
    // Validates: only jpg, jpeg, png, gif, webp allowed
    // Max file size enforced by ASP.NET Core request size limits
}
```

---

## DI Registration

```csharp
// GameCollection.Infrastructure/ConfigureServices.cs
services.AddDbContext<GameDbContext>(...);
services.AddIdentity<IdentityUser, IdentityRole>(...).AddEntityFrameworkStores<GameDbContext>();
services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IIdentityService, IdentityService>();
services.AddScoped<IPermissionService, PermissionService>();
services.AddScoped<IFileStorageService, LocalFileStorageService>();
// JWT bearer authentication configuration...
```

---

## Migrations

```bash
# From repository root, run in backend/ directory:
dotnet ef migrations add <MigrationName> \
  --project GameCollection.Infrastructure \
  --startup-project GameCollection.API

dotnet ef database update \
  --project GameCollection.Infrastructure \
  --startup-project GameCollection.API
```

> [!TIP]
> In production (Docker), the database is migrated automatically in `Program.cs` via `context.Database.MigrateAsync()` on startup.

---

## Navigation

← [[Backend/Application Layer]] | [[Backend/API Layer]] →
