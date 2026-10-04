# Seeded Data

tags: #database #seeding #defaults #credentials

## Overview

`GameDbContextSeed.SeedAsync()` runs automatically on every application startup via `Program.cs`. It is **idempotent** — it checks for existing records before inserting and is safe to run multiple times.

**File:** `backend/GameCollection.Infrastructure/Data/GameDbContextSeed.cs`

---

## Seeded Permissions (20 total)

| # | Name | Category |
|:---|:---|:---|
| 1 | `Games.View` | Catalog |
| 2 | `Games.Create` | Catalog |
| 3 | `Games.Edit` | Catalog |
| 4 | `Games.Delete` | Catalog |
| 5 | `Libraries.View` | Library |
| 6 | `Libraries.Manage` | Library |
| 7 | `GameRequests.Submit` | Game Requests |
| 8 | `GameRequests.ViewMine` | Game Requests |
| 9 | `GameRequests.Review` | Game Requests |
| 10 | `GameRequests.Approve` | Game Requests |
| 11 | `GameRequests.Reject` | Game Requests |
| 12 | `GameRequests.Delete` | Game Requests |
| 13 | `Roles.View` | RBAC |
| 14 | `Roles.Create` | RBAC |
| 15 | `Roles.Edit` | RBAC |
| 16 | `Roles.Delete` | RBAC |
| 17 | `Users.View` | User Management |
| 18 | `Users.ManageRoles` | User Management |
| 19 | `Metadata.View` | Master Taxonomy |
| 20 | `Metadata.Manage` | Master Taxonomy |

---

## Seeded Roles

| Role Name | IsActive | Permissions Assigned |
|:---|:---:|:---|
| **Super Admin** | ✅ | All 20 permissions + unconditional bypass |
| **Admin** | ✅ | Everything except `Roles.Create/Edit/Delete` |
| **Game Curator** | ✅ | `Games.*`, `Libraries.*`, `GameRequests.*`, `Metadata.*`, not `Roles.*`/`Users.*` |
| **User** | ✅ | `Games.View`, `Libraries.*`, `GameRequests.Submit`, `GameRequests.ViewMine` |

---

## Seeded Users

| Username | Email | Password | Roles |
|:---|:---|:---|:---|
| `admin` | `admin@gamecollection.com` | `Admin123!` | Super Admin, Admin |
| `user` | `user@gamecollection.com` | `User123!` | User |

> [!CAUTION]
> **Change these passwords immediately** in any non-local/production deployment.

---

## Seeded Taxonomy Data (Sample)

### Platforms
- PC (Windows), PlayStation 5, Xbox Series X, Nintendo Switch, Steam Deck
- PlayStation 4, Xbox One, iOS, Android, macOS

### Digital Services
- Steam, Epic Games Store, GOG Galaxy, PlayStation Network, Xbox Game Pass
- Nintendo eShop, EA App, Battle.net, Ubisoft Connect, Humble Bundle

### Genres
- Action, RPG, Strategy, Simulation, Adventure, Puzzle, Horror, Sports
- Racing, Fighting, Platformer, Shooter, MOBA, Battle Royale, Survival

### Developers (sample)
- CD Projekt Red, FromSoftware, Nintendo, Naughty Dog, Bethesda Game Studios
- Valve Corporation, Riot Games, Epic Games, Insomniac Games, Rockstar Games

### Publishers (sample)
- CD Projekt, Bandai Namco, Nintendo, Sony Interactive Entertainment
- Microsoft, EA, Ubisoft, Activision, 2K Games

---

## Sample Catalog Games

The seeder includes a small set of example games to demonstrate the catalog:

| Title | Developer | Genres | Platforms |
|:---|:---|:---|:---|
| The Witcher 3: Wild Hunt | CD Projekt Red | RPG, Open World | PC, PS5, Xbox Series X, Nintendo Switch |
| Elden Ring | FromSoftware | Action, RPG, Soulslike | PC, PS5, Xbox Series X |
| Hollow Knight | Team Cherry | Platformer, Metroidvania | PC, Nintendo Switch |
| Cyberpunk 2077 | CD Projekt Red | RPG, Action, Sci-Fi | PC, PS5, Xbox Series X |

> [!NOTE]
> Sample game data may vary. The seeder focuses on demonstrating a working catalog, not comprehensive coverage.

---

## Adding New Seed Data

To add new seeded taxonomy items or test games, edit `GameDbContextSeed.cs` and re-run the application or run:

```bash
dotnet run --project GameCollection.API
```

The seeder will insert only records that don't already exist.

---

## Navigation

← [[Database/Entities Reference]] | [[Configuration/Backend Settings]] →
