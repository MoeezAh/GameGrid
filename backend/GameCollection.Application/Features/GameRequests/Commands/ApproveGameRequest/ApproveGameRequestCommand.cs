using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameCollection.Application.DTOs.GameRequest;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Enums;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.GameRequests.Commands.ApproveGameRequest;

public record ApproveGameRequestCommand(int RequestId, ApproveGameRequestDto Dto, string ReviewerUserId) : IRequest<(bool Success, int? GameId, string[] Errors)>;

public class ApproveGameRequestCommandHandler : IRequestHandler<ApproveGameRequestCommand, (bool Success, int? GameId, string[] Errors)>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveGameRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, int? GameId, string[] Errors)> Handle(ApproveGameRequestCommand request, CancellationToken cancellationToken)
    {
        var gameRequest = await _unitOfWork.Repository<GameRequest>().GetByIdAsync(request.RequestId);
        if (gameRequest == null)
        {
            return (false, null, new[] { "Game request not found." });
        }

        if (gameRequest.Status != GameRequestStatus.Pending)
        {
            return (false, null, new[] { $"Request cannot be approved because its current status is {gameRequest.Status}." });
        }

        int targetGameId;

        if (request.Dto.ExistingGameId.HasValue && request.Dto.ExistingGameId.Value > 0)
        {
            // Link to existing catalog game
            var existingGame = await _unitOfWork.Repository<Game>().GetByIdAsync(request.Dto.ExistingGameId.Value);
            if (existingGame == null)
            {
                return (false, null, new[] { "Specified existing game was not found in catalog." });
            }
            targetGameId = existingGame.Id;
        }
        else
        {
            // Check for existing duplicate by title (case-insensitive)
            var duplicateGame = await _unitOfWork.Repository<Game>().GetQueryable()
                .FirstOrDefaultAsync(g => g.Title.ToLower() == gameRequest.GameTitle.ToLower(), cancellationToken);

            if (duplicateGame != null)
            {
                targetGameId = duplicateGame.Id;
            }
            else
            {
                // Create new catalog game
                DateTimeOffset? releaseDate = null;
                if (!string.IsNullOrWhiteSpace(gameRequest.ApproximateReleaseYear) &&
                    int.TryParse(gameRequest.ApproximateReleaseYear.Trim(), out var year) &&
                    year >= 1970 && year <= 2100)
                {
                    releaseDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
                }

                var newGame = new Game
                {
                    Title = gameRequest.GameTitle,
                    Description = gameRequest.AdditionalInformation,
                    Notes = !string.IsNullOrEmpty(gameRequest.Links) ? $"References:\n{gameRequest.Links}" : null,
                    ReleaseDate = releaseDate
                };

                await _unitOfWork.Repository<Game>().AddAsync(newGame);
                await _unitOfWork.SaveChangesAsync();
                targetGameId = newGame.Id;
            }
        }

        gameRequest.Status = GameRequestStatus.Approved;
        gameRequest.CreatedGameId = targetGameId;
        gameRequest.ReviewedByUserId = request.ReviewerUserId;
        gameRequest.ReviewedAt = DateTimeOffset.UtcNow;
        gameRequest.ReviewNotes = request.Dto.ReviewNotes;

        _unitOfWork.Repository<GameRequest>().Update(gameRequest);
        await _unitOfWork.SaveChangesAsync();

        return (true, targetGameId, Array.Empty<string>());
    }
}
