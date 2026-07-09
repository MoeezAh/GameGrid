using System.Threading;
using System.Threading.Tasks;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.Games.Commands.DeleteGame;

public record DeleteGameCommand(int Id, string UserId) : IRequest<bool>;

public class DeleteGameCommandHandler : IRequestHandler<DeleteGameCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGameCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _unitOfWork.Repository<Game>().GetQueryable()
            .FirstOrDefaultAsync(g => g.Id == request.Id && g.UserId == request.UserId, cancellationToken);

        if (game == null) return false;

        // Perform soft delete
        game.IsDeleted = true;
        
        _unitOfWork.Repository<Game>().Update(game);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
