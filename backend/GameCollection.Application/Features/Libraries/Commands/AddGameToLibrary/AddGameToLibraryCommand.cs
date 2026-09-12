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

namespace GameCollection.Application.Features.Libraries.Commands.AddGameToLibrary;

public record AddGameToLibraryCommand(AddGameToLibraryDto Dto, string UserId) : IRequest<(bool Success, int Id, string[] Errors)>;

public class AddGameToLibraryCommandHandler : IRequestHandler<AddGameToLibraryCommand, (bool Success, int Id, string[] Errors)>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddGameToLibraryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, int Id, string[] Errors)> Handle(AddGameToLibraryCommand request, CancellationToken cancellationToken)
    {
        var game = await _unitOfWork.Repository<Game>().GetByIdAsync(request.Dto.GameId);
        if (game == null)
        {
            return (false, 0, new[] { "Selected catalog game was not found." });
        }

        // Check if already in user's library
        var existingEntry = await _unitOfWork.Repository<UserLibraryEntry>().GetQueryable()
            .FirstOrDefaultAsync(l => l.GameId == request.Dto.GameId && l.UserId == request.UserId, cancellationToken);

        if (existingEntry != null)
        {
            return (false, existingEntry.Id, new[] { "This game is already in your library." });
        }

        var entry = new UserLibraryEntry
        {
            GameId = request.Dto.GameId,
            UserId = request.UserId,
            OwnGame = request.Dto.OwnGame,
            Wishlist = request.Dto.Wishlist,
            Backlog = request.Dto.Backlog,
            PhysicalCopy = request.Dto.PhysicalCopy,
            DigitalCopy = request.Dto.DigitalCopy,
            CollectorsEdition = request.Dto.CollectorsEdition,
            SpecialEdition = request.Dto.SpecialEdition,
            PurchaseDate = request.Dto.PurchaseDate,
            PurchasePrice = request.Dto.PurchasePrice,
            Currency = request.Dto.Currency,
            StorePurchasedFrom = request.Dto.StorePurchasedFrom,
            PurchaseRegion = request.Dto.PurchaseRegion,
            ReceiptReference = request.Dto.ReceiptReference,
            Gifted = request.Dto.Gifted,
            StartedPlayingDate = request.Dto.StartedPlayingDate,
            CompletedDate = request.Dto.CompletedDate,
            LastPlayedDate = request.Dto.LastPlayedDate,
            HoursPlayed = request.Dto.HoursPlayed,
            CompletionStatus = request.Dto.CompletionStatus,
            PersonalRating = request.Dto.PersonalRating,
            PersonalNotes = request.Dto.PersonalNotes
        };

        // Add user-selected platforms (does not alter central game platforms)
        if (request.Dto.PlatformIds != null && request.Dto.PlatformIds.Any())
        {
            entry.Platforms = await _unitOfWork.Repository<Platform>().GetQueryable()
                .Where(p => request.Dto.PlatformIds.Contains(p.Id)).ToListAsync(cancellationToken);
        }

        // Add user-selected digital services
        if (request.Dto.DigitalServiceIds != null && request.Dto.DigitalServiceIds.Any())
        {
            entry.DigitalServices = await _unitOfWork.Repository<DigitalService>().GetQueryable()
                .Where(s => request.Dto.DigitalServiceIds.Contains(s.Id)).ToListAsync(cancellationToken);
        }

        await _unitOfWork.Repository<UserLibraryEntry>().AddAsync(entry);
        await _unitOfWork.SaveChangesAsync();

        return (true, entry.Id, Array.Empty<string>());
    }
}
