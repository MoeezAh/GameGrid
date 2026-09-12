using System;
using System.Collections.Generic;

namespace GameCollection.Application.DTOs.Game;

public class GameListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? CoverImage { get; set; }
    public string? Banner { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
    public double? CommunityRating { get; set; }
    public double? CriticRating { get; set; }
    public int? MetacriticScore { get; set; }

    public List<string> Platforms { get; set; } = new();
    public List<string> Genres { get; set; } = new();
    public List<string> Services { get; set; } = new();
    public List<string> Tags { get; set; } = new();

    public bool IsInUserLibrary { get; set; }
    public int? UserLibraryEntryId { get; set; }
}
