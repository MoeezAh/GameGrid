# Pages Reference

tags: #frontend #pages #ui #features

A quick reference for all pages, their routes, permissions, and key features.

---

## Public Pages

| Page | Route | File | Description |
|:---|:---|:---|:---|
| Login | `/login` | `pages/Login.jsx` | Credential form, auto-redirect if logged in |
| Register | `/register` | `pages/Register.jsx` | Account creation, auto-assigns User role |

---

## Authenticated Pages

### Dashboard — `/`
**File:** `pages/Dashboard.jsx`
**Requires:** Login

**Features:**
- 4 KPI stat cards (Total Games, Completed, Backlog, Hours Played)
- 4 interactive Recharts charts:
  - Platform distribution (PieChart)
  - Completion status breakdown (BarChart)
  - Genre distribution (horizontal BarChart)
  - Release year timeline (AreaChart)
- "Continue Playing" quick-action widget
- "Backlog Highlights" quick-action widget

---

### Central Catalog — `/catalog`
**File:** `pages/Catalog.jsx`
**Requires:** Login (+ `Games.View` on backend)

**Features:**
- Grid view (cover art cards with hover overlay showing quick info)
- List view (dense tabular layout)
- Collapsible filters sidebar:
  - Full-text search
  - Platform multi-select
  - Genre multi-select
  - Digital service multi-select
  - Developer / Publisher multi-select
- Sort by: Title, Release Date, Critic Score, Community Rating
- Pagination controls
- "Add Game to Catalog" button (only visible with `Games.Create`)

---

### Game Detail — `/games/:id`
**File:** `pages/GameDetail.jsx`
**Requires:** Login

**Features:**
- Full-width hero banner
- Cover art + game logo
- Feature badges (Multiplayer, Co-op, VR, Crossplay, Cloud Saves, Controller, Steam Deck)
- Ratings display (Critic, Community, Metacritic, OpenCritic)
- ESRB/PEGI content rating
- Developer, Publisher, Franchise, Series, Release dates
- Screenshots lightbox gallery
- YouTube trailer embed player
- "Add to Library" button → opens platform/service selection modal
- "Edit Game" button (only with `Games.Edit` or Super Admin)

---

### Game Form — `/games/add` & `/games/edit/:id`
**File:** `pages/GameForm.jsx`
**Requires:** `Games.Create` (add) | `Games.Edit` (edit) | Super Admin (either)

**Tab 1 — General Info:**
- Title, Alternate Titles, Original Title
- Description, Notes
- Release Date, Original Release Date, Early Access Date
- Critic Rating (0–10), Community Rating (0–10)
- Metacritic Score, OpenCritic Score
- ESRB Rating, PEGI Rating
- Achievement Count, DLC Count, Expansion Count
- Steam Deck Compatibility
- Feature flags: Multiplayer, Co-op, VR, Crossplay, Cloud Saves, Controller Support

**Tab 2 — Taxonomy & Platforms:**
- Platforms (multi-select checklist from `/api/metadata/platforms/list`)
- Digital Services (multi-select)
- Developers (multi-select)
- Publishers (multi-select)
- Genres (multi-select)
- Tags (multi-select)
- Themes (multi-select)
- Franchise (single select)
- Series (single select)

**Tab 3 — Media & Assets:**
- Cover Image upload (file input → POST `/api/games/upload`)
- Banner upload
- Box Art upload
- Logo upload
- Screenshots (multiple file uploads)
- Trailer URL (YouTube)
- Gameplay URL (YouTube)

---

### Personal Library — `/library`
**File:** `pages/Library.jsx`
**Requires:** `Libraries.View`

**Features:**
- 4 view modes: Grid / List / Card / Gallery (cover wall)
- Filter by: Completion Status, Ownership (Own/Wishlist/Backlog), Platform
- Search by game title
- Quick-update modal: increment hours played, change completion status
- Full-edit modal: all ownership + play + rating + notes fields
- Remove from library (with confirmation)
- "Add to Library" from catalog flow

---

### My Game Requests — `/game-requests`
**File:** `pages/GameRequests/MyGameRequests.jsx`
**Requires:** `GameRequests.Submit` + `GameRequests.ViewMine`

**Features:**
- Submit new game request form
- Status list: Pending (yellow) / Approved (green) / Rejected (red)
- Reviewer notes shown on rejected requests

---

### Profile — `/profile`
**File:** `pages/Profile.jsx`
**Requires:** Login

**Features:**
- Display username, email, joined date
- Assigned roles list
- Effective permissions list (categorized)
- Change password form
- Dark/Light theme toggle (persists to localStorage)

---

## Admin Pages

### Admin Game Requests — `/admin/game-requests`
**File:** `pages/Admin/AdminGameRequests.jsx`
**Requires:** Any of: `GameRequests.Review`, `GameRequests.Approve`, `GameRequests.Reject`

**Features:**
- Status tabs: All / Pending / Approved / Rejected
- Duplicate detection: "⚠️ Similar games found" warning with matching catalog entries
- Approve button → ingests request as new catalog game (or links to existing)
- Reject button → modal with required reviewer notes field

---

### Admin Roles — `/admin/roles`
**File:** `pages/Admin/AdminRoles.jsx`
**Requires:** Super Admin only

**Features:**
- Role list with active/inactive badge
- Create new role modal
- Edit role modal (name, description, active toggle)
- Visual permission matrix with category grouping
- Per-permission toggle checkboxes
- Immediate save to backend

---

### Admin Users — `/admin/users`
**File:** `pages/Admin/AdminUsers.jsx`
**Requires:** `Users.View` | `Users.ManageRoles`

**Features:**
- Searchable, sortable user table
- Columns: Username, Email, Created Date, Status, Roles
- Role assignment modal: multi-select role checkboxes
- Enforces minimum 1 role requirement

---

### Admin Metadata — `/admin/:type`
**File:** `pages/Admin/AdminMetadata.jsx`
**Requires:** Admin role or Super Admin

**Supported types:**
`platforms` | `services` | `developers` | `publishers` | `genres` | `tags` | `themes` | `franchises` | `series`

**Features:**
- Paginated table view
- "Create New" button → modal form
- "Edit" inline → modal form
- "Delete" with confirmation dialog
- Type-specific form fields (e.g. Developers have Country, FoundedYear, Website)

---

## Navigation

← [[Frontend/Route Guards]] | [[Database/Entity Relationship Model]] →
