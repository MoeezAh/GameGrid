using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GameGrid.Application.DTOs.GameRequest;
using GameGrid.Domain.Entities;
using GameGrid.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameGrid.Application.Features.GameRequests.Queries.GetMyGameRequests;

public record GetMyGameRequestsQuery(string UserId) : IRequest<List<GameRequestDto>>;

public class GetMyGameRequestsQueryHandler : IRequestHandler<GetMyGameRequestsQuery, List<GameRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMyGameRequestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<GameRequestDto>> Handle(GetMyGameRequestsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _unitOfWork.Repository<GameRequest>().GetQueryable()
            .Where(r => r.RequestedByUserId == request.UserId)
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<GameRequestDto>>(entities);
    }
}
