# Backend Settings

tags: #configuration #backend #appsettings

## `appsettings.json` — Full Reference

**File:** `backend/GameGrid.API/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "CorsSettings": {
    "AllowedOrigins": "http://localhost:5173"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GameGridDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "Secret": "SuperSecretKeyForGameGridManagementAppKeyHere_1234567890!",
    "Issuer": "GameGridAPI",
    "Audience": "GameGridApp",
    "DurationInMinutes": 1440
  }
}
```

---

## Key Reference

| Key | Default Value | Description |
|:---|:---|:---|
| `CorsSettings:AllowedOrigins` | `http://localhost:5173` | Comma-separated list of allowed frontend origins. E.g. for Docker: `http://localhost` |
| `ConnectionStrings:DefaultConnection` | LocalDB string | SQL Server connection string |
| `JwtSettings:Secret` | `SuperSecretKey...` | HMAC-SHA256 signing key. **Must be 32+ characters.** |
| `JwtSettings:Issuer` | `GameGridAPI` | JWT `iss` claim |
| `JwtSettings:Audience` | `GameGridApp` | JWT `aud` claim |
| `JwtSettings:DurationInMinutes` | `1440` | Token lifetime — 1440 min = 24 hours |
| `Logging:LogLevel:Default` | `Information` | Minimum log level for application code |
| `Logging:LogLevel:Microsoft.AspNetCore` | `Warning` | Reduces framework noise |

---

## Environment-Specific Overrides

`appsettings.Development.json` overrides values in development:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

---

## Docker Environment Variables

When using Docker Compose, settings are passed as environment variables using double-underscore as the section separator:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ConnectionStrings__DefaultConnection=Server=database;Database=GameGridDb;User Id=sa;Password=...
  - JwtSettings__Secret=SuperSecretKeyHere
  - JwtSettings__Issuer=GameGridAPI
  - JwtSettings__Audience=GameGridApp
  - JwtSettings__DurationInMinutes=1440
```

---

## VS Code Launch Ports

In `.vscode/launch.json`, ports are explicitly set:

```json
{
  "ASPNETCORE_URLS": "https://localhost:7214;http://localhost:5139"
}
```

This ensures:
- Swagger: `https://localhost:7214/swagger`
- API calls: `http://localhost:5139/api`

---

## Password Policy (ASP.NET Identity)

Configured in `Infrastructure/ConfigureServices.cs` to be lenient for development:

```csharp
options.Password.RequireDigit = false;
options.Password.RequiredLength = 6;
options.Password.RequireNonAlphanumeric = false;
options.Password.RequireUppercase = false;
options.Password.RequireLowercase = false;
```

> [!WARNING]
> Tighten these password requirements for production deployments.

---

## Navigation

← [[Database/Seeded Data]] | [[Configuration/Frontend Settings]] →
