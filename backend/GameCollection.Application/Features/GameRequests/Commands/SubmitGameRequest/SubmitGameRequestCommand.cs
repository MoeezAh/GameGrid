using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameCollection.Application.DTOs.GameRequest;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Enums;
using GameCollection.Domain.Interfaces;
using MediatR;

namespace GameCollection.Application.Features.GameRequests.Commands.SubmitGameRequest;

public record SubmitGameRequestCommand(SubmitGameRequestDto Dto, string UserId) : IRequest<(bool Success, int Id, string[] Errors)>;

public class SubmitGameRequestCommandHandler : IRequestHandler<SubmitGameRequestCommand, (bool Success, int Id, string[] Errors)>
{
    private readonly IUnitOfWork _unitOfWork;

    public SubmitGameRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, int Id, string[] Errors)> Handle(SubmitGameRequestCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Dto.GameTitle))
        {
            return (false, 0, new[] { "Game title is required." });
        }

        var linksString = request.Dto.Links != null && request.Dto.Links.Any()
            ? string.Join("\n", request.Dto.Links.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()))
            : null;

        var gameRequest = new GameRequest
        {
            GameTitle = request.Dto.GameTitle.Trim(),
            ApproximateReleaseYear = request.Dto.ApproximateReleaseYear?.Trim(),
            Platforms = request.Dto.Platforms?.Trim(),
            Links = linksString,
            AdditionalInformation = request.Dto.AdditionalInformation?.Trim(),
            Status = GameRequestStatus.Pending,
            RequestedByUserId = request.UserId,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await _unitOfWork.Repository<GameRequest>().AddAsync(gameRequest);
        await _unitOfWork.SaveChangesAsync();

        return (true, gameRequest.Id, Array.Empty<string>());
    }
}
