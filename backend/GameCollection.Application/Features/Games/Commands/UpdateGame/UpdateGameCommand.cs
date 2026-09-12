using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Application.Features.Games.Commands.UpdateGame;

public record UpdateGameCommand : IRequest<bool>
{
    public int Id { get; init; }
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

public class UpdateGameCommandHandler : IRequestHandler<UpdateGameCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGameCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _unitOfWork.Repository<Game>().GetQueryable()
            .Include(g => g.Developers)
            .Include(g => g.Publishers)
            .Include(g => g.Genres)
            .Include(g => g.Tags)
            .Include(g => g.Themes)
            .Include(g => g.Platforms)
            .Include(g => g.DigitalServices)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (game == null) return false;

        // Update fields
        game.Title = request.Title.Trim();
        game.AlternateTitles = request.AlternateTitles;
        game.OriginalTitle = request.OriginalTitle;
        game.Description = request.Description;
        game.Notes = request.Notes;
        game.CommunityRating = request.CommunityRating;
        game.CriticRating = request.CriticRating;
        game.ReleaseDate = request.ReleaseDate;
        game.OriginalReleaseDate = request.OriginalReleaseDate;
        game.EarlyAccessDate = request.EarlyAccessDate;
        
        if (!string.IsNullOrWhiteSpace(request.CoverImage)) game.CoverImage = request.CoverImage;
        if (!string.IsNullOrWhiteSpace(request.BoxArt)) game.BoxArt = request.BoxArt;
        if (!string.IsNullOrWhiteSpace(request.Banner)) game.Banner = request.Banner;
        if (!string.IsNullOrWhiteSpace(request.Logo)) game.Logo = request.Logo;
        if (!string.IsNullOrWhiteSpace(request.Screenshots)) game.Screenshots = request.Screenshots;
        if (!string.IsNullOrWhiteSpace(request.Artwork)) game.Artwork = request.Artwork;
        if (!string.IsNullOrWhiteSpace(request.FanArt)) game.FanArt = request.FanArt;
        
        game.TrailerUrl = request.TrailerUrl;
        game.GameplayUrl = request.GameplayUrl;
        game.YoutubeLinks = request.YoutubeLinks;
        game.EsrbRating = request.EsrbRating;
        game.PegiRating = request.PegiRating;
        game.MetacriticScore = request.MetacriticScore;
        game.OpenCriticScore = request.OpenCriticScore;
        game.MultiplayerSupport = request.MultiplayerSupport;
        game.CoopSupport = request.CoopSupport;
        game.VrSupport = request.VrSupport;
        game.CrossplaySupport = request.CrossplaySupport;
        game.CloudSaveSupport = request.CloudSaveSupport;
        game.ControllerSupport = request.ControllerSupport;
        game.SteamDeckCompatibility = request.SteamDeckCompatibility;
        game.AchievementCount = request.AchievementCount;
        game.DlcCount = request.DlcCount;
        game.ExpansionCount = request.ExpansionCount;
        game.FranchiseId = request.FranchiseId;
        game.SeriesId = request.SeriesId;

        // Sync Developers
        game.Developers.Clear();
        if (request.DeveloperIds.Any())
        {
            game.Developers = await _unitOfWork.Repository<Developer>().GetQueryable()
                .Where(d => request.DeveloperIds.Contains(d.Id)).ToListAsync(cancellationToken);
        }

        // Sync Publishers
        game.Publishers.Clear();
        if (request.PublisherIds.Any())
        {
            game.Publishers = await _unitOfWork.Repository<Publisher>().GetQueryable()
                .Where(p => request.PublisherIds.Contains(p.Id)).ToListAsync(cancellationToken);
        }

        // Sync Genres
        game.Genres.Clear();
        if (request.GenreIds.Any())
        {
            game.Genres = await _unitOfWork.Repository<Genre>().GetQueryable()
                .Where(g => request.GenreIds.Contains(g.Id)).ToListAsync(cancellationToken);
        }

        // Sync Tags
        game.Tags.Clear();
        if (request.TagIds.Any())
        {
            game.Tags = await _unitOfWork.Repository<Tag>().GetQueryable()
                .Where(t => request.TagIds.Contains(t.Id)).ToListAsync(cancellationToken);
        }

        // Sync Themes
        game.Themes.Clear();
        if (request.ThemeIds.Any())
        {
            game.Themes = await _unitOfWork.Repository<Theme>().GetQueryable()
                .Where(t => request.ThemeIds.Contains(t.Id)).ToListAsync(cancellationToken);
        }

        // Sync Platforms
        game.Platforms.Clear();
        if (request.PlatformIds.Any())
        {
            game.Platforms = await _unitOfWork.Repository<Platform>().GetQueryable()
                .Where(p => request.PlatformIds.Contains(p.Id)).ToListAsync(cancellationToken);
        }

        // Sync Services
        game.DigitalServices.Clear();
        if (request.ServiceIds.Any())
        {
            game.DigitalServices = await _unitOfWork.Repository<DigitalService>().GetQueryable()
                .Where(s => request.ServiceIds.Contains(s.Id)).ToListAsync(cancellationToken);
        }

        _unitOfWork.Repository<Game>().Update(game);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
