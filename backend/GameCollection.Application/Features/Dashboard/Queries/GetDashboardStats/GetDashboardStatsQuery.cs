using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GameCollection.Application.DTOs.Dashboard;
using GameCollection.Application.DTOs.Game;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Enums;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.Dashboard.Queries.GetDashboardStats;

public record GetDashboardStatsQuery(string UserId) : IRequest<DashboardDto>;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DashboardDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<UserLibraryEntry>();
        
        // Fetch all library entries for this user with their game and taxonomy details
        var userEntries = await repository.GetQueryable()
            .Include(l => l.Game)
                .ThenInclude(g => g.Platforms)
            .Include(l => l.Game)
                .ThenInclude(g => g.Genres)
            .Include(l => l.Game)
                .ThenInclude(g => g.DigitalServices)
            .Include(l => l.Game)
                .ThenInclude(g => g.Tags)
            .Include(l => l.Platforms)
            .Include(l => l.DigitalServices)
            .Where(l => l.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        if (!userEntries.Any())
        {
            return new DashboardDto
            {
                TotalGamesOwned = 0,
                TotalCompletedGames = 0,
                TotalUnplayedGames = 0,
                TotalPlatforms = 0,
                TotalServices = 0,
                WishlistCount = 0,
                BacklogCount = 0,
                CompletionPercentage = 0,
                RecentlyAddedGames = new List<GameListDto>(),
                RecentlyCompletedGames = new List<GameListDto>(),
                MostPlayedGames = new List<GameListDto>(),
                GamesByPlatform = new List<ChartDataItem>(),
                GamesByGenre = new List<ChartDataItem>(),
                GamesByReleaseYear = new List<ChartDataItem>(),
                GamesByStatus = new List<ChartDataItem>()
            };
        }

        var totalGamesOwned = userEntries.Count(l => l.OwnGame);
        var totalCompletedGames = userEntries.Count(l => l.CompletionStatus == CompletionStatus.Completed || l.CompletionStatus == CompletionStatus.Completed100);
        var totalUnplayedGames = userEntries.Count(l => l.CompletionStatus == CompletionStatus.NotStarted);
        var wishlistCount = userEntries.Count(l => l.Wishlist);
        var backlogCount = userEntries.Count(l => l.Backlog);

        // Completion percentage
        double completionPercentage = 0;
        if (totalGamesOwned > 0)
        {
            completionPercentage = Math.Round((double)totalCompletedGames / totalGamesOwned * 100, 2);
        }

        // Count unique platforms across user library (prefer user selected platforms, fallback to game platforms)
        var totalPlatforms = userEntries
            .SelectMany(l => l.Platforms.Any() ? l.Platforms : l.Game.Platforms)
            .Select(p => p.Id)
            .Distinct()
            .Count();

        var totalServices = userEntries
            .SelectMany(l => l.DigitalServices.Any() ? l.DigitalServices : l.Game.DigitalServices)
            .Select(s => s.Id)
            .Distinct()
            .Count();

        // Recent listings mapped to GameListDto
        var recentlyAdded = userEntries
            .OrderByDescending(l => l.CreatedDate)
            .Take(5)
            .Select(MapToGameListDto)
            .ToList();

        var recentlyCompleted = userEntries
            .Where(l => l.CompletionStatus == CompletionStatus.Completed || l.CompletionStatus == CompletionStatus.Completed100)
            .OrderByDescending(l => l.CompletedDate ?? l.UpdatedDate)
            .Take(5)
            .Select(MapToGameListDto)
            .ToList();

        var mostPlayed = userEntries
            .Where(l => l.HoursPlayed > 0)
            .OrderByDescending(l => l.HoursPlayed)
            .Take(5)
            .Select(MapToGameListDto)
            .ToList();

        // Chart Data - Games by Platform
        var gamesByPlatform = userEntries
            .SelectMany(l => l.Platforms.Any() ? l.Platforms : l.Game.Platforms)
            .GroupBy(p => p.Name)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key,
                Value = grp.Count()
            })
            .OrderByDescending(x => x.Value)
            .ToList();

        // Chart Data - Games by Genre
        var gamesByGenre = userEntries
            .SelectMany(l => l.Game.Genres)
            .GroupBy(g => g.Name)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key,
                Value = grp.Count()
            })
            .OrderByDescending(x => x.Value)
            .ToList();

        // Chart Data - Games by Release Year
        var gamesByReleaseYear = userEntries
            .Where(l => l.Game.ReleaseDate != null)
            .GroupBy(l => l.Game.ReleaseDate!.Value.Year)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key.ToString(),
                Value = grp.Count()
            })
            .OrderBy(item => item.Name)
            .ToList();

        // Chart Data - Games by Status
        var gamesByStatus = userEntries
            .GroupBy(l => l.CompletionStatus)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key switch
                {
                    CompletionStatus.NotStarted => "Not Started",
                    CompletionStatus.Playing => "Playing",
                    CompletionStatus.OnHold => "On Hold",
                    CompletionStatus.Completed => "Completed",
                    CompletionStatus.Dropped => "Dropped",
                    CompletionStatus.Completed100 => "100% Completed",
                    CompletionStatus.Replaying => "Replaying",
                    _ => grp.Key.ToString()
                },
                Value = grp.Count()
            })
            .ToList();

        return new DashboardDto
        {
            TotalGamesOwned = totalGamesOwned,
            TotalCompletedGames = totalCompletedGames,
            TotalUnplayedGames = totalUnplayedGames,
            TotalPlatforms = totalPlatforms,
            TotalServices = totalServices,
            WishlistCount = wishlistCount,
            BacklogCount = backlogCount,
            CompletionPercentage = completionPercentage,
            RecentlyAddedGames = recentlyAdded,
            RecentlyCompletedGames = recentlyCompleted,
            MostPlayedGames = mostPlayed,
            GamesByPlatform = gamesByPlatform,
            GamesByGenre = gamesByGenre,
            GamesByReleaseYear = gamesByReleaseYear,
            GamesByStatus = gamesByStatus
        };
    }

    private static GameListDto MapToGameListDto(UserLibraryEntry l)
    {
        return new GameListDto
        {
            Id = l.GameId,
            Title = l.Game.Title,
            CoverImage = l.Game.CoverImage,
            Banner = l.Game.Banner,
            ReleaseDate = l.Game.ReleaseDate,
            CommunityRating = l.Game.CommunityRating,
            CriticRating = l.Game.CriticRating,
            MetacriticScore = l.Game.MetacriticScore,
            Platforms = l.Platforms.Any() ? l.Platforms.Select(p => p.Name).ToList() : l.Game.Platforms.Select(p => p.Name).ToList(),
            Genres = l.Game.Genres.Select(g => g.Name).ToList(),
            Services = l.DigitalServices.Any() ? l.DigitalServices.Select(s => s.Name).ToList() : l.Game.DigitalServices.Select(s => s.Name).ToList(),
            Tags = l.Game.Tags.Select(t => t.Name).ToList(),
            IsInUserLibrary = true,
            UserLibraryEntryId = l.Id
        };
    }
}
