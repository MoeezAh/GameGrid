using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        var query = repository.GetQueryable()
            .Include(l => l.Game)
                .ThenInclude(g => g.Platforms)
            .Include(l => l.Game)
                .ThenInclude(g => g.Genres)
            .Include(l => l.Platforms)
            .Include(l => l.DigitalServices)
            .Where(l => l.UserId == request.UserId);

        var totalGamesOwned = await query.CountAsync(l => l.OwnGame, cancellationToken);
        var totalCompletedGames = await query.CountAsync(l => l.CompletionStatus == CompletionStatus.Completed || l.CompletionStatus == CompletionStatus.Completed100, cancellationToken);
        var totalUnplayedGames = await query.CountAsync(l => l.CompletionStatus == CompletionStatus.NotStarted, cancellationToken);
        var wishlistCount = await query.CountAsync(l => l.Wishlist, cancellationToken);
        var backlogCount = await query.CountAsync(l => l.Backlog, cancellationToken);

        // Completion percentage
        double completionPercentage = 0;
        if (totalGamesOwned > 0)
        {
            completionPercentage = Math.Round((double)totalCompletedGames / totalGamesOwned * 100, 2);
        }

        // Count unique platforms across user library
        var totalPlatforms = await query.SelectMany(l => l.Platforms.Any() ? l.Platforms : l.Game.Platforms)
            .Select(p => p.Id).Distinct().CountAsync(cancellationToken);

        var totalServices = await query.SelectMany(l => l.DigitalServices.Any() ? l.DigitalServices : l.Game.DigitalServices)
            .Select(s => s.Id).Distinct().CountAsync(cancellationToken);

        // Recent listings mapped to GameListDto
        var recentlyAddedEntries = await query
            .OrderByDescending(l => l.CreatedDate)
            .Take(5)
            .ToListAsync(cancellationToken);

        var recentlyCompletedEntries = await query
            .Where(l => l.CompletionStatus == CompletionStatus.Completed || l.CompletionStatus == CompletionStatus.Completed100)
            .OrderByDescending(l => l.CompletedDate ?? l.UpdatedDate)
            .Take(5)
            .ToListAsync(cancellationToken);

        var mostPlayedEntries = await query
            .Where(l => l.HoursPlayed > 0)
            .OrderByDescending(l => l.HoursPlayed)
            .Take(5)
            .ToListAsync(cancellationToken);

        var recentlyAdded = recentlyAddedEntries.Select(MapToGameListDto).ToList();
        var recentlyCompleted = recentlyCompletedEntries.Select(MapToGameListDto).ToList();
        var mostPlayed = mostPlayedEntries.Select(MapToGameListDto).ToList();

        // Chart Data - Games by Platform
        var gamesByPlatform = await query
            .SelectMany(l => l.Platforms.Any() ? l.Platforms : l.Game.Platforms)
            .GroupBy(p => p.Name)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key,
                Value = grp.Count()
            })
            .ToListAsync(cancellationToken);

        // Chart Data - Games by Genre
        var gamesByGenre = await query
            .SelectMany(l => l.Game.Genres)
            .GroupBy(g => g.Name)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key,
                Value = grp.Count()
            })
            .ToListAsync(cancellationToken);

        // Chart Data - Games by Release Year
        var gamesByReleaseYear = await query
            .Where(l => l.Game.ReleaseDate != null)
            .GroupBy(l => l.Game.ReleaseDate!.Value.Year)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key.ToString(),
                Value = grp.Count()
            })
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);

        // Chart Data - Games by Status
        var gamesByStatus = await query
            .GroupBy(l => l.CompletionStatus)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key.ToString(),
                Value = grp.Count()
            })
            .ToListAsync(cancellationToken);

        foreach (var statusItem in gamesByStatus)
        {
            statusItem.Name = statusItem.Name switch
            {
                "NotStarted" => "Not Started",
                "Completed100" => "100% Completed",
                _ => statusItem.Name
            };
        }

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
