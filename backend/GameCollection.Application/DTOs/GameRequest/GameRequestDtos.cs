using System;
using System.Collections.Generic;
using GameCollection.Domain.Enums;

namespace GameCollection.Application.DTOs.GameRequest;

public class GameRequestDto
{
    public int Id { get; set; }
    public string GameTitle { get; set; } = null!;
    public string? ApproximateReleaseYear { get; set; }
    public string? Platforms { get; set; }
    public string? Links { get; set; }
    public List<string> LinkList { get; set; } = new();
    public string? AdditionalInformation { get; set; }

    public GameRequestStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public string? ReviewNotes { get; set; }

    public string RequestedByUserId { get; set; } = null!;
    public string? RequestedByUsername { get; set; }
    public string? ReviewedByUserId { get; set; }
    public string? ReviewedByUsername { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }

    public int? CreatedGameId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
}

public class SubmitGameRequestDto
{
    public string GameTitle { get; set; } = null!;
    public string? ApproximateReleaseYear { get; set; }
    public string? Platforms { get; set; }
    public List<string>? Links { get; set; } = new();
    public string? AdditionalInformation { get; set; }
}

public class ApproveGameRequestDto
{
    public string? ReviewNotes { get; set; }
    public int? ExistingGameId { get; set; } // Link to existing catalog game if duplicate
}

public class RejectGameRequestDto
{
    public string RejectionReason { get; set; } = null!;
}

public class DuplicateCheckResultDto
{
    public bool HasPotentialDuplicate { get; set; }
    public List<DuplicateCandidateDto> Candidates { get; set; } = new();
}

public class DuplicateCandidateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTimeOffset? ReleaseDate { get; set; }
    public string? CoverImage { get; set; }
}
