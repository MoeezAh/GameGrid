using System.Collections.Generic;
using GameGrid.Application.DTOs.Game;

namespace GameGrid.Application.DTOs.Dashboard;

public class DashboardDto
{
    public int TotalGamesOwned { get; set; }
    public int TotalCompletedGames { get; set; }
    public int TotalUnplayedGames { get; set; }
    public int TotalPlatforms { get; set; }
    public int TotalServices { get; set; }
    public int WishlistCount { get; set; }
    public int BacklogCount { get; set; }
    public double CompletionPercentage { get; set; }

    public List<GameListDto> RecentlyAddedGames { get; set; } = new();
    public List<GameListDto> RecentlyCompletedGames { get; set; } = new();
    public List<GameListDto> MostPlayedGames { get; set; } = new();

    public List<ChartDataItem> GamesByPlatform { get; set; } = new();
    public List<ChartDataItem> GamesByGenre { get; set; } = new();
    public List<ChartDataItem> GamesByReleaseYear { get; set; } = new();
    public List<ChartDataItem> GamesByStatus { get; set; } = new();
}

public class ChartDataItem
{
    public string Name { get; set; } = null!;
    public double Value { get; set; }
}
