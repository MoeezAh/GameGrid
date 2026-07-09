using System;
using System.Collections.Generic;
using GameCollection.Domain.Enums;

namespace GameCollection.Application.DTOs.Game;

public class GameDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? AlternateTitles { get; set; }
    public string? OriginalTitle { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? PersonalNotes { get; set; }

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
    public double? CommunityRating { get; set; }
    public double? CriticRating { get; set; }

    public DateTimeOffset? ReleaseDate { get; set; }
    public DateTimeOffset? OriginalReleaseDate { get; set; }
    public DateTimeOffset? EarlyAccessDate { get; set; }

    public string? CoverImage { get; set; }
    public string? BoxArt { get; set; }
    public string? Banner { get; set; }
    public string? Logo { get; set; }
    public string? Screenshots { get; set; }
    public string? Artwork { get; set; }
    public string? FanArt { get; set; }
    public string? TrailerUrl { get; set; }
    public string? GameplayUrl { get; set; }
    public string? YoutubeLinks { get; set; }

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
    public string? SteamDeckCompatibility { get; set; }
    public int AchievementCount { get; set; }
    public int DlcCount { get; set; }
    public int ExpansionCount { get; set; }

    public string UserId { get; set; } = null!;

    public int? FranchiseId { get; set; }
    public string? FranchiseName { get; set; }

    public int? SeriesId { get; set; }
    public string? SeriesName { get; set; }

    public List<MetadataItemDto> Developers { get; set; } = new();
    public List<MetadataItemDto> Publishers { get; set; } = new();
    public List<MetadataItemDto> Genres { get; set; } = new();
    public List<MetadataItemDto> Tags { get; set; } = new();
    public List<MetadataItemDto> Themes { get; set; } = new();
    public List<MetadataItemDto> Platforms { get; set; } = new();
    public List<MetadataItemDto> DigitalServices { get; set; } = new();
}

public class MetadataItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
