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
    public string? SortBy { get; init; }
    public string? SortOrder { get; init; } // "asc" or "desc"
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? UserId { get; init; }
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
            .AsNoTracking();

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

        // Apply sorting
        bool isDescending = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        
        query = request.SortBy?.ToLower() switch
        {
            "releasedate" => isDescending ? query.OrderByDescending(g => g.ReleaseDate) : query.OrderBy(g => g.ReleaseDate),
            "criticrating" => isDescending ? query.OrderByDescending(g => g.CriticRating) : query.OrderBy(g => g.CriticRating),
            "metacritic" => isDescending ? query.OrderByDescending(g => g.MetacriticScore) : query.OrderBy(g => g.MetacriticScore),
            _ => isDescending ? query.OrderByDescending(g => g.Title) : query.OrderBy(g => g.Title)
        };

        // Paginate
        int totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<GameListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        // Populate user library ownership if UserId supplied
        if (!string.IsNullOrEmpty(request.UserId) && items.Any())
        {
            var gameIds = items.Select(i => i.Id).ToList();
            var userLibraryMap = await _unitOfWork.Repository<UserLibraryEntry>().GetQueryable()
                .Where(l => l.UserId == request.UserId && gameIds.Contains(l.GameId))
                .Select(l => new { l.Id, l.GameId })
                .ToDictionaryAsync(l => l.GameId, l => l.Id, cancellationToken);

            foreach (var item in items)
            {
                if (userLibraryMap.TryGetValue(item.Id, out var libraryEntryId))
                {
                    item.IsInUserLibrary = true;
                    item.UserLibraryEntryId = libraryEntryId;
                }
            }
        }

        return new PaginatedList<GameListDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
