# Authentication

tags: #security #authentication #jwt

## Mechanism: JWT Bearer Tokens

GameGrid uses **stateless JWT Bearer authentication**. No server-side sessions are maintained — the token itself carries all identity and permission information.

---

## Token Structure

```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "sub": "<userId>",                  // NameIdentifier claim
    "name": "<username>",
    "roles": ["Admin", "User"],         // all assigned role names
    "permissions": ["Games.View", "Libraries.Manage", ...],  // aggregated permissions
    "isSuperAdmin": true/false,
    "iss": "GameCollectionAPI",
    "aud": "GameCollectionApp",
    "exp": <unix timestamp>
  }
}
```

---

## Token Configuration (`appsettings.json`)

```json
{
  "JwtSettings": {
    "Secret": "SuperSecretKeyForGameCollectionManagementAppKeyHere_1234567890!",
    "Issuer": "GameCollectionAPI",
    "Audience": "GameCollectionApp",
    "DurationInMinutes": 1440
  }
}
```

> [!CAUTION]
> **Always replace `JwtSettings:Secret`** with a strong, randomly generated key in production environments. Never commit the real secret to source control.

---

## Login Flow

```
POST /api/auth/login
Body: { "usernameOrEmail": "admin", "password": "Admin123!" }

→ IdentityService validates credentials via ASP.NET Identity
→ PermissionService loads all permissions from user's active roles
→ JWT token generated (HMAC-SHA256)
→ Response:
  {
    "token": "<JWT>",
    "username": "admin",
    "email": "admin@example.com",
    "roles": ["Super Admin", "Admin"],
    "permissions": ["Games.View", "Games.Create", ...],
    "isSuperAdmin": true
  }
```

## Registration Flow

```
POST /api/auth/register
Body: { "username": "newuser", "email": "...", "password": "..." }

→ Creates ASP.NET Identity user
→ Automatically assigns default "User" role
→ Returns same token response as login
```

---

## Token Validation (API Side)

Configured in `GameCollection.Infrastructure/ConfigureServices.cs`:

```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = true,
    ValidIssuer = issuer,
    ValidateAudience = true,
    ValidAudience = audience,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero   // No grace period — tokens expire exactly at exp
};
```

---

## Frontend: Token Storage & Attachment

```javascript
// AuthContext.jsx — stored in localStorage
localStorage.setItem('token', authData.token);

// services/api.js — Axios interceptor adds header to every request
axios.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// Response interceptor — on 401, auto-logout
axios.interceptors.response.use(null, error => {
  if (error.response?.status === 401) {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/login';
  }
  return Promise.reject(error);
});
```

---

## Password Change

```
POST /api/auth/change-password
Auth: Bearer <token>
Body: { "currentPassword": "...", "newPassword": "..." }
```

Validated via ASP.NET Identity's `CheckPasswordAsync` before updating.

---

## Navigation

← [[Architecture/Data Flow]] | [[Security/RBAC System]] →
