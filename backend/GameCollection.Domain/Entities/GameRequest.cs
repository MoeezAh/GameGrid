using System;
using GameCollection.Domain.Common;
using GameCollection.Domain.Enums;

namespace GameCollection.Domain.Entities;

public class GameRequest : BaseAuditableEntity
{
    public string GameTitle { get; set; } = null!;
    public string? ApproximateReleaseYear { get; set; }
    public string? Platforms { get; set; }
    public string? Links { get; set; }
    public string? AdditionalInformation { get; set; }

    public GameRequestStatus Status { get; set; } = GameRequestStatus.Pending;
    public string? ReviewNotes { get; set; }

    public string RequestedByUserId { get; set; } = null!;
    public string? ReviewedByUserId { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }

    public int? CreatedGameId { get; set; }
    public Game? CreatedGame { get; set; }
}
