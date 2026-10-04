# React App Structure

tags: #frontend #react #routing #structure

## Entry Point

```
main.jsx → App.jsx (Router + AuthProvider) → Layout.jsx (shell) → Pages
```

### `main.jsx`
Mounts the React app into `#root`:
```jsx
ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
```

---

## `App.jsx` — Route Definitions

All routes are defined here. The layout shell wraps all authenticated routes.

```
/login              → Login.jsx              (AuthRoute — redirect if logged in)
/register           → Register.jsx           (AuthRoute)

/ (protected)       → Layout.jsx shell
  /                 → Dashboard.jsx          (index)
  /catalog          → Catalog.jsx
  /library          → Library.jsx
  /game-requests    → MyGameRequests.jsx
  /games/add        → GameForm.jsx           [Games.Create]
  /games/edit/:id   → GameForm.jsx           [Games.Edit]
  /games/:id        → GameDetail.jsx
  /profile          → Profile.jsx

  /admin/game-requests → AdminGameRequests.jsx  [GameRequests.Review|Approve|Reject]
  /admin/roles         → AdminRoles.jsx          [Super Admin only]
  /admin/users         → AdminUsers.jsx          [Users.View|ManageRoles]
  /admin/:type         → AdminMetadata.jsx       [Admin role]

* → redirect to /
```

---

## `Layout.jsx` — App Shell

Provides the persistent application chrome:

| Section | Description |
|:---|:---|
| **Sidebar** | Navigation links, conditional Admin section (based on permissions), user avatar + logout |
| **Header** | Page title + theme toggle (Dark/Light) |
| **Main outlet** | `<Outlet />` renders the active page component |

Navigation items are dynamically shown/hidden based on user permissions using `hasPermission()` and `isSuperAdmin()` from `useAuth()`.

---

## Pages

### `Dashboard.jsx` — `/`
- KPI stat cards: Total Games, Completed, Backlog, Hours Played
- **Recharts components:**
  - `PieChart` — Platform distribution
  - `BarChart` — Completion status breakdown
  - `BarChart` (horizontal) — Genre distribution
  - `AreaChart` — Release year timeline
- Quick-action widgets: Currently Playing, Backlog Highlights

### `Catalog.jsx` — `/catalog`
- Fetches paginated games from `GET /api/games`
- **Views:** Grid (cover cards with hover overlays) / List (tabular)
- **Filters panel:** collapsible, multi-select by genre, platform, service, developer, publisher
- Debounced search input
- Dynamic sort controls

### `Library.jsx` — `/library`
- Fetches current user's library from `GET /api/libraries`
- **Views:** Grid / List / Large Card / Gallery (cover wall)
- Filter by completion status, ownership type, platform
- Quick-update modal for playtime & status
- Remove from library confirmation dialog

### `GameDetail.jsx` — `/games/:id`
- Full hero banner + cover art
- Feature badge chips (Multiplayer, Co-op, VR, Cloud Saves, etc.)
- Screenshots lightbox gallery
- Embedded YouTube trailer player
- **"Add to Library" modal** — platform selection checkboxes
- Edit button (shown if `Games.Edit` permission)

### `GameForm.jsx` — `/games/add` & `/games/edit/:id`
3-tab form for creating/editing catalog games:
- **Tab 1 — General Info:** Title, aliases, descriptions, ratings, release dates, feature flags
- **Tab 2 — Taxonomy & Platforms:** Multi-select for platforms, services, developers, publishers, genres, tags, themes, franchise, series
- **Tab 3 — Media & Assets:** File upload inputs for cover/banner/screenshots + YouTube URL inputs

### `Login.jsx` / `Register.jsx`
- Dark theme auth forms
- Calls `login()` / `register()` from `AuthContext`
- Redirects to `/` on success

### `Profile.jsx` — `/profile`
- Displays: username, email, assigned roles, effective permissions
- Password change form (validates current password)
- Theme toggle switch (persists to localStorage)

---

## Admin Pages

### `AdminGameRequests.jsx` — `/admin/game-requests`
- Tabbed filter: Pending / Approved / Rejected
- Duplicate detection warning with matching catalog games
- Approve button: one-click ingestion into catalog
- Reject button: opens modal for feedback notes

### `AdminRoles.jsx` — `/admin/roles`
- Role list with Create / Edit / Delete controls
- Permission matrix editor: categorized permission cards with toggles
- Real-time save (debounced or on-blur)

### `AdminUsers.jsx` — `/admin/users`
- Searchable user table
- Role assignment modal with multi-select checkboxes
- Validation: user must have ≥ 1 role

### `AdminMetadata.jsx` — `/admin/:type`
- Generic CRUD table driven by `:type` URL param
- Handles all 9 taxonomy types with a single component
- Inline create/edit modal
- Confirm-before-delete dialog

---

## Services

### `services/api.js`
```javascript
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5139/api',
});

// Attach JWT on every request
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// Auto-logout on 401
api.interceptors.response.use(null, error => {
  if (error.response?.status === 401) {
    localStorage.clear();
    window.location.href = '/login';
  }
  return Promise.reject(error);
});
```

---

## CSS Design System

**File:** `src/index.css`

Uses CSS custom properties (variables) for the entire HSL gaming theme:

```css
:root {
  --color-bg-primary: hsl(220, 20%, 10%);
  --color-bg-secondary: hsl(220, 18%, 14%);
  --color-accent: hsl(260, 70%, 65%);
  --color-accent-glow: hsl(260, 80%, 70%);
  /* ... */
}

[data-theme="light"] {
  --color-bg-primary: hsl(0, 0%, 96%);
  /* ... */
}
```

Includes custom component classes (`.game-card`, `.kpi-card`, `.sidebar`, etc.) and glassmorphism effects.

---

## Navigation

← [[Backend/API Endpoints]] | [[Frontend/Auth Context]] →
