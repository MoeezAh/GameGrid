# Entity Relationship Model

tags: #database #erd #schema #relationships

## High-Level ER Diagram

```
+------------------+         +-------------------------+         +------------------+
| IdentityUser     |1-------*|   ApplicationUserRole   |*-------1| ApplicationRole  |
| (ASP.NET)        |         +-------------------------+         +------------------+
| - Id (string)    |                                                     | 1
| - UserName       |                                                     |
| - Email          |                                                     | *
+------------------+         +-------------------------+         +------------------+
        | 1                  |   ApplicationRolePermission|        |    Permission    |
        |                    | - RoleId (FK) PK           |1------1| - Id             |
        | *                  | - PermissionId (FK) PK     |        | - Name (unique)  |
+------------------+         +-------------------------+         | - Category       |
| UserLibraryEntry |                                             +------------------+
| - Id             |
| - UserId (FK)    |
| - GameId (FK)    |
| - OwnGame        |         +---------------------------+
| - Wishlist       |*-------1|           Game            |
| - Backlog        |         | - Id                      |
| - HoursPlayed    |         | - Title (indexed)         |
| - CompletionStatus|        | - Description             |
| - PersonalRating |         | - ReleaseDate             |
| - PersonalNotes  |         | - CoverImage              |
+--------+---------+         | - Banner                  |
         | *                 | - TrailerUrl              |
         |                   | - IsDeleted (soft delete) |
         | *                 | - FranchiseId (FK)        |
+------------------+         | - SeriesId (FK)           |
|UserLibraryPlatform|        +------------+--------------+
|(junction table)  |                      | *
+------------------+          Many-to-Many via junction tables:
                              ├── GamePlatforms      → Platform
                              ├── GameServices       → DigitalService
                              ├── GameDevelopers     → Developer
                              ├── GamePublishers     → Publisher
                              ├── GameGenres         → Genre
                              ├── GameTags           → Tag
                              └── GameThemes         → Theme

+------------------+
|   GameRequest    |
| - Id             |
| - GameTitle      |
| - Status (enum)  |
| - RequestedByUserId (FK → IdentityUser)
| - ReviewedByUserId (FK → IdentityUser)
| - CreatedGameId  (FK → Game, nullable)
+------------------+
```

---

## Table Inventory

| Table Name | Description | PK |
|:---|:---|:---|
| `AspNetUsers` | ASP.NET Identity users | `Id` (string/GUID) |
| `Games` | Central game catalog | `Id` (int, auto) |
| `UserLibraryEntries` | Personal library entries | `Id` (int, auto) |
| `GameRequests` | Community game submissions | `Id` (int, auto) |
| `ApplicationRoles` | Custom RBAC roles | `Id` (int, auto) |
| `Permissions` | Permission registry | `Id` (int, auto) |
| `ApplicationRolePermissions` | Role ↔ Permission assignments | Composite `(RoleId, PermissionId)` |
| `ApplicationUserRoles` | User ↔ Custom Role assignments | Composite `(UserId, RoleId)` |
| `Platforms` | Hardware platform records | `Id` (int, auto) |
| `DigitalServices` | Digital storefronts | `Id` (int, auto) |
| `Developers` | Game developer studios | `Id` (int, auto) |
| `Publishers` | Publishing houses | `Id` (int, auto) |
| `Genres` | Game genre classifications | `Id` (int, auto) |
| `Tags` | Descriptive tags | `Id` (int, auto) |
| `Themes` | Narrative theme records | `Id` (int, auto) |
| `Franchises` | Broad IP franchises | `Id` (int, auto) |
| `Series` | Sub-series groupings | `Id` (int, auto) |
| `GamePlatforms` | Game ↔ Platform junction | Composite FK |
| `GameServices` | Game ↔ DigitalService junction | Composite FK |
| `GameDevelopers` | Game ↔ Developer junction | Composite FK |
| `GamePublishers` | Game ↔ Publisher junction | Composite FK |
| `GameGenres` | Game ↔ Genre junction | Composite FK |
| `GameTags` | Game ↔ Tag junction | Composite FK |
| `GameThemes` | Game ↔ Theme junction | Composite FK |
| `UserLibraryPlatforms` | LibraryEntry ↔ Platform (user-owned) | Composite FK |
| `UserLibraryServices` | LibraryEntry ↔ DigitalService | Composite FK |

---

## Key Relationships

| Relationship | Type | Notes |
|:---|:---|:---|
| IdentityUser → UserLibraryEntry | 1:N | One user, many library entries |
| Game → UserLibraryEntry | 1:N | One game can be in many users' libraries |
| Game → Platform | N:M | Via `GamePlatforms` junction |
| Game → Genre | N:M | Via `GameGenres` junction |
| Game → Developer | N:M | Via `GameDevelopers` junction |
| Game → Franchise | N:1 | Game belongs to one optional franchise |
| Game → Series | N:1 | Game belongs to one optional series |
| UserLibraryEntry → Platform | N:M | User's **owned** platforms (subset of game platforms) |
| ApplicationRole → Permission | N:M | Via `ApplicationRolePermissions` |
| IdentityUser → ApplicationRole | N:M | Via `ApplicationUserRoles` |
| GameRequest → Game | N:1 | Approved request links to created/existing game |

---

## Navigation

← [[Frontend/Pages Reference]] | [[Database/Entities Reference]] →
