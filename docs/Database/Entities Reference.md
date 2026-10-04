# Entities Reference

tags: #database #entities #schema #fields

Full field-level documentation for all domain entities.

---

## `Game` — Central Catalog

| Field | Type | Nullable | Notes |
|:---|:---|:---:|:---|
| `Id` | int | ❌ | Auto-increment PK |
| `Title` | string | ❌ | Indexed for search |
| `AlternateTitles` | string | ✅ | Pipe-delimited alternate names |
| `OriginalTitle` | string | ✅ | Original language title |
| `Description` | string | ✅ | Long-form game description |
| `Notes` | string | ✅ | Internal curator notes |
| `CommunityRating` | double | ✅ | 0.0 – 10.0 |
| `CriticRating` | double | ✅ | 0.0 – 10.0 |
| `MetacriticScore` | int | ✅ | 0 – 100 |
| `OpenCriticScore` | int | ✅ | 0 – 100 |
| `ReleaseDate` | DateTimeOffset | ✅ | Global release date |
| `OriginalReleaseDate` | DateTimeOffset | ✅ | First platform release |
| `EarlyAccessDate` | DateTimeOffset | ✅ | Early access start |
| `CoverImage` | string | ✅ | Relative URL `/uploads/...` |
| `BoxArt` | string | ✅ | |
| `Banner` | string | ✅ | Wide hero banner |
| `Logo` | string | ✅ | Transparent logo |
| `Screenshots` | string | ✅ | JSON array of URLs |
| `Artwork` | string | ✅ | |
| `FanArt` | string | ✅ | |
| `TrailerUrl` | string | ✅ | YouTube URL |
| `GameplayUrl` | string | ✅ | YouTube URL |
| `YoutubeLinks` | string | ✅ | Additional YT links |
| `EsrbRating` | string | ✅ | E, E10+, T, M, AO, RP |
| `PegiRating` | string | ✅ | 3, 7, 12, 16, 18 |
| `MultiplayerSupport` | bool | ❌ | Default: false |
| `CoopSupport` | bool | ❌ | |
| `VrSupport` | bool | ❌ | |
| `CrossplaySupport` | bool | ❌ | |
| `CloudSaveSupport` | bool | ❌ | |
| `ControllerSupport` | bool | ❌ | |
| `SteamDeckCompatibility` | string | ✅ | Verified/Playable/Unsupported/Unknown |
| `AchievementCount` | int | ❌ | Default: 0 |
| `DlcCount` | int | ❌ | Default: 0 |
| `ExpansionCount` | int | ❌ | Default: 0 |
| `FranchiseId` | int | ✅ | FK |
| `SeriesId` | int | ✅ | FK |
| `IsDeleted` | bool | ❌ | Soft delete (inherited) |
| `CreatedAt` | DateTimeOffset | ❌ | Auto-set (inherited) |
| `UpdatedAt` | DateTimeOffset | ✅ | Auto-set on update (inherited) |

---

## `UserLibraryEntry` — Personal Library

| Field | Type | Nullable | Notes |
|:---|:---|:---:|:---|
| `Id` | int | ❌ | PK |
| `UserId` | string | ❌ | FK to IdentityUser |
| `GameId` | int | ❌ | FK to Game |
| `OwnGame` | bool | ❌ | Default: true |
| `Wishlist` | bool | ❌ | |
| `Backlog` | bool | ❌ | |
| `PhysicalCopy` | bool | ❌ | |
| `DigitalCopy` | bool | ❌ | |
| `CollectorsEdition` | bool | ❌ | |
| `SpecialEdition` | bool | ❌ | |
| `PurchaseDate` | DateTimeOffset | ✅ | |
| `PurchasePrice` | decimal | ✅ | |
| `Currency` | string | ✅ | e.g. "USD", "GBP" |
| `StorePurchasedFrom` | string | ✅ | e.g. "Steam", "Amazon" |
| `PurchaseRegion` | string | ✅ | |
| `ReceiptReference` | string | ✅ | Order ID, etc. |
| `Gifted` | bool | ❌ | |
| `StartedPlayingDate` | DateTimeOffset | ✅ | |
| `CompletedDate` | DateTimeOffset | ✅ | |
| `LastPlayedDate` | DateTimeOffset | ✅ | |
| `HoursPlayed` | double | ❌ | Default: 0 |
| `CompletionStatus` | CompletionStatus (enum) | ❌ | Default: NotStarted (0) |
| `PersonalRating` | double | ✅ | 0.0 – 10.0 |
| `PersonalNotes` | string | ✅ | Free-text notes |

---

## `GameRequest` — Community Submission

| Field | Type | Nullable | Notes |
|:---|:---|:---:|:---|
| `Id` | int | ❌ | PK |
| `GameTitle` | string | ❌ | Requested game name |
| `ApproximateReleaseYear` | string | ✅ | e.g. "2024" or "2025-2026" |
| `Platforms` | string | ✅ | Comma-delimited platform names |
| `Links` | string | ✅ | Reference URLs (Steam, IGDB, Wikipedia) |
| `AdditionalInformation` | string | ✅ | Extra context from requester |
| `Status` | GameRequestStatus | ❌ | 0=Pending, 1=Approved, 2=Rejected |
| `ReviewNotes` | string | ✅ | Reviewer's rejection/approval notes |
| `RequestedByUserId` | string | ❌ | FK to IdentityUser |
| `ReviewedByUserId` | string | ✅ | FK to IdentityUser (reviewer) |
| `ReviewedAt` | DateTimeOffset | ✅ | |
| `CreatedGameId` | int | ✅ | FK to Game (set on approval) |

---

## `ApplicationRole` — RBAC Role

| Field | Type | Nullable | Notes |
|:---|:---|:---:|:---|
| `Id` | int | ❌ | PK |
| `Name` | string | ❌ | Unique role name |
| `Description` | string | ✅ | |
| `IsActive` | bool | ❌ | Inactive roles are ignored in permission checks |
| `CreatedAt` | DateTimeOffset | ❌ | |
| `UpdatedAt` | DateTimeOffset | ✅ | |

---

## `Permission` — Permission Registry

| Field | Type | Nullable | Notes |
|:---|:---|:---:|:---|
| `Id` | int | ❌ | PK |
| `Name` | string | ❌ | Unique e.g. `"Games.Create"` |
| `Description` | string | ✅ | Human-readable description |
| `Category` | string | ✅ | Group: Catalog, Library, etc. |

---

## Taxonomy Entities

### `Platform`

| Field | Type | Notes |
|:---|:---|:---|
| `Id` | int | PK |
| `Name` | string | e.g. "PlayStation 5" |
| `Manufacturer` | string? | e.g. "Sony" |
| `ReleaseYear` | int? | e.g. 2020 |

### `DigitalService`

| Field | Type | Notes |
|:---|:---|:---|
| `Id` | int | PK |
| `Name` | string | e.g. "Steam" |
| `Url` | string? | Service homepage |

### `Developer`

| Field | Type | Notes |
|:---|:---|:---|
| `Id` | int | PK |
| `Name` | string | Studio name |
| `Country` | string? | Country of origin |
| `FoundedYear` | int? | |
| `Website` | string? | |

### `Publisher` — same fields as Developer

### `Genre`, `Tag`, `Theme`, `Franchise`, `Series`

| Field | Type | Notes |
|:---|:---|:---|
| `Id` | int | PK |
| `Name` | string | |
| `Description` | string? | (not on Tag) |

---

## Navigation

← [[Database/Entity Relationship Model]] | [[Database/Seeded Data]] →
