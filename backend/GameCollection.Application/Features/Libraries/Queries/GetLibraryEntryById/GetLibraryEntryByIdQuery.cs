using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GameCollection.Application.DTOs.Library;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.Libraries.Queries.GetLibraryEntryById;

public record GetLibraryEntryByIdQuery(int Id, string UserId, bool CanViewAny = false) : IRequest<UserLibraryEntryDto?>;

public class GetLibraryEntryByIdQueryHandler : IRequestHandler<GetLibraryEntryByIdQuery, UserLibraryEntryDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLibraryEntryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserLibraryEntryDto?> Handle(GetLibraryEntryByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<UserLibraryEntry>().GetQueryable()
            .Include(l => l.Game)
                .ThenInclude(g => g.Franchise)
            .Include(l => l.Game)
                .ThenInclude(g => g.Series)
            .Include(l => l.Game)
                .ThenInclude(g => g.Developers)
            .Include(l => l.Game)
                .ThenInclude(g => g.Publishers)
            .Include(l => l.Game)
                .ThenInclude(g => g.Genres)
            .Include(l => l.Game)
                .ThenInclude(g => g.Tags)
            .Include(l => l.Game)
                .ThenInclude(g => g.Themes)
            .Include(l => l.Game)
                .ThenInclude(g => g.Platforms)
            .Include(l => l.Game)
                .ThenInclude(g => g.DigitalServices)
            .Include(l => l.Platforms)
            .Include(l => l.DigitalServices);

        var entry = request.CanViewAny
            ? await query.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken)
            : await query.FirstOrDefaultAsync(l => l.Id == request.Id && l.UserId == request.UserId, cancellationToken);

        if (entry == null) return null;

        return _mapper.Map<UserLibraryEntryDto>(entry);
    }
}
