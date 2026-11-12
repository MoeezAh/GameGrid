using System;
using System.Threading;
using System.Threading.Tasks;
using GameGrid.Application.DTOs.GameRequest;
using GameGrid.Domain.Entities;
using GameGrid.Domain.Enums;
using GameGrid.Domain.Interfaces;
using MediatR;

namespace GameGrid.Application.Features.GameRequests.Commands.RejectGameRequest;

public record RejectGameRequestCommand(int RequestId, RejectGameRequestDto Dto, string ReviewerUserId) : IRequest<(bool Success, string[] Errors)>;

public class RejectGameRequestCommandHandler : IRequestHandler<RejectGameRequestCommand, (bool Success, string[] Errors)>
{
    private readonly IUnitOfWork _unitOfWork;

    public RejectGameRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, string[] Errors)> Handle(RejectGameRequestCommand request, CancellationToken cancellationToken)
    {
        var gameRequest = await _unitOfWork.Repository<GameRequest>().GetByIdAsync(request.RequestId);
        if (gameRequest == null)
        {
            return (false, new[] { "Game request not found." });
        }

        if (gameRequest.Status != GameRequestStatus.Pending)
        {
            return (false, new[] { $"Request cannot be rejected because its current status is {gameRequest.Status}." });
        }

        gameRequest.Status = GameRequestStatus.Rejected;
        gameRequest.ReviewedByUserId = request.ReviewerUserId;
        gameRequest.ReviewedAt = DateTimeOffset.UtcNow;
        gameRequest.ReviewNotes = request.Dto.RejectionReason;

        _unitOfWork.Repository<GameRequest>().Update(gameRequest);
        await _unitOfWork.SaveChangesAsync();

        return (true, Array.Empty<string>());
    }
}
