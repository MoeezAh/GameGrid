using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameCollection.Application.DTOs.GameRequest;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.GameRequests.Queries.CheckDuplicateGame;

public record CheckDuplicateGameQuery(string Title) : IRequest<DuplicateCheckResultDto>;

public class CheckDuplicateGameQueryHandler : IRequestHandler<CheckDuplicateGameQuery, DuplicateCheckResultDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckDuplicateGameQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DuplicateCheckResultDto> Handle(CheckDuplicateGameQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return new DuplicateCheckResultDto { HasPotentialDuplicate = false };
        }

        var normalized = request.Title.Trim().ToLower();

        var candidates = await _unitOfWork.Repository<Game>().GetQueryable()
            .Where(g => g.Title.ToLower().Contains(normalized) ||
                        (g.AlternateTitles != null && g.AlternateTitles.ToLower().Contains(normalized)) ||
                        normalized.Contains(g.Title.ToLower()))
            .Take(5)
            .Select(g => new DuplicateCandidateDto
            {
                Id = g.Id,
                Title = g.Title,
                ReleaseDate = g.ReleaseDate,
                CoverImage = g.CoverImage
            })
            .ToListAsync(cancellationToken);

        return new DuplicateCheckResultDto
        {
            HasPotentialDuplicate = candidates.Any(),
            Candidates = candidates
        };
    }
}
