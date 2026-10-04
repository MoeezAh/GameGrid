# Auth Context

tags: #frontend #auth #context #session #permissions

## Overview

`AuthContext.jsx` is the central session management and permission evaluation hub for the React frontend. It wraps the entire app via `<AuthProvider>` and exposes a `useAuth()` hook.

**File:** `src/context/AuthContext.jsx`

---

## State

```typescript
{
  user: {
    username: string,
    email: string,
    roles: string[],            // e.g. ["Super Admin", "Admin"]
    permissions: string[],       // e.g. ["Games.View", "Games.Create", ...]
    isSuperAdmin: boolean
  } | null,
  loading: boolean               // true during initial localStorage hydration
}
```

The `user` object is persisted in `localStorage` as JSON. On every page load, it is re-hydrated from storage (no API call needed).

---

## Context Methods

### `login(usernameOrEmail, password)`
```javascript
const result = await login('admin', 'Admin123!');
// result: { success: true } or { success: false, message: "..." }
```
- POSTs to `/api/auth/login`
- Saves token to `localStorage.setItem('token', ...)`
- Saves user object to `localStorage.setItem('user', ...)`
- Sets `user` state

### `register(username, email, password)`
Same flow as login but POSTs to `/api/auth/register`.

### `logout()`
Clears `localStorage` (token + user) and sets `user = null`.

### `refreshProfile()`
```javascript
await refreshProfile();
```
Calls `GET /api/auth/profile` to re-fetch current roles/permissions. Useful after an admin changes a user's roles — the user's session is updated without re-login.

---

## Permission Helpers

### `isSuperAdmin()`
```javascript
const { isSuperAdmin } = useAuth();
if (isSuperAdmin()) { /* show super admin controls */ }
```
Returns `true` if `user.isSuperAdmin === true`.

### `hasPermission(permissionName)`
```javascript
const { hasPermission } = useAuth();
if (hasPermission('Games.Create')) { /* show create button */ }
```
- Returns `true` if Super Admin (full bypass)
- Returns `true` if `user.permissions` includes the given identifier
- Returns `false` if not logged in

### `hasAnyPermission(permissionNames[])`
```javascript
if (hasAnyPermission(['GameRequests.Review', 'GameRequests.Approve'])) { ... }
```
Returns `true` if the user has **any** of the provided permissions.

### `hasRole(roleName)`
```javascript
if (hasRole('Administrator')) { ... }
```
Checks if `user.roles` contains the given role name.

### `isAdmin()`
Convenience alias — returns `true` if Super Admin, or has role "Administrator" or "Admin". Used for the `AdminRoute` guard.

---

## Usage Example

```jsx
import { useAuth } from '../context/AuthContext';

function MyComponent() {
  const { user, hasPermission, isSuperAdmin, logout } = useAuth();
  
  if (!user) return null;
  
  return (
    <div>
      <p>Welcome, {user.username}</p>
      {hasPermission('Games.Create') && (
        <button>Add New Game</button>
      )}
      {isSuperAdmin() && (
        <a href="/admin/roles">Manage Roles</a>
      )}
      <button onClick={logout}>Logout</button>
    </div>
  );
}
```

---

## Session Persistence

```
App loads
  └── useEffect() fires
        └── reads localStorage('user') + localStorage('token')
              ├── valid → setUser(parsedUser), setLoading(false)
              └── invalid/missing → setUser(null), setLoading(false)

Route guards check loading before rendering
  → prevents flash of unauthorized content on page reload
```

---

## Navigation

← [[Frontend/React App Structure]] | [[Frontend/Route Guards]] →
