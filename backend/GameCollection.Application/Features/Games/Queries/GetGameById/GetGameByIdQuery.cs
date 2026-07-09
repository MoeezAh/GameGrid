using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GameCollection.Application.DTOs.Game;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.Games.Queries.GetGameById;

public record GetGameByIdQuery(int Id, string UserId) : IRequest<GameDto?>;

public class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQuery, GameDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetGameByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GameDto?> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var game = await _unitOfWork.Repository<Game>().GetQueryable()
            .Include(g => g.Franchise)
            .Include(g => g.Series)
            .Include(g => g.Developers)
            .Include(g => g.Publishers)
            .Include(g => g.Genres)
            .Include(g => g.Tags)
            .Include(g => g.Themes)
            .Include(g => g.Platforms)
            .Include(g => g.DigitalServices)
            .FirstOrDefaultAsync(g => g.Id == request.Id && g.UserId == request.UserId, cancellationToken);

        if (game == null) return null;

        return _mapper.Map<GameDto>(game);
    }
}
