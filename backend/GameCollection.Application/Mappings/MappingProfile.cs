using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameCollection.Application.DTOs.Game;
using GameCollection.Application.DTOs.GameRequest;
using GameCollection.Application.DTOs.Library;
using GameCollection.Application.DTOs.Metadata;
using GameCollection.Application.DTOs.RBAC;
using GameCollection.Domain.Entities;

namespace GameCollection.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Developer Mappings
        CreateMap<Developer, DeveloperDto>().ReverseMap();
        CreateMap<CreateDeveloperDto, Developer>();
        CreateMap<UpdateDeveloperDto, Developer>();

        // Publisher Mappings
        CreateMap<Publisher, PublisherDto>().ReverseMap();
        CreateMap<CreatePublisherDto, Publisher>();
        CreateMap<UpdatePublisherDto, Publisher>();

        // Platform Mappings
        CreateMap<Platform, PlatformDto>().ReverseMap();
        CreateMap<CreatePlatformDto, Platform>();
        CreateMap<UpdatePlatformDto, Platform>();

        // DigitalService Mappings
        CreateMap<DigitalService, DigitalServiceDto>().ReverseMap();
        CreateMap<CreateDigitalServiceDto, DigitalService>();
        CreateMap<UpdateDigitalServiceDto, DigitalService>();

        // Genre Mappings
        CreateMap<Genre, GenreDto>().ReverseMap();
        CreateMap<CreateGenreDto, Genre>();
        CreateMap<UpdateGenreDto, Genre>();

        // Tag Mappings
        CreateMap<Tag, TagDto>().ReverseMap();
        CreateMap<CreateTagDto, Tag>();
        CreateMap<UpdateTagDto, Tag>();

        // Theme Mappings
        CreateMap<Theme, ThemeDto>().ReverseMap();
        CreateMap<CreateThemeDto, Theme>();
        CreateMap<UpdateThemeDto, Theme>();

        // Franchise Mappings
        CreateMap<Franchise, FranchiseDto>().ReverseMap();
        CreateMap<CreateFranchiseDto, Franchise>();
        CreateMap<UpdateFranchiseDto, Franchise>();

        // Series Mappings
        CreateMap<Series, SeriesDto>().ReverseMap();
        CreateMap<CreateSeriesDto, Series>();
        CreateMap<UpdateSeriesDto, Series>();

        // RBAC Mappings
        CreateMap<Permission, PermissionDto>();
        CreateMap<ApplicationRole, RoleDto>()
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.RolePermissions.Select(rp => rp.Permission)));

        // Catalog Game Mappings
        CreateMap<Game, GameDto>()
            .ForMember(dest => dest.FranchiseName, opt => opt.MapFrom(src => src.Franchise != null ? src.Franchise.Name : null))
            .ForMember(dest => dest.SeriesName, opt => opt.MapFrom(src => src.Series != null ? src.Series.Name : null))
            .ForMember(dest => dest.Developers, opt => opt.MapFrom(src => src.Developers.Select(d => new MetadataItemDto { Id = d.Id, Name = d.Name }).ToList()))
            .ForMember(dest => dest.Publishers, opt => opt.MapFrom(src => src.Publishers.Select(p => new MetadataItemDto { Id = p.Id, Name = p.Name }).ToList()))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => new MetadataItemDto { Id = g.Id, Name = g.Name }).ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => new MetadataItemDto { Id = t.Id, Name = t.Name }).ToList()))
            .ForMember(dest => dest.Themes, opt => opt.MapFrom(src => src.Themes.Select(t => new MetadataItemDto { Id = t.Id, Name = t.Name }).ToList()))
            .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src => src.Platforms.Select(p => new MetadataItemDto { Id = p.Id, Name = p.Name }).ToList()))
            .ForMember(dest => dest.DigitalServices, opt => opt.MapFrom(src => src.DigitalServices.Select(s => new MetadataItemDto { Id = s.Id, Name = s.Name }).ToList()));

        CreateMap<Game, GameListDto>()
            .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src => src.Platforms.Select(p => p.Name).ToList()))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.Name).ToList()))
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.DigitalServices.Select(s => s.Name).ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name).ToList()));

        // User Library Entry Mappings
        CreateMap<UserLibraryEntry, UserLibraryEntryDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Game.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Game.Description))
            .ForMember(dest => dest.CoverImage, opt => opt.MapFrom(src => src.Game.CoverImage))
            .ForMember(dest => dest.Banner, opt => opt.MapFrom(src => src.Game.Banner))
            .ForMember(dest => dest.BoxArt, opt => opt.MapFrom(src => src.Game.BoxArt))
            .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Game.Logo))
            .ForMember(dest => dest.Screenshots, opt => opt.MapFrom(src => src.Game.Screenshots))
            .ForMember(dest => dest.TrailerUrl, opt => opt.MapFrom(src => src.Game.TrailerUrl))
            .ForMember(dest => dest.YoutubeLinks, opt => opt.MapFrom(src => src.Game.YoutubeLinks))
            .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.Game.ReleaseDate))
            .ForMember(dest => dest.CriticRating, opt => opt.MapFrom(src => src.Game.CriticRating))
            .ForMember(dest => dest.CommunityRating, opt => opt.MapFrom(src => src.Game.CommunityRating))
            .ForMember(dest => dest.MetacriticScore, opt => opt.MapFrom(src => src.Game.MetacriticScore))
            .ForMember(dest => dest.EsrbRating, opt => opt.MapFrom(src => src.Game.EsrbRating))
            .ForMember(dest => dest.PegiRating, opt => opt.MapFrom(src => src.Game.PegiRating))
            .ForMember(dest => dest.FranchiseName, opt => opt.MapFrom(src => src.Game.Franchise != null ? src.Game.Franchise.Name : null))
            .ForMember(dest => dest.SeriesName, opt => opt.MapFrom(src => src.Game.Series != null ? src.Game.Series.Name : null))
            .ForMember(dest => dest.CatalogPlatforms, opt => opt.MapFrom(src => src.Game.Platforms.Select(p => new MetadataItemDto { Id = p.Id, Name = p.Name }).ToList()))
            .ForMember(dest => dest.CatalogServices, opt => opt.MapFrom(src => src.Game.DigitalServices.Select(s => new MetadataItemDto { Id = s.Id, Name = s.Name }).ToList()))
            .ForMember(dest => dest.Developers, opt => opt.MapFrom(src => src.Game.Developers.Select(d => new MetadataItemDto { Id = d.Id, Name = d.Name }).ToList()))
            .ForMember(dest => dest.Publishers, opt => opt.MapFrom(src => src.Game.Publishers.Select(p => new MetadataItemDto { Id = p.Id, Name = p.Name }).ToList()))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Game.Genres.Select(g => new MetadataItemDto { Id = g.Id, Name = g.Name }).ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Game.Tags.Select(t => new MetadataItemDto { Id = t.Id, Name = t.Name }).ToList()))
            .ForMember(dest => dest.Themes, opt => opt.MapFrom(src => src.Game.Themes.Select(t => new MetadataItemDto { Id = t.Id, Name = t.Name }).ToList()))
            .ForMember(dest => dest.UserPlatforms, opt => opt.MapFrom(src => src.Platforms.Select(p => new MetadataItemDto { Id = p.Id, Name = p.Name }).ToList()))
            .ForMember(dest => dest.UserServices, opt => opt.MapFrom(src => src.DigitalServices.Select(s => new MetadataItemDto { Id = s.Id, Name = s.Name }).ToList()));

        CreateMap<UserLibraryEntry, UserLibraryListDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Game.Title))
            .ForMember(dest => dest.CoverImage, opt => opt.MapFrom(src => src.Game.CoverImage))
            .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.Game.ReleaseDate))
            .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src => src.Platforms.Any() 
                ? src.Platforms.Select(p => p.Name).ToList() 
                : src.Game.Platforms.Select(p => p.Name).ToList()))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Game.Genres.Select(g => g.Name).ToList()))
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.DigitalServices.Any()
                ? src.DigitalServices.Select(s => s.Name).ToList()
                : src.Game.DigitalServices.Select(s => s.Name).ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Game.Tags.Select(t => t.Name).ToList()));

        // Game Request Mappings
        CreateMap<GameRequest, GameRequestDto>()
            .ForMember(dest => dest.LinkList, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Links) 
                ? src.Links.Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).ToList() 
                : new List<string>()));
    }
}
