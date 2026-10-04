# Domain Layer

tags: #backend #domain #entities #ddd

## Overview

The Domain layer (`GameCollection.Domain`) is the **innermost ring** of the Clean Architecture. It has **zero external dependencies** — no NuGet packages, no framework references, just pure C# models.

---

## Base Class: `BaseAuditableEntity`

All entities inherit from this base class located in `Domain/Common/`:

```csharp
public abstract class BaseAuditableEntity
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;  // Soft delete flag
}
```

The EF Core global query filter in `GameDbContext` automatically excludes `IsDeleted = true` records from all queries.

---

## Core Entities

### `Game` — Central Catalog Entry

```csharp
// File: Entities/Game.cs
public class Game : BaseAuditableEntity
{
    // Identity
    string Title              // required, indexed
    string? AlternateTitles   // pipe-delimited alternate names
    string? OriginalTitle
    string? Description
    string? Notes

    // Ratings
    double? CommunityRating
    double? CriticRating
    int? MetacriticScore
    int? OpenCriticScore

    // Release
    DateTimeOffset? ReleaseDate
    DateTimeOffset? OriginalReleaseDate
    DateTimeOffset? EarlyAccessDate

    // Media
    string? CoverImage        // relative URL: /uploads/<file>
    string? BoxArt
    string? Banner
    string? Logo
    string? Screenshots       // JSON array of URLs
    string? TrailerUrl        // YouTube embed URL
    string? GameplayUrl

    // Feature Flags
    bool MultiplayerSupport
    bool CoopSupport
    bool VrSupport
    bool CrossplaySupport
    bool CloudSaveSupport
    bool ControllerSupport
    string? SteamDeckCompatibility   // Verified | Playable | Unsupported | Unknown
    string? EsrbRating
    string? PegiRating
    int AchievementCount
    int DlcCount
    int ExpansionCount

    // Taxonomy relationships (many-to-many)
    ICollection<Developer> Developers
    ICollection<Publisher> Publishers
    ICollection<Genre> Genres
    ICollection<Tag> Tags
    ICollection<Theme> Themes
    ICollection<Platform> Platforms
    ICollection<DigitalService> DigitalServices

    // FK relationships
    int? FranchiseId → Franchise
    int? SeriesId    → Series

    // Reverse navigation
    ICollection<UserLibraryEntry> LibraryEntries
}
```

### `UserLibraryEntry` — Personal Library Record

```csharp
// File: Entities/UserLibraryEntry.cs
public class UserLibraryEntry : BaseAuditableEntity
{
    string UserId             // FK to ASP.NET Identity user
    int GameId                // FK to Game

    // Ownership
    bool OwnGame = true
    bool Wishlist
    bool Backlog
    bool PhysicalCopy
    bool DigitalCopy
    bool CollectorsEdition
    bool SpecialEdition

    // Purchase tracking
    DateTimeOffset? PurchaseDate
    decimal? PurchasePrice
    string? Currency
    string? StorePurchasedFrom
    string? PurchaseRegion
    bool Gifted

    // Play tracking
    DateTimeOffset? StartedPlayingDate
    DateTimeOffset? CompletedDate
    DateTimeOffset? LastPlayedDate
    double HoursPlayed
    CompletionStatus CompletionStatus = NotStarted

    // Personal metadata
    double? PersonalRating    // 0.0 – 10.0
    string? PersonalNotes

    // User's owned platforms/services (subset of what the game supports)
    ICollection<Platform> Platforms
    ICollection<DigitalService> DigitalServices
}
```

### `GameRequest` — Community Submission

```csharp
// File: Entities/GameRequest.cs
public class GameRequest : BaseAuditableEntity
{
    string GameTitle
    string? ApproximateReleaseYear
    string? Platforms              // comma-delimited
    string? Links                  // reference URLs (Steam, IGDB, Wikipedia)
    string? AdditionalInformation

    GameRequestStatus Status = Pending
    string? ReviewNotes            // reviewer's feedback on rejection

    string RequestedByUserId
    string? ReviewedByUserId
    DateTimeOffset? ReviewedAt

    int? CreatedGameId             // set when approved → links to created Game
    Game? CreatedGame
}
```

### `ApplicationRole` — Custom RBAC Role

```csharp
public class ApplicationRole : BaseAuditableEntity
{
    string Name                    // unique role name
    string? Description
    bool IsActive = true
    ICollection<ApplicationRolePermission> RolePermissions
}
```

### `Permission` — Individual Permission Record

```csharp
public class Permission : BaseAuditableEntity
{
    string Name                    // unique identifier e.g. "Games.Create"
    string? Description
    string? Category               // "Catalog", "Library", "GameRequests", etc.
}
```

---

## Taxonomy Entities

| Entity | Key Fields |
|:---|:---|
| `Platform` | Name, Manufacturer, ReleaseYear |
| `DigitalService` | Name, Url |
| `Developer` | Name, Country, FoundedYear, Website |
| `Publisher` | Name, Headquarters, Website |
| `Genre` | Name, Description |
| `Tag` | Name |
| `Theme` | Name, Description |
| `Franchise` | Name, Description |
| `Series` | Name, Description |

---

## Enums

### `CompletionStatus`

```csharp
public enum CompletionStatus
{
    NotStarted = 0,
    Playing = 1,
    OnHold = 2,
    Completed = 3,
    Dropped = 4,
    HundredPercentCompleted = 5,
    Replaying = 6
}
```

### `GameRequestStatus`

```csharp
public enum GameRequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
```

---

## Domain Interfaces

### `IRepository<T>`

```csharp
public interface IRepository<T> where T : BaseAuditableEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);          // Sets IsDeleted = true (soft delete)
}
```

### `IUnitOfWork`

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

---

## Navigation

← [[Security/Permission Reference]] | [[Backend/Application Layer]] →
