# Route Guards

tags: #frontend #routing #authorization #guards

## Overview

Route guards are React wrapper components in `src/components/RouteGuards.jsx` that protect routes based on authentication status and user permissions. They wrap page components in `App.jsx`.

All guards show a **spinner** while `loading = true` (during session hydration) to prevent flash of unauthenticated content.

---

## Available Guards

### `ProtectedRoute`
**Purpose:** Require the user to be logged in.

```jsx
<ProtectedRoute>
  <SomePage />
</ProtectedRoute>
```

Behavior:
- `loading` → show spinner
- `!user` → redirect to `/login`
- `user` → render children

Used as the wrapper for the entire authenticated app shell in `App.jsx`.

---

### `AuthRoute`
**Purpose:** Redirect already-logged-in users away from auth pages (login, register).

```jsx
<AuthRoute>
  <Login />
</AuthRoute>
```

Behavior:
- `loading` → show spinner
- `user` is already logged in → redirect to `/`
- `!user` → render children (show the login/register page)

---

### `PermissionRoute`
**Purpose:** Require a specific permission (or any of a set of permissions).

```jsx
// Single permission required
<PermissionRoute permission="Games.Create">
  <GameForm />
</PermissionRoute>

// Any of multiple permissions
<PermissionRoute anyPermissions={['GameRequests.Review', 'GameRequests.Approve', 'GameRequests.Reject']}>
  <AdminGameRequests />
</PermissionRoute>
```

Behavior:
- `loading` → spinner
- `!user` → `/login`
- `isSuperAdmin()` → bypass, render children
- `permission` prop set and `!hasPermission(permission)` → redirect to `/`
- `anyPermissions` prop set and `!hasAnyPermission(anyPermissions)` → redirect to `/`
- Otherwise → render children

Props:
| Prop | Type | Description |
|:---|:---|:---|
| `permission` | `string` | Single permission identifier required |
| `anyPermissions` | `string[]` | User needs at least one of these |
| `children` | ReactNode | The protected component |

---

### `SuperAdminRoute`
**Purpose:** Restrict route to Super Admin users only.

```jsx
<SuperAdminRoute>
  <AdminRoles />
</SuperAdminRoute>
```

Behavior:
- `loading` → spinner
- `!user` → `/login`
- `!isSuperAdmin()` → `/` (access denied)
- `isSuperAdmin()` → render children

---

### `AdminRoute`
**Purpose:** Allow Admin role, Super Admin, or users with `Metadata.View` permission.

```jsx
<AdminRoute>
  <AdminMetadata />
</AdminRoute>
```

Behavior:
- `loading` → spinner
- `!user` → `/login`
- `isSuperAdmin() || hasPermission('Metadata.View') || isAdmin()` → render
- Otherwise → `/`

---

## Guard Decision Summary

| Guard | Not logged in | Super Admin | Has permission | No permission |
|:---|:---:|:---:|:---:|:---:|
| `ProtectedRoute` | → `/login` | ✅ render | ✅ render | ✅ render |
| `AuthRoute` | ✅ render | → `/` | → `/` | → `/` |
| `PermissionRoute` | → `/login` | ✅ bypass | ✅ render | → `/` |
| `SuperAdminRoute` | → `/login` | ✅ render | → `/` | → `/` |
| `AdminRoute` | → `/login` | ✅ render | (Metadata.View) ✅ | → `/` |

---

## Adding a New Protected Route

```jsx
// 1. In App.jsx, import the guard
import { PermissionRoute } from './components/RouteGuards';

// 2. Wrap the new page component
<Route
  path="new-feature"
  element={
    <PermissionRoute permission="NewFeature.View">
      <NewFeaturePage />
    </PermissionRoute>
  }
/>
```

---

## Navigation

← [[Frontend/Auth Context]] | [[Frontend/Pages Reference]] →
