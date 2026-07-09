using System;
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
        var repository = _unitOfWork.Repository<Game>();
        var query = repository.GetQueryable()
            .Where(g => g.UserId == request.UserId);

        var totalGamesOwned = await query.CountAsync(g => g.OwnGame, cancellationToken);
        var totalCompletedGames = await query.CountAsync(g => g.CompletionStatus == CompletionStatus.Completed || g.CompletionStatus == CompletionStatus.Completed100, cancellationToken);
        var totalUnplayedGames = await query.CountAsync(g => g.CompletionStatus == CompletionStatus.NotStarted, cancellationToken);
        var wishlistCount = await query.CountAsync(g => g.Wishlist, cancellationToken);
        var backlogCount = await query.CountAsync(g => g.Backlog, cancellationToken);

        // Calculate completion percentage based on games owned
        double completionPercentage = 0;
        if (totalGamesOwned > 0)
        {
            completionPercentage = Math.Round((double)totalCompletedGames / totalGamesOwned * 100, 2);
        }

        // Platforms & Services count in user's library
        var totalPlatforms = await query.SelectMany(g => g.Platforms).Select(p => p.Id).Distinct().CountAsync(cancellationToken);
        var totalServices = await query.SelectMany(g => g.DigitalServices).Select(s => s.Id).Distinct().CountAsync(cancellationToken);

        // Recent listings
        var recentlyAdded = await query
            .OrderByDescending(g => g.CreatedDate)
            .Take(5)
            .ProjectTo<GameListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var recentlyCompleted = await query
            .Where(g => g.CompletionStatus == CompletionStatus.Completed || g.CompletionStatus == CompletionStatus.Completed100)
            .OrderByDescending(g => g.CompletedDate ?? g.UpdatedDate)
            .Take(5)
            .ProjectTo<GameListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var mostPlayed = await query
            .Where(g => g.HoursPlayed > 0)
            .OrderByDescending(g => g.HoursPlayed)
            .Take(5)
            .ProjectTo<GameListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        // Chart Data - Games by Platform
        var gamesByPlatform = await query
            .SelectMany(g => g.Platforms)
            .GroupBy(p => p.Name)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key,
                Value = grp.Count()
            })
            .ToListAsync(cancellationToken);

        // Chart Data - Games by Genre
        var gamesByGenre = await query
            .SelectMany(g => g.Genres)
            .GroupBy(g => g.Name)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key,
                Value = grp.Count()
            })
            .ToListAsync(cancellationToken);

        // Chart Data - Games by Release Year
        var gamesByReleaseYear = await query
            .Where(g => g.ReleaseDate != null)
            .GroupBy(g => g.ReleaseDate!.Value.Year)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key.ToString(),
                Value = grp.Count()
            })
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);

        // Chart Data - Games by Status
        var gamesByStatus = await query
            .GroupBy(g => g.CompletionStatus)
            .Select(grp => new ChartDataItem
            {
                Name = grp.Key.ToString(),
                Value = grp.Count()
            })
            .ToListAsync(cancellationToken);

        // Format CompletionStatus names nicely for UI
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
}
