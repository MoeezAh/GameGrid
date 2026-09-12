using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameCollection.Application.DTOs.Library;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.Libraries.Commands.UpdateLibraryEntry;

public record UpdateLibraryEntryCommand(int Id, UpdateLibraryEntryDto Dto, string UserId, bool CanUpdateAny = false) : IRequest<bool>;

public class UpdateLibraryEntryCommandHandler : IRequestHandler<UpdateLibraryEntryCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLibraryEntryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateLibraryEntryCommand request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<UserLibraryEntry>().GetQueryable()
            .Include(l => l.Platforms)
            .Include(l => l.DigitalServices);

        var entry = request.CanUpdateAny 
            ? await query.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken)
            : await query.FirstOrDefaultAsync(l => l.Id == request.Id && l.UserId == request.UserId, cancellationToken);

        if (entry == null) return false;

        entry.OwnGame = request.Dto.OwnGame;
        entry.Wishlist = request.Dto.Wishlist;
        entry.Backlog = request.Dto.Backlog;
        entry.PhysicalCopy = request.Dto.PhysicalCopy;
        entry.DigitalCopy = request.Dto.DigitalCopy;
        entry.CollectorsEdition = request.Dto.CollectorsEdition;
        entry.SpecialEdition = request.Dto.SpecialEdition;
        entry.PurchaseDate = request.Dto.PurchaseDate;
        entry.PurchasePrice = request.Dto.PurchasePrice;
        entry.Currency = request.Dto.Currency;
        entry.StorePurchasedFrom = request.Dto.StorePurchasedFrom;
        entry.PurchaseRegion = request.Dto.PurchaseRegion;
        entry.ReceiptReference = request.Dto.ReceiptReference;
        entry.Gifted = request.Dto.Gifted;
        entry.StartedPlayingDate = request.Dto.StartedPlayingDate;
        entry.CompletedDate = request.Dto.CompletedDate;
        entry.LastPlayedDate = request.Dto.LastPlayedDate;
        entry.HoursPlayed = request.Dto.HoursPlayed;
        entry.CompletionStatus = request.Dto.CompletionStatus;
        entry.PersonalRating = request.Dto.PersonalRating;
        entry.PersonalNotes = request.Dto.PersonalNotes;

        // Sync Platforms
        entry.Platforms.Clear();
        if (request.Dto.PlatformIds != null && request.Dto.PlatformIds.Any())
        {
            entry.Platforms = await _unitOfWork.Repository<Platform>().GetQueryable()
                .Where(p => request.Dto.PlatformIds.Contains(p.Id)).ToListAsync(cancellationToken);
        }

        // Sync Digital Services
        entry.DigitalServices.Clear();
        if (request.Dto.DigitalServiceIds != null && request.Dto.DigitalServiceIds.Any())
        {
            entry.DigitalServices = await _unitOfWork.Repository<DigitalService>().GetQueryable()
                .Where(s => request.Dto.DigitalServiceIds.Contains(s.Id)).ToListAsync(cancellationToken);
        }

        _unitOfWork.Repository<UserLibraryEntry>().Update(entry);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
