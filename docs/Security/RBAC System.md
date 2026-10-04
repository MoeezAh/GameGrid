# RBAC System

tags: #security #rbac #authorization #permissions

## Overview

GameGrid uses a **Generic, Permission-Based Authorization** system. Instead of checking role names directly in business logic, every protected action is guarded by a discrete **permission identifier** (e.g., `"Games.Create"`).

This means:
- Adding a new role requires **zero code changes** — only database configuration
- The Super Admin has an **unconditional bypass** over all permission checks
- Roles are additive: a user's effective permissions = **union of all assigned role permissions**

---

## Role Hierarchy

```
                  +-------------------+
                  |   Super Admin     |  ← Full system bypass, role creation, RBAC matrix
                  +---------+---------+
                            |
      +---------------------+---------------------+
      |                                           |
+-----v-----+                               +-----v-----+
|   Admin   |  Catalog, Users, Requests     |  Curator  |  Catalog review & curation
+-----+-----+  Metadata                     +-----+-----+
      |                                           |
      +---------------------+---------------------+
                            |
                      +-----v-----+
                      |   User    |  Personal library, catalog view, request submissions
                      +-----------+
```

---

## How Permissions Are Enforced

### Backend: `[HasPermission]` Attribute

```csharp
// In any controller action:
[HasPermission("Games.Create")]
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateGameCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

The `HasPermission` attribute is backed by:
1. **`PermissionPolicyProvider`** — dynamically creates an `IAuthorizationPolicy` for any permission string on demand
2. **`PermissionRequirement`** — wraps the permission name
3. **`PermissionAuthorizationHandler`** — resolves the requirement:

```csharp
// Super Admin bypass
if (await _permissionService.IsSuperAdminAsync(userId))
{
    context.Succeed(requirement);
    return;
}

// Check effective permissions
if (await _permissionService.HasPermissionAsync(userId, requirement.Permission))
{
    context.Succeed(requirement);
}
```

### Frontend: Route Guards

```jsx
// PermissionRoute — single permission
<PermissionRoute permission="Games.Create">
  <GameForm />
</PermissionRoute>

// PermissionRoute — any of multiple permissions
<PermissionRoute anyPermissions={['GameRequests.Review', 'GameRequests.Approve']}>
  <AdminGameRequests />
</PermissionRoute>

// SuperAdminRoute — exclusive to Super Admin
<SuperAdminRoute>
  <AdminRoles />
</SuperAdminRoute>
```

### Frontend: In-Component Checks

```jsx
const { hasPermission, isSuperAdmin } = useAuth();

// Conditionally render UI elements
{hasPermission('Games.Edit') && <button>Edit Game</button>}
{isSuperAdmin() && <Link to="/admin/roles">Manage Roles</Link>}
```

---

## Default Roles & Capabilities

| Role | Key Capabilities |
|:---|:---|
| **Super Admin** | Everything. Unconditional bypass. Exclusive role/permission CRUD. |
| **Admin** | Full catalog management, metadata curation, user role assignment, request review. |
| **Game Curator** | Catalog moderation, game request review and approval, taxonomy management. |
| **User** | Browse catalog, manage personal library, submit game requests. |

---

## Permission Aggregation

```
User has roles: [Admin, Curator]

Admin permissions:    [Games.View, Games.Create, Games.Edit, Games.Delete,
                       Users.View, Users.ManageRoles, GameRequests.Review, ...]
Curator permissions:  [Games.View, Games.Create, GameRequests.Approve, ...]

Effective permissions = UNION of both sets (deduplicated)
```

---

## Super Admin Bypass

The Super Admin check happens before any permission evaluation. This is implemented in `PermissionService.IsSuperAdminAsync()` and checked in:
- `PermissionAuthorizationHandler` (API side)
- `isSuperAdmin()` helper in `AuthContext` (frontend side)

> [!NOTE]
> The Super Admin role is seeded at startup and cannot be deleted via the Admin UI (safe-deletion guards are in place).

---

## Navigation

← [[Security/Authentication]] | [[Security/Permission Reference]] →
