# Antigravity Game Collection System — Comprehensive Feature & Technical Specification

---

## Executive Summary & High-Level Overview

**Antigravity** is a full-stack, enterprise-grade game collection management platform designed for gamers, collectors, curators, and gaming community administrators. The system decouples the **Authoritative Central Game Catalog** from **Personal User Libraries**, empowering users to track and customize their gaming backlog, playtime, and owned platforms while maintaining a verified central database of video games.

The platform is secured by a dynamic, configurable **Role-Based Access Control (RBAC)** engine where all platform actions are controlled by discrete permissions rather than hardcoded role names.

```
+-----------------------------------------------------------------------------------+
|                               ANTIGRAVITY ECOSYSTEM                               |
+-----------------------------------------------------------------------------------+
|                                                                                   |
|  +---------------------------+                     +---------------------------+  |
|  |   Central Game Catalog    |<===================>|   Personal User Library   |  |
|  | (Authoritative Metadata)  |  "Add to Library"   |   (Ownership & Playtime)  |  |
|  +---------------------------+                     +---------------------------+  |
|               ^                                                  |                |
|               | (Approve & Ingest)                               |                |
|  +---------------------------+                                   v                |
|  |   Game Request Workflow   |<--------------------+   +-------------------+      |
|  |  (Community Contributions)|   "Submit Request"  |   | Analytics & KPIs  |      |
|  +---------------------------+                     |   | (Visual Charts)   |      |
|                                                    |   +-------------------+      |
|                                                    |                              |
|  +-------------------------------------------------+---------------------------+  |
|  |                   Generic Role-Based Access Control (RBAC)                  |  |
|  |              Users  --->  Assigned Roles  --->  Granular Permissions        |  |
|  +-----------------------------------------------------------------------------+  |
+-----------------------------------------------------------------------------------+
```

---

## System Architecture & Technology Stack

| Layer | Technologies & Frameworks | Key Responsibilities |
| :--- | :--- | :--- |
| **Frontend Client** | React 19, Vite 8, Bootstrap 5 Icons, Recharts 3, Axios, React Router v7 | Glassmorphic Dark/Light UI, dynamic permission routing, real-time analytics charts, multi-view catalog exploration. |
| **API & Security** | ASP.NET Core 10 Web API, MediatR (CQRS), FluentValidation, AutoMapper | RESTful API endpoints, dynamic permission authorization handlers, JWT bearer authentication. |
| **Business Logic** | .NET 10 Class Library (Clean Architecture Application Layer) | Commands, Queries, Domain Validation, Business Rules, Permission Services. |
| **Data & Persistence** | EF Core 10, SQLite / SQL Server, Code-First Migrations | Relational schema, soft delete query filters, many-to-many junction tables, database seeding. |
| **Storage & Assets** | ASP.NET Core Static Web Assets | Local file upload handler with format verification (covers, banners, screenshots). |

---

## User Roles & Access Control Matrix

The application employs a permission-based security architecture. Users are assigned one or more roles, and permissions are dynamically aggregated. **Super Admin** possesses an unconditional dynamic bypass across all authorization requirements.

```
                      +-------------------+
                      |   Super Admin     | (Full system bypass & role configuration)
                      +---------+---------+
                                |
      +-------------------------+-------------------------+
      |                                                   |
+-----v-----+                                       +-----v-----+
|   Admin   | (Catalog, Users, Requests, Metadata)  |  Curator  | (Catalog review & curation)
+-----+-----+                                       +-----+-----+
      |                                                   |
      +-------------------------+-------------------------+
                                |
                          +-----v-----+
                          |   User    | (Personal library, catalog view, request submissions)
                          +-----------+
```

### Permission Hierarchy Reference

| Permission Identifier | Description | Target Subsystem |
| :--- | :--- | :--- |
| `Games.View` | View games in the central catalog | Central Catalog |
| `Games.Create` | Create authoritative game catalog records directly | Central Catalog |
| `Games.Edit` | Modify metadata, taxonomy, and media of catalog games | Central Catalog |
| `Games.Delete` | Soft-delete games from the central catalog | Central Catalog |
| `Libraries.View` | View personal game library and play statistics | Personal Library |
| `Libraries.Manage` | Add games to library, edit personal rating, status, playtime, or remove | Personal Library |
| `GameRequests.Submit` | Submit new game requests for catalog ingestion | Game Requests |
| `GameRequests.ViewMine`| View personal game request status and feedback | Game Requests |
| `GameRequests.Review` | Access moderator dashboard to inspect pending requests | Game Requests |
| `GameRequests.Approve`| Approve game requests and ingest them into the catalog | Game Requests |
| `GameRequests.Reject` | Reject requests with reviewer notes | Game Requests |
| `GameRequests.Delete` | Delete game request records | Game Requests |
| `Roles.View` | View defined roles and assigned permissions | RBAC Administration |
| `Roles.Create` | Create new system roles *(Super Admin exclusive)* | RBAC Administration |
| `Roles.Edit` | Edit role names, descriptions, and assign permissions *(Super Admin exclusive)* | RBAC Administration |
| `Roles.Delete` | Deactivate/delete system roles *(Super Admin exclusive)* | RBAC Administration |
| `Users.View` | View registered user profiles and their assigned roles | User Management |
| `Users.ManageRoles` | Assign and modify roles for registered users | User Management |
| `Metadata.View` | View taxonomy master records (genres, platforms, developers, etc.) | Master Taxonomy |
| `Metadata.Manage` | Create, edit, and delete taxonomy master records | Master Taxonomy |

### Default Role Capabilities Summary

- **Super Admin**:
  - Full, unrestricted access to every platform function.
  - Exclusive authority to create, edit, deactivate roles and configure permission matrices.
  - Unconditional bypass in authorization handlers.
- **Admin**:
  - Full catalog management, metadata and taxonomy curation.
  - Reviews, approves, and rejects user game requests with duplicate checking.
  - Manages registered users and assigns standard roles.
- **Game Curator**:
  - Dedicated catalog moderation role.
  - Reviews and approves community game requests.
  - Creates and updates central catalog entries and taxonomy items.
- **Standard User**:
  - Explores the central game catalog with filters and search.
  - Manages their personal library (playtime, status, personal notes, ratings, owned platforms).
  - Submits game requests for missing catalog titles and tracks submission statuses.

---

## Detailed Functional Specifications

### 1. Analytics & KPI Dashboard (`/`)

The Dashboard provides high-level insight into personal gaming metrics through aggregate calculations and interactive Recharts visualizations.

- **KPI Cards**:
  - **Total Games**: Count of games tracked in personal library.
  - **Completed Games**: Count and percentage of completed titles.
  - **In Backlog**: Games queued for future playthroughs.
  - **Hours Played**: Total accumulated playtime across all library entries.
- **Interactive Visualizations**:
  - **Platform Distribution Chart**: Pie chart detailing library games by owned platform.
  - **Completion Status Bar Chart**: Breakdown across statuses (*Not Started, Playing, On Hold, Completed, Dropped, 100% Completed, Replaying*).
  - **Genre Distribution Chart**: Horizontal bar chart mapping favorite game genres.
  - **Release Year Timeline**: Historical distribution of library games by release year.
- **Quick Action Widgets**:
  - **Continue Playing**: Quick cards for currently active games with direct link to update play session hours.
  - **Backlog Highlights**: Immediate access to unplayed games.

---

### 2. Central Game Catalog (`/catalog`)

An authoritative repository of video games containing comprehensive metadata, taxonomy tags, and media assets.

- **Multi-View Catalog Layouts**:
  - **Grid View**: Visual cover card layout with hover overlays.
  - **List View**: Dense tabular data view displaying release dates, genres, developer, and ratings.
- **Filtering & Search Engine**:
  - Full-text search across Title, Alternate Titles, and Original Title.
  - Multi-select filters by Genre, Platform, Digital Service, Developer, and Publisher.
  - Dynamic sorting by Title, Release Date, Critic Score, and Community Rating.
- **Catalog Detail View (`/games/:id`)**:
  - High-resolution hero banner, box art, logos, and screenshots lightbox.
  - Metadata breakdown: Developer, Publisher, Franchise, Series, ESRB/PEGI ratings, Steam Deck compatibility.
  - Feature badges: Multiplayer, Co-op, Crossplay, Cloud Saves, Controller Support, VR Support.
  - Integrated YouTube video trailer embed player.
  - Direct **"Add to Library"** modal trigger.
- **Catalog Management (`/games/add`, `/games/edit/:id`)**:
  - Accessible only to users with `Games.Create` / `Games.Edit` permissions or Super Admin.
  - Comprehensive 3-tab form:
    1. *General Info*: Title, aliases, descriptions, release dates, aggregate scores, feature flags.
    2. *Taxonomy & Platforms*: Multi-select checklists for supported platforms, services, developers, publishers, genres, tags, and themes.
    3. *Media & Assets*: Upload local image files for cover and banner art; link screenshots and video URLs.

---

### 3. Personal User Library (`/library`)

A decoupled personal space where users track their relationship with games without modifying catalog data.

- **Ownership Classification**:
  - **Owned Game**: Physical or digital ownership toggle.
  - **Wishlist**: Games targeted for future purchase.
  - **Backlog**: Games acquired but awaiting playthrough.
- **Personal Play Statistics**:
  - **Completion Status**: *Not Started (0), Playing (1), On Hold (2), Completed (3), Dropped (4), 100% Completed (5), Replaying (6)*.
  - **Playtime Tracker**: Log hours played per game.
  - **Personal Rating**: 0 to 10 scale rating.
  - **Personal Notes & Dates**: Started date, completed date, last played date, and custom gameplay notes.
- **Multi-Platform Personal Ownership**:
  - Central games can support multiple platforms (*e.g., PC, PS5, Xbox Series X*).
  - Users select precisely **which platforms they own the game on** (*e.g., user owns on PC only*).
  - Central catalog platform definitions remain unaffected.
- **Library Management**:
  - Quick update modal to increment hours played and switch completion statuses.
  - One-click removal from personal library (does not delete the game from the central catalog).

---

### 4. Game Request Workflow (`/game-requests`, `/admin/game-requests`)

A community contribution system that protects central catalog integrity by routing user submissions through a curator review queue.

- **Submission Lifecycle**:
  ```
  [User Submits Request] ---> Status: Pending
                                  |
            +---------------------+---------------------+
            |                                           |
  [Curator Approves]                          [Curator Rejects]
  - Checks for duplicates                     - Provides rejection reason
  - Ingests into Central Catalog              - Status: Rejected
  - Status: Approved
  ```
- **User Submission Interface (`/game-requests`)**:
  - Submit title, approximate release year, requested platforms, descriptions, and reference links (e.g. Steam, Wikipedia, IGDB).
  - Real-time status tracker (*Pending, Approved, Rejected*) with reviewer response notes.
- **Reviewer & Admin Dashboard (`/admin/game-requests`)**:
  - Filter by pending, approved, or rejected requests.
  - **Catalog Duplicate Detection Tool**: Automatically performs similarity matching against existing catalog games to prevent duplicate catalog entries.
  - **One-Click Approval**: Ingests request parameters directly into a new catalog game or associates with an existing entry.
  - **Rejection Flow**: Prompts reviewer for feedback notes to notify the requester.

---

### 5. Role-Based Access Control (RBAC) Management (`/admin/roles`)

*Exclusive to Super Admin users.*

- **Role Administration**:
  - Create new custom roles with name, description, and active status.
  - Edit existing roles and toggle activation states.
  - Safe deletion safeguards preventing removal of default roles.
- **Visual Permission Matrix**:
  - Categorized permission cards (*Catalog, Library, Game Requests, RBAC, Users, Metadata*).
  - Interactive checkboxes to toggle granular permissions per role.
  - Instant synchronization with backend database via transactional commands.

---

### 6. User Management & Role Assignment (`/admin/users`)

*Accessible to users with `Users.View` and `Users.ManageRoles` permissions.*

- **User Directory**:
  - Searchable user table displaying Username, Email, Created Date, Active Status, and Assigned Roles.
- **Multi-Role Assignment Modal**:
  - Checkbox selection allowing users to hold multiple active roles simultaneously.
  - Enforces rule: Every user must maintain at least one assigned role.
  - User permissions automatically compute as the union of all assigned active roles.

---

### 7. Master Taxonomy & Metadata Management (`/admin/:type`)

*Accessible to users with `Metadata.View` and `Metadata.Manage` permissions.*

Provides dedicated CRUD interfaces for 9 relational metadata entities:
1. **Platforms**: Hardware systems (*PC, PlayStation 5, Xbox Series X, Nintendo Switch, Steam Deck, etc.*) with manufacturer and release year.
2. **Digital Services**: Storefronts and launchers (*Steam, Epic Games Store, GOG, PlayStation Network, Xbox Game Pass, etc.*).
3. **Developers**: Studio names, country of origin, founded year, and website.
4. **Publishers**: Publishing houses, headquarters, and web references.
5. **Genres**: Categorical classifications (*Action, RPG, Strategy, Adventure, Simulation, etc.*).
6. **Tags**: Descriptive tags (*Soulslike, Cyberpunk, Open World, Pixel Art, Story Rich, etc.*).
7. **Themes**: Narrative themes (*Sci-Fi, Fantasy, Horror, Post-Apocalyptic, Historical, etc.*).
8. **Franchises**: Broad intellectual properties (*The Witcher, Final Fantasy, Mario, The Legend of Zelda*).
9. **Series**: Sub-series groupings (*Witcher Main Trilogy, Final Fantasy VII Remake Project*).

---

### 8. User Profile & System Preferences (`/profile`)

- **Profile Details**: View username, registered email, and dynamic list of assigned roles with effective permissions.
- **Account Security**: Secure password update form with current password verification.
- **Theme Switcher**: Instant toggle between Dark Mode and Light Mode with persistent `localStorage` state.

---

## Low-Level Technical & Data Specifications

### Relational Entity-Relationship Model

```
+------------------+         +-------------------------+         +------------------+
| ApplicationUser  |1-------*|   ApplicationUserRole   |*-------1| ApplicationRole  |
+------------------+         +-------------------------+         +------------------+
        | 1                                                               | 1
        |                                                                 |
        | *                                                               | *
+------------------+         +-------------------------+         +------------------+
| UserLibraryEntry |*-------1|          Game           |1-------*|RolePermission    |
+------------------+         +-------------------------+         +------------------+
        | 1                               | 1                             | *
        |                                 |                               |
        | *                               | *                             | 1
+--------------------+       +-------------------------+         +------------------+
|UserLibraryPlatform |       |  GamePlatform / Media   |         |    Permission    |
+--------------------+       +-------------------------+         +------------------+
```

### Primary Database Entities

1. **`Permission`**:
   - `Id` (int, PK), `Name` (string, unique, indexed), `Description` (string), `Category` (string), `CreatedDate` (DateTime).
2. **`ApplicationRole`**:
   - `Id` (int, PK), `Name` (string, unique), `Description` (string), `IsActive` (bool), `CreatedDate` (DateTime), `UpdatedDate` (DateTime?).
3. **`ApplicationRolePermission`**:
   - `RoleId` (int, FK), `PermissionId` (int, FK). Composite PK `(RoleId, PermissionId)`.
4. **`ApplicationUserRole`**:
   - `UserId` (int, FK), `RoleId` (int, FK). Composite PK `(UserId, RoleId)`.
5. **`Game`** (Authoritative Catalog):
   - `Id` (int, PK), `Title` (string, indexed), `Description` (string), `ReleaseDate` (DateTime?), `CriticRating` (decimal?), `CommunityRating` (decimal?), `CoverImage` (string), `Banner` (string), `TrailerUrl` (string), `IsDeleted` (bool - soft delete).
   - Navigation: `Platforms`, `DigitalServices`, `Developers`, `Publishers`, `Genres`, `Tags`, `Themes`, `Franchise`, `Series`.
6. **`UserLibraryEntry`** (Personal Library):
   - `Id` (int, PK), `UserId` (int, FK), `GameId` (int, FK), `OwnGame` (bool), `Wishlist` (bool), `Backlog` (bool), `CompletionStatus` (int), `HoursPlayed` (decimal), `PersonalRating` (decimal?), `PersonalNotes` (string), `StartedPlayingDate` (DateTime?), `CompletedDate` (DateTime?).
   - Navigation: `Platforms` (via `UserLibraryPlatform`), `DigitalServices` (via `UserLibraryService`).
7. **`GameRequest`**:
   - `Id` (int, PK), `UserId` (int, FK), `Title` (string), `ApproximateReleaseYear` (int?), `RequestedPlatforms` (string), `ReferenceLinks` (string), `Description` (string), `Status` (int: 0=Pending, 1=Approved, 2=Rejected), `ReviewerNotes` (string), `ReviewedByUserId` (int?), `CreatedDate` (DateTime), `ReviewedDate` (DateTime?).

---

## API Endpoints Reference

### Authentication & Identity (`/api/auth`)
- `POST /api/auth/register` — Register a new user account (automatically assigned default `User` role).
- `POST /api/auth/login` — Authenticate and retrieve JWT Bearer token with roles and permissions payload.
- `GET /api/auth/me` — Retrieve current authenticated user profile and permissions.
- `POST /api/auth/change-password` — Update user password.

### RBAC & User Management (`/api/roles`, `/api/users`)
- `GET /api/roles` — Retrieve all roles and their assigned permissions *(Requires `Roles.View` or Super Admin)*.
- `GET /api/roles/permissions` — Retrieve all system permissions categorized.
- `POST /api/roles` — Create a new role with assigned permissions *(Super Admin exclusive)*.
- `PUT /api/roles/{id}` — Update role details and permission assignments *(Super Admin exclusive)*.
- `DELETE /api/roles/{id}` — Deactivate/delete a role *(Super Admin exclusive)*.
- `GET /api/users` — List registered users and their roles *(Requires `Users.View`)*.
- `PUT /api/users/{id}/roles` — Assign roles to a user *(Requires `Users.ManageRoles`)*.

### Central Catalog (`/api/games`)
- `GET /api/games` — Paginated, filtered catalog search *(Public / `Games.View`)*.
- `GET /api/games/{id}` — Detailed catalog game view.
- `POST /api/games` — Create a new authoritative catalog game *(Requires `Games.Create`)*.
- `PUT /api/games/{id}` — Update catalog game metadata *(Requires `Games.Edit`)*.
- `DELETE /api/games/{id}` — Soft delete a game from the catalog *(Requires `Games.Delete`)*.
- `POST /api/games/upload` — Upload media asset file (multipart form data).

### Personal Library (`/api/libraries`)
- `GET /api/libraries` — Retrieve current user's personal game library *(Requires `Libraries.View`)*.
- `GET /api/libraries/{id}` — Retrieve personal library entry by ID.
- `POST /api/libraries` — Add catalog game to personal library with platform selections *(Requires `Libraries.Manage`)*.
- `PUT /api/libraries/{id}` — Update playtime, completion status, rating, or notes *(Requires `Libraries.Manage`)*.
- `DELETE /api/libraries/{id}` — Remove game entry from personal library *(Requires `Libraries.Manage`)*.

### Game Requests (`/api/gamerequests`)
- `GET /api/gamerequests/my` — List requests submitted by the authenticated user *(Requires `GameRequests.ViewMine`)*.
- `POST /api/gamerequests` — Submit a new game request *(Requires `GameRequests.Submit`)*.
- `GET /api/gamerequests` — List all requests across users *(Requires `GameRequests.Review`)*.
- `GET /api/gamerequests/check-duplicate` — Check title similarity against catalog *(Requires `GameRequests.Review`)*.
- `POST /api/gamerequests/{id}/approve` — Approve request and create/link catalog entry *(Requires `GameRequests.Approve`)*.
- `POST /api/gamerequests/{id}/reject` — Reject request with feedback notes *(Requires `GameRequests.Reject`)*.

### Master Metadata & Analytics (`/api/metadata`, `/api/dashboard`)
- `GET /api/metadata/{category}/list` — Lookup select lists for forms.
- `GET /api/metadata/{category}` — Paginated table view for metadata management.
- `POST /api/metadata/{category}` — Create taxonomy record *(Requires `Metadata.Manage`)*.
- `PUT /api/metadata/{category}/{id}` — Update taxonomy record *(Requires `Metadata.Manage`)*.
- `DELETE /api/metadata/{category}/{id}` — Delete taxonomy record *(Requires `Metadata.Manage`)*.
- `GET /api/dashboard/stats` — Aggregated KPI metrics and chart datasets for authenticated user.

---

## Security & Reliability Standards

1. **Clean Architecture Isolation**:
   - Domain layer has zero external dependencies.
   - CQRS pattern separates read operations (Queries) from state-mutating operations (Commands).
2. **Zero Hardcoded Role Dependencies**:
   - Business actions evaluate discrete permissions via `[HasPermission(...)]`.
   - Adding new roles requires no code changes or recompilations.
3. **Data Integrity & Soft Deletes**:
   - EF Core global query filters prevent accidental data destruction by marking catalog items as `IsDeleted = true`.
4. **Client-Side Resilience**:
   - Axios request and response interceptors attach JWT bearer tokens and handle session expirations seamlessly.
   - Route guards prevent unauthorized component rendering before API round-trips.
