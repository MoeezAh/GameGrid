using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GameCollection.Application.Common.Models;
using GameCollection.Application.DTOs.Library;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Enums;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.Libraries.Queries.GetMyLibrary;

public record GetMyLibraryQuery : IRequest<PaginatedList<UserLibraryListDto>>
{
    public string UserId { get; init; } = null!;
    public string? SearchTerm { get; init; }
    public List<int>? PlatformIds { get; init; }
    public List<int>? ServiceIds { get; init; }
    public List<int>? GenreIds { get; init; }
    public List<CompletionStatus>? CompletionStatuses { get; init; }
    public bool? Wishlist { get; init; }
    public bool? Backlog { get; init; }
    public bool? OwnGame { get; init; }
    public string? SortBy { get; init; }
    public string? SortOrder { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 12;
}

public class GetMyLibraryQueryHandler : IRequestHandler<GetMyLibraryQuery, PaginatedList<UserLibraryListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMyLibraryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedList<UserLibraryListDto>> Handle(GetMyLibraryQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<UserLibraryEntry>().GetQueryable()
            .Include(l => l.Game)
                .ThenInclude(g => g.Platforms)
            .Include(l => l.Game)
                .ThenInclude(g => g.Genres)
            .Include(l => l.Game)
                .ThenInclude(g => g.Tags)
            .Include(l => l.Platforms)
            .Include(l => l.DigitalServices)
            .Where(l => l.UserId == request.UserId);

        // Search
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(l => l.Game.Title.ToLower().Contains(term) ||
                                     (l.Game.AlternateTitles != null && l.Game.AlternateTitles.ToLower().Contains(term)));
        }

        // Platform filter (check user platforms first, fallback to game platforms)
        if (request.PlatformIds != null && request.PlatformIds.Any())
        {
            query = query.Where(l => l.Platforms.Any(p => request.PlatformIds.Contains(p.Id)) ||
                                     (!l.Platforms.Any() && l.Game.Platforms.Any(p => request.PlatformIds.Contains(p.Id))));
        }

        // Service filter
        if (request.ServiceIds != null && request.ServiceIds.Any())
        {
            query = query.Where(l => l.DigitalServices.Any(s => request.ServiceIds.Contains(s.Id)) ||
                                     (!l.DigitalServices.Any() && l.Game.DigitalServices.Any(s => request.ServiceIds.Contains(s.Id))));
        }

        // Genre filter
        if (request.GenreIds != null && request.GenreIds.Any())
        {
            query = query.Where(l => l.Game.Genres.Any(g => request.GenreIds.Contains(g.Id)));
        }

        // Status & ownership filters
        if (request.CompletionStatuses != null && request.CompletionStatuses.Any())
        {
            query = query.Where(l => request.CompletionStatuses.Contains(l.CompletionStatus));
        }

        if (request.Wishlist.HasValue)
        {
            query = query.Where(l => l.Wishlist == request.Wishlist.Value);
        }

        if (request.Backlog.HasValue)
        {
            query = query.Where(l => l.Backlog == request.Backlog.Value);
        }

        if (request.OwnGame.HasValue)
        {
            query = query.Where(l => l.OwnGame == request.OwnGame.Value);
        }

        // Sorting
        bool isDescending = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        query = request.SortBy?.ToLower() switch
        {
            "releasedate" => isDescending ? query.OrderByDescending(l => l.Game.ReleaseDate) : query.OrderBy(l => l.Game.ReleaseDate),
            "purchasedate" => isDescending ? query.OrderByDescending(l => l.PurchaseDate) : query.OrderBy(l => l.PurchaseDate),
            "rating" => isDescending ? query.OrderByDescending(l => l.PersonalRating) : query.OrderBy(l => l.PersonalRating),
            "hoursplayed" => isDescending ? query.OrderByDescending(l => l.HoursPlayed) : query.OrderBy(l => l.HoursPlayed),
            _ => isDescending ? query.OrderByDescending(l => l.Game.Title) : query.OrderBy(l => l.Game.Title)
        };

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<UserLibraryListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PaginatedList<UserLibraryListDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
