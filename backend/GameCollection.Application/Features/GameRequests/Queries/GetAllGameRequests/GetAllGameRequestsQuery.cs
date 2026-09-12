using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GameCollection.Application.DTOs.GameRequest;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Enums;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.GameRequests.Queries.GetAllGameRequests;

public record GetAllGameRequestsQuery(GameRequestStatus? Status = null) : IRequest<List<GameRequestDto>>;

public class GetAllGameRequestsQueryHandler : IRequestHandler<GetAllGameRequestsQuery, List<GameRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllGameRequestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<GameRequestDto>> Handle(GetAllGameRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<GameRequest>().GetQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        return await query
            .OrderByDescending(r => r.CreatedDate)
            .ProjectTo<GameRequestDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
