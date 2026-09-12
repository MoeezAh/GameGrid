using System.Threading;
using System.Threading.Tasks;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.GameRequests.Commands.DeleteGameRequest;

public record DeleteGameRequestCommand(int Id, string UserId, bool CanDeleteAny = false) : IRequest<bool>;

public class DeleteGameRequestCommandHandler : IRequestHandler<DeleteGameRequestCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGameRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteGameRequestCommand request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<GameRequest>().GetQueryable();
        var gameRequest = request.CanDeleteAny
            ? await query.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            : await query.FirstOrDefaultAsync(r => r.Id == request.Id && r.RequestedByUserId == request.UserId, cancellationToken);

        if (gameRequest == null) return false;

        gameRequest.IsDeleted = true;
        _unitOfWork.Repository<GameRequest>().Update(gameRequest);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
