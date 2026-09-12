using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.Games.Commands.CreateGame;

public record CreateGameCommand : IRequest<int>
{
    public string Title { get; init; } = null!;
    public string? AlternateTitles { get; init; }
    public string? OriginalTitle { get; init; }
    public string? Description { get; init; }
    public string? Notes { get; init; }

    public double? CommunityRating { get; init; }
    public double? CriticRating { get; init; }

    public DateTimeOffset? ReleaseDate { get; init; }
    public DateTimeOffset? OriginalReleaseDate { get; init; }
    public DateTimeOffset? EarlyAccessDate { get; init; }

    public string? CoverImage { get; init; }
    public string? BoxArt { get; init; }
    public string? Banner { get; init; }
    public string? Logo { get; init; }
    public string? Screenshots { get; init; }
    public string? Artwork { get; init; }
    public string? FanArt { get; init; }
    public string? TrailerUrl { get; init; }
    public string? GameplayUrl { get; init; }
    public string? YoutubeLinks { get; init; }

    public string? EsrbRating { get; init; }
    public string? PegiRating { get; init; }
    public int? MetacriticScore { get; init; }
    public int? OpenCriticScore { get; init; }
    public bool MultiplayerSupport { get; init; }
    public bool CoopSupport { get; init; }
    public bool VrSupport { get; init; }
    public bool CrossplaySupport { get; init; }
    public bool CloudSaveSupport { get; init; }
    public bool ControllerSupport { get; init; }
    public string? SteamDeckCompatibility { get; init; }
    public int AchievementCount { get; init; }
    public int DlcCount { get; init; }
    public int ExpansionCount { get; init; }

    public int? FranchiseId { get; init; }
    public int? SeriesId { get; init; }

    public List<int> DeveloperIds { get; init; } = new();
    public List<int> PublisherIds { get; init; } = new();
    public List<int> GenreIds { get; init; } = new();
    public List<int> TagIds { get; init; } = new();
    public List<int> ThemeIds { get; init; } = new();
    public List<int> PlatformIds { get; init; } = new();
    public List<int> ServiceIds { get; init; } = new();
}

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateGameCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var game = new Game
        {
            Title = request.Title.Trim(),
            AlternateTitles = request.AlternateTitles,
            OriginalTitle = request.OriginalTitle,
            Description = request.Description,
            Notes = request.Notes,
            CommunityRating = request.CommunityRating,
            CriticRating = request.CriticRating,
            ReleaseDate = request.ReleaseDate,
            OriginalReleaseDate = request.OriginalReleaseDate,
            EarlyAccessDate = request.EarlyAccessDate,
            CoverImage = request.CoverImage,
            BoxArt = request.BoxArt,
            Banner = request.Banner,
            Logo = request.Logo,
            Screenshots = request.Screenshots,
            Artwork = request.Artwork,
            FanArt = request.FanArt,
            TrailerUrl = request.TrailerUrl,
            GameplayUrl = request.GameplayUrl,
            YoutubeLinks = request.YoutubeLinks,
            EsrbRating = request.EsrbRating,
            PegiRating = request.PegiRating,
            MetacriticScore = request.MetacriticScore,
            OpenCriticScore = request.OpenCriticScore,
            MultiplayerSupport = request.MultiplayerSupport,
            CoopSupport = request.CoopSupport,
            VrSupport = request.VrSupport,
            CrossplaySupport = request.CrossplaySupport,
            CloudSaveSupport = request.CloudSaveSupport,
            ControllerSupport = request.ControllerSupport,
            SteamDeckCompatibility = request.SteamDeckCompatibility,
            AchievementCount = request.AchievementCount,
            DlcCount = request.DlcCount,
            ExpansionCount = request.ExpansionCount,
            FranchiseId = request.FranchiseId,
            SeriesId = request.SeriesId
        };

        if (request.DeveloperIds.Any())
        {
            game.Developers = await _unitOfWork.Repository<Developer>().GetQueryable()
                .Where(d => request.DeveloperIds.Contains(d.Id)).ToListAsync(cancellationToken);
        }
        if (request.PublisherIds.Any())
        {
            game.Publishers = await _unitOfWork.Repository<Publisher>().GetQueryable()
                .Where(p => request.PublisherIds.Contains(p.Id)).ToListAsync(cancellationToken);
        }
        if (request.GenreIds.Any())
        {
            game.Genres = await _unitOfWork.Repository<Genre>().GetQueryable()
                .Where(g => request.GenreIds.Contains(g.Id)).ToListAsync(cancellationToken);
        }
        if (request.TagIds.Any())
        {
            game.Tags = await _unitOfWork.Repository<Tag>().GetQueryable()
                .Where(t => request.TagIds.Contains(t.Id)).ToListAsync(cancellationToken);
        }
        if (request.ThemeIds.Any())
        {
            game.Themes = await _unitOfWork.Repository<Theme>().GetQueryable()
                .Where(t => request.ThemeIds.Contains(t.Id)).ToListAsync(cancellationToken);
        }
        if (request.PlatformIds.Any())
        {
            game.Platforms = await _unitOfWork.Repository<Platform>().GetQueryable()
                .Where(p => request.PlatformIds.Contains(p.Id)).ToListAsync(cancellationToken);
        }
        if (request.ServiceIds.Any())
        {
            game.DigitalServices = await _unitOfWork.Repository<DigitalService>().GetQueryable()
                .Where(s => request.ServiceIds.Contains(s.Id)).ToListAsync(cancellationToken);
        }

        await _unitOfWork.Repository<Game>().AddAsync(game);
        await _unitOfWork.SaveChangesAsync();

        return game.Id;
    }
}
