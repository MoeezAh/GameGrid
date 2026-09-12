using System.Threading;
using System.Threading.Tasks;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.Libraries.Commands.RemoveGameFromLibrary;

public record RemoveGameFromLibraryCommand(int Id, string UserId, bool CanRemoveAny = false) : IRequest<bool>;

public class RemoveGameFromLibraryCommandHandler : IRequestHandler<RemoveGameFromLibraryCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveGameFromLibraryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveGameFromLibraryCommand request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<UserLibraryEntry>().GetQueryable();
        var entry = request.CanRemoveAny
            ? await query.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken)
            : await query.FirstOrDefaultAsync(l => l.Id == request.Id && l.UserId == request.UserId, cancellationToken);

        if (entry == null) return false;

        entry.IsDeleted = true;
        _unitOfWork.Repository<UserLibraryEntry>().Update(entry);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
