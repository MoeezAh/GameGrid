using System;
using System.Collections.Generic;
using GameCollection.Application.DTOs.Game;
using GameCollection.Domain.Enums;

namespace GameCollection.Application.DTOs.Library;

public class UserLibraryEntryDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public int GameId { get; set; }

    // Catalog Game Overview
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
    public string? Banner { get; set; }
    public string? BoxArt { get; set; }
    public string? Logo { get; set; }
    public string? Screenshots { get; set; }
    public string? TrailerUrl { get; set; }
    public string? YoutubeLinks { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
    public double? CriticRating { get; set; }
    public double? CommunityRating { get; set; }
    public int? MetacriticScore { get; set; }
    public string? EsrbRating { get; set; }
    public string? PegiRating { get; set; }

    // Catalog Relationships
    public List<MetadataItemDto> CatalogPlatforms { get; set; } = new();
    public List<MetadataItemDto> CatalogServices { get; set; } = new();
    public List<MetadataItemDto> Developers { get; set; } = new();
    public List<MetadataItemDto> Publishers { get; set; } = new();
    public List<MetadataItemDto> Genres { get; set; } = new();
    public List<MetadataItemDto> Tags { get; set; } = new();
    public List<MetadataItemDto> Themes { get; set; } = new();
    public string? FranchiseName { get; set; }
    public string? SeriesName { get; set; }

    // Personal Ownership
    public bool OwnGame { get; set; }
    public bool Wishlist { get; set; }
    public bool Backlog { get; set; }
    public bool PhysicalCopy { get; set; }
    public bool DigitalCopy { get; set; }
    public bool CollectorsEdition { get; set; }
    public bool SpecialEdition { get; set; }

    // Personal Purchase
    public DateTimeOffset? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Currency { get; set; }
    public string? StorePurchasedFrom { get; set; }
    public string? PurchaseRegion { get; set; }
    public string? ReceiptReference { get; set; }
    public bool Gifted { get; set; }

    // Personal Play & Rating
    public DateTimeOffset? StartedPlayingDate { get; set; }
    public DateTimeOffset? CompletedDate { get; set; }
    public DateTimeOffset? LastPlayedDate { get; set; }
    public double HoursPlayed { get; set; }
    public CompletionStatus CompletionStatus { get; set; }
    public double? PersonalRating { get; set; }
    public string? PersonalNotes { get; set; }

    // User Selected Owned Platforms & Services
    public List<MetadataItemDto> UserPlatforms { get; set; } = new();
    public List<MetadataItemDto> UserServices { get; set; } = new();
}

public class UserLibraryListDto
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public string Title { get; set; } = null!;
    public string? CoverImage { get; set; }
    public CompletionStatus CompletionStatus { get; set; }
    public bool Wishlist { get; set; }
    public bool Backlog { get; set; }
    public bool OwnGame { get; set; }
    public double HoursPlayed { get; set; }
    public double? PersonalRating { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
    public DateTimeOffset? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }

    // User's owned platforms (or fallback to catalog platforms)
    public List<string> Platforms { get; set; } = new();
    public List<string> Genres { get; set; } = new();
    public List<string> Services { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public class AddGameToLibraryDto
{
    public int GameId { get; set; }
    public List<int> PlatformIds { get; set; } = new();
    public List<int>? DigitalServiceIds { get; set; } = new();

    public bool OwnGame { get; set; } = true;
    public bool Wishlist { get; set; }
    public bool Backlog { get; set; }
    public bool PhysicalCopy { get; set; }
    public bool DigitalCopy { get; set; }
    public bool CollectorsEdition { get; set; }
    public bool SpecialEdition { get; set; }

    public DateTimeOffset? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Currency { get; set; }
    public string? StorePurchasedFrom { get; set; }
    public string? PurchaseRegion { get; set; }
    public string? ReceiptReference { get; set; }
    public bool Gifted { get; set; }

    public DateTimeOffset? StartedPlayingDate { get; set; }
    public DateTimeOffset? CompletedDate { get; set; }
    public DateTimeOffset? LastPlayedDate { get; set; }
    public double HoursPlayed { get; set; }
    public CompletionStatus CompletionStatus { get; set; } = CompletionStatus.NotStarted;

    public double? PersonalRating { get; set; }
    public string? PersonalNotes { get; set; }
}

public class UpdateLibraryEntryDto
{
    public List<int> PlatformIds { get; set; } = new();
    public List<int>? DigitalServiceIds { get; set; } = new();

    public bool OwnGame { get; set; }
    public bool Wishlist { get; set; }
    public bool Backlog { get; set; }
    public bool PhysicalCopy { get; set; }
    public bool DigitalCopy { get; set; }
    public bool CollectorsEdition { get; set; }
    public bool SpecialEdition { get; set; }

    public DateTimeOffset? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Currency { get; set; }
    public string? StorePurchasedFrom { get; set; }
    public string? PurchaseRegion { get; set; }
    public string? ReceiptReference { get; set; }
    public bool Gifted { get; set; }

    public DateTimeOffset? StartedPlayingDate { get; set; }
    public DateTimeOffset? CompletedDate { get; set; }
    public DateTimeOffset? LastPlayedDate { get; set; }
    public double HoursPlayed { get; set; }
    public CompletionStatus CompletionStatus { get; set; }

    public double? PersonalRating { get; set; }
    public string? PersonalNotes { get; set; }
}
