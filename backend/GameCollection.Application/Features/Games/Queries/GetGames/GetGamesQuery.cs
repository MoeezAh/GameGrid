using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GameCollection.Application.Common.Models;
using GameCollection.Application.DTOs.Game;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Enums;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.Games.Queries.GetGames;

public record GetGamesQuery : IRequest<PaginatedList<GameListDto>>
{
    public string? SearchTerm { get; init; }
    public List<int>? PlatformIds { get; init; }
    public List<int>? ServiceIds { get; init; }
    public List<int>? GenreIds { get; init; }
    public List<int>? DeveloperIds { get; init; }
    public List<int>? PublisherIds { get; init; }
    public List<CompletionStatus>? CompletionStatuses { get; init; }
    public bool? Wishlist { get; init; }
    public bool? Backlog { get; init; }
    public bool? OwnGame { get; init; }
    public string? SortBy { get; init; }
    public string? SortOrder { get; init; } // "asc" or "desc"
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string UserId { get; init; } = null!;
}

public class GetGamesQueryHandler : IRequestHandler<GetGamesQuery, PaginatedList<GameListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetGamesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GameListDto>> Handle(GetGamesQuery request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Game>();
        var query = repository.GetQueryable()
            .Include(g => g.Platforms)
            .Include(g => g.Genres)
            .Include(g => g.DigitalServices)
            .Include(g => g.Tags)
            .Where(g => g.UserId == request.UserId);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(g => g.Title.ToLower().Contains(term) || 
                                     (g.AlternateTitles != null && g.AlternateTitles.ToLower().Contains(term)) ||
                                     (g.OriginalTitle != null && g.OriginalTitle.ToLower().Contains(term)));
        }

        if (request.PlatformIds != null && request.PlatformIds.Any())
        {
            query = query.Where(g => g.Platforms.Any(p => request.PlatformIds.Contains(p.Id)));
        }

        if (request.ServiceIds != null && request.ServiceIds.Any())
        {
            query = query.Where(g => g.DigitalServices.Any(s => request.ServiceIds.Contains(s.Id)));
        }

        if (request.GenreIds != null && request.GenreIds.Any())
        {
            query = query.Where(g => g.Genres.Any(g => request.GenreIds.Contains(g.Id)));
        }

        if (request.DeveloperIds != null && request.DeveloperIds.Any())
        {
            query = query.Where(g => g.Developers.Any(d => request.DeveloperIds.Contains(d.Id)));
        }

        if (request.PublisherIds != null && request.PublisherIds.Any())
        {
            query = query.Where(g => g.Publishers.Any(p => request.PublisherIds.Contains(p.Id)));
        }

        if (request.CompletionStatuses != null && request.CompletionStatuses.Any())
        {
            query = query.Where(g => request.CompletionStatuses.Contains(g.CompletionStatus));
        }

        if (request.Wishlist.HasValue)
        {
            query = query.Where(g => g.Wishlist == request.Wishlist.Value);
        }

        if (request.Backlog.HasValue)
        {
            query = query.Where(g => g.Backlog == request.Backlog.Value);
        }

        if (request.OwnGame.HasValue)
        {
            query = query.Where(g => g.OwnGame == request.OwnGame.Value);
        }

        // Apply sorting
        bool isDescending = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        
        query = request.SortBy?.ToLower() switch
        {
            "releasedate" => isDescending ? query.OrderByDescending(g => g.ReleaseDate) : query.OrderBy(g => g.ReleaseDate),
            "purchasedate" => isDescending ? query.OrderByDescending(g => g.PurchaseDate) : query.OrderBy(g => g.PurchaseDate),
            "rating" => isDescending ? query.OrderByDescending(g => g.PersonalRating) : query.OrderBy(g => g.PersonalRating),
            "hoursplayed" => isDescending ? query.OrderByDescending(g => g.HoursPlayed) : query.OrderBy(g => g.HoursPlayed),
            _ => isDescending ? query.OrderByDescending(g => g.Title) : query.OrderBy(g => g.Title)
        };

        // Paginate and Project
        int totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<GameListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PaginatedList<GameListDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
