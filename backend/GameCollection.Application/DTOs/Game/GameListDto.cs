using System;
using System.Collections.Generic;
using GameCollection.Domain.Enums;

namespace GameCollection.Application.DTOs.Game;

public class GameListDto
{
    public int Id { get; set; }
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

    public List<string> Platforms { get; set; } = new();
    public List<string> Genres { get; set; } = new();
    public List<string> Services { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}
