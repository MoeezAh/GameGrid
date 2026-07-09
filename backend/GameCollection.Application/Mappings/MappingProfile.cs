using System.Linq;
using AutoMapper;
using GameCollection.Application.DTOs.Game;
using GameCollection.Application.DTOs.Metadata;
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

        // Game Mappings
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
    }
}
