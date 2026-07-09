using System;
using System.Collections.Generic;
using GameCollection.Domain.Common;
using GameCollection.Domain.Enums;

namespace GameCollection.Domain.Entities;

public class Game : BaseAuditableEntity
{
    // Basic Information
    public string Title { get; set; } = null!;
    public string? AlternateTitles { get; set; }
    public string? OriginalTitle { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? PersonalNotes { get; set; }

    // Ownership Information
    public bool OwnGame { get; set; }
    public bool Wishlist { get; set; }
    public bool Backlog { get; set; }
    public bool PhysicalCopy { get; set; }
    public bool DigitalCopy { get; set; }
    public bool CollectorsEdition { get; set; }
    public bool SpecialEdition { get; set; }

    // Purchase Information
    public DateTimeOffset? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Currency { get; set; }
    public string? StorePurchasedFrom { get; set; }
    public string? PurchaseRegion { get; set; }
    public string? ReceiptReference { get; set; }
    public bool Gifted { get; set; }

    // Play Information
    public DateTimeOffset? StartedPlayingDate { get; set; }
    public DateTimeOffset? CompletedDate { get; set; }
    public DateTimeOffset? LastPlayedDate { get; set; }
    public double HoursPlayed { get; set; }
    public CompletionStatus CompletionStatus { get; set; } = CompletionStatus.NotStarted;

    // Rating Information
    public double? PersonalRating { get; set; }
    public double? CommunityRating { get; set; }
    public double? CriticRating { get; set; }

    // Release Information
    public DateTimeOffset? ReleaseDate { get; set; }
    public DateTimeOffset? OriginalReleaseDate { get; set; }
    public DateTimeOffset? EarlyAccessDate { get; set; }

    // Media Management
    public string? CoverImage { get; set; }
    public string? BoxArt { get; set; }
    public string? Banner { get; set; }
    public string? Logo { get; set; }
    
    // Comma-separated screenshots or JSON string. Comma-separated is easy to store and split.
    public string? Screenshots { get; set; } 
    public string? Artwork { get; set; }
    public string? FanArt { get; set; }
    public string? TrailerUrl { get; set; }
    public string? GameplayUrl { get; set; }
    public string? YoutubeLinks { get; set; }

    // Additional Collection Data
    public string? EsrbRating { get; set; }
    public string? PegiRating { get; set; }
    public int? MetacriticScore { get; set; }
    public int? OpenCriticScore { get; set; }
    public bool MultiplayerSupport { get; set; }
    public bool CoopSupport { get; set; }
    public bool VrSupport { get; set; }
    public bool CrossplaySupport { get; set; }
    public bool CloudSaveSupport { get; set; }
    public bool ControllerSupport { get; set; }
    public string? SteamDeckCompatibility { get; set; } // e.g. Verified, Playable, Unsupported, Unknown
    public int AchievementCount { get; set; }
    public int DlcCount { get; set; }
    public int ExpansionCount { get; set; }

    // Multi-user identification
    public string UserId { get; set; } = null!;

    // Configurable Relationships
    public int? FranchiseId { get; set; }
    public Franchise? Franchise { get; set; }

    public int? SeriesId { get; set; }
    public Series? Series { get; set; }

    public ICollection<Developer> Developers { get; set; } = new List<Developer>();
    public ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<Theme> Themes { get; set; } = new List<Theme>();
    public ICollection<Platform> Platforms { get; set; } = new List<Platform>();
    public ICollection<DigitalService> DigitalServices { get; set; } = new List<DigitalService>();
}
