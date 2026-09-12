using System;
using System.Collections.Generic;
using GameCollection.Domain.Common;
using GameCollection.Domain.Enums;

namespace GameCollection.Domain.Entities;

public class UserLibraryEntry : BaseAuditableEntity
{
    public string UserId { get; set; } = null!;

    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    // Ownership Information
    public bool OwnGame { get; set; } = true;
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

    // Rating & Notes
    public double? PersonalRating { get; set; }
    public string? PersonalNotes { get; set; }

    // User's Owned/Selected Platforms & Services for this Game
    public ICollection<Platform> Platforms { get; set; } = new List<Platform>();
    public ICollection<DigitalService> DigitalServices { get; set; } = new List<DigitalService>();
}
