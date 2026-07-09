using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GameCollection.Application.DTOs.Metadata;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MetadataController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public Guid? SystemUserId => Guid.Empty; // Mock or System audit

    public MetadataController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    #region Generic Helper

    private async Task<IActionResult> GetPagedAsync<T, TDto>(
        int pageNumber,
        int pageSize,
        string? search,
        string? sortBy,
        string? sortOrder,
        Func<IQueryable<T>, string, IQueryable<T>> filterFunc,
        Func<IQueryable<T>, string?, string?, IQueryable<T>> sortFunc) where T : class
    {
        var query = _unitOfWork.Repository<T>().GetQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = filterFunc(query, search.Trim().ToLower());
        }

        query = sortFunc(query, sortBy, sortOrder);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<TDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new
        {
            items,
            totalCount,
            pageNumber,
            totalPages,
            hasNextPage = pageNumber < totalPages,
            hasPreviousPage = pageNumber > 1
        });
    }

    private async Task<IActionResult> GetListAsync<T, TDto>() where T : class
    {
        var items = await _unitOfWork.Repository<T>().GetQueryable()
            .ProjectTo<TDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return Ok(items);
    }

    private async Task<IActionResult> GetByIdAsync<T, TDto>(int id) where T : class
    {
        var item = await _unitOfWork.Repository<T>().GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<TDto>(item));
    }

    private async Task<IActionResult> CreateAsync<T, TDto, TCreateDto>(TCreateDto createDto) where T : class
    {
        var item = _mapper.Map<T>(createDto);
        await _unitOfWork.Repository<T>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<TDto>(item);
        return Created("", dto);
    }

    private async Task<IActionResult> UpdateAsync<T, TUpdateDto>(int id, TUpdateDto updateDto) where T : class
    {
        var item = await _unitOfWork.Repository<T>().GetByIdAsync(id);
        if (item == null) return NotFound();

        _mapper.Map(updateDto, item);
        _unitOfWork.Repository<T>().Update(item);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private async Task<IActionResult> DeleteAsync<T>(int id) where T : class
    {
        var item = await _unitOfWork.Repository<T>().GetByIdAsync(id);
        if (item == null) return NotFound();

        _unitOfWork.Repository<T>().Delete(item);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region Platforms

    [HttpGet("platforms")]
    public Task<IActionResult> GetPlatforms([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<Platform, PlatformDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(p => p.Name.ToLower().Contains(s) || (p.Manufacturer != null && p.Manufacturer.ToLower().Contains(s))),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return col?.ToLower() switch
                {
                    "manufacturer" => desc ? q.OrderByDescending(p => p.Manufacturer) : q.OrderBy(p => p.Manufacturer),
                    "releasedate" => desc ? q.OrderByDescending(p => p.ReleaseDate) : q.OrderBy(p => p.ReleaseDate),
                    _ => desc ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name)
                };
            });
    }

    [HttpGet("platforms/list")]
    public Task<IActionResult> GetPlatformsList() => GetListAsync<Platform, PlatformDto>();

    [HttpGet("platforms/{id:int}")]
    public Task<IActionResult> GetPlatform(int id) => GetByIdAsync<Platform, PlatformDto>(id);

    [Authorize(Roles = "Administrator")]
    [HttpPost("platforms")]
    public Task<IActionResult> CreatePlatform([FromBody] CreatePlatformDto dto) => CreateAsync<Platform, PlatformDto, CreatePlatformDto>(dto);

    [Authorize(Roles = "Administrator")]
    [HttpPut("platforms/{id:int}")]
    public Task<IActionResult> UpdatePlatform(int id, [FromBody] UpdatePlatformDto dto) => UpdateAsync<Platform, UpdatePlatformDto>(id, dto);

    [Authorize(Roles = "Administrator")]
    [HttpDelete("platforms/{id:int}")]
    public Task<IActionResult> DeletePlatform(int id) => DeleteAsync<Platform>(id);

    #endregion

    #region Services

    [HttpGet("services")]
    public Task<IActionResult> GetServices([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<DigitalService, DigitalServiceDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s) || (x.Website != null && x.Website.ToLower().Contains(s))),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name);
            });
    }

    [HttpGet("services/list")]
    public Task<IActionResult> GetServicesList() => GetListAsync<DigitalService, DigitalServiceDto>();

    [HttpGet("services/{id:int}")]
    public Task<IActionResult> GetService(int id) => GetByIdAsync<DigitalService, DigitalServiceDto>(id);

    [Authorize(Roles = "Administrator")]
    [HttpPost("services")]
    public Task<IActionResult> CreateService([FromBody] CreateDigitalServiceDto dto) => CreateAsync<DigitalService, DigitalServiceDto, CreateDigitalServiceDto>(dto);

    [Authorize(Roles = "Administrator")]
    [HttpPut("services/{id:int}")]
    public Task<IActionResult> UpdateService(int id, [FromBody] UpdateDigitalServiceDto dto) => UpdateAsync<DigitalService, UpdateDigitalServiceDto>(id, dto);

    [Authorize(Roles = "Administrator")]
    [HttpDelete("services/{id:int}")]
    public Task<IActionResult> DeleteService(int id) => DeleteAsync<DigitalService>(id);

    #endregion

    #region Developers

    [HttpGet("developers")]
    public Task<IActionResult> GetDevelopers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<Developer, DeveloperDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s) || (x.Country != null && x.Country.ToLower().Contains(s))),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name);
            });
    }

    [HttpGet("developers/list")]
    public Task<IActionResult> GetDevelopersList() => GetListAsync<Developer, DeveloperDto>();

    [HttpGet("developers/{id:int}")]
    public Task<IActionResult> GetDeveloper(int id) => GetByIdAsync<Developer, DeveloperDto>(id);

    [Authorize(Roles = "Administrator")]
    [HttpPost("developers")]
    public Task<IActionResult> CreateDeveloper([FromBody] CreateDeveloperDto dto) => CreateAsync<Developer, DeveloperDto, CreateDeveloperDto>(dto);

    [Authorize(Roles = "Administrator")]
    [HttpPut("developers/{id:int}")]
    public Task<IActionResult> UpdateDeveloper(int id, [FromBody] UpdateDeveloperDto dto) => UpdateAsync<Developer, UpdateDeveloperDto>(id, dto);

    [Authorize(Roles = "Administrator")]
    [HttpDelete("developers/{id:int}")]
    public Task<IActionResult> DeleteDeveloper(int id) => DeleteAsync<Developer>(id);

    #endregion

    #region Publishers

    [HttpGet("publishers")]
    public Task<IActionResult> GetPublishers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<Publisher, PublisherDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s) || (x.Country != null && x.Country.ToLower().Contains(s))),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name);
            });
    }

    [HttpGet("publishers/list")]
    public Task<IActionResult> GetPublishersList() => GetListAsync<Publisher, PublisherDto>();

    [HttpGet("publishers/{id:int}")]
    public Task<IActionResult> GetPublisher(int id) => GetByIdAsync<Publisher, PublisherDto>(id);

    [Authorize(Roles = "Administrator")]
    [HttpPost("publishers")]
    public Task<IActionResult> CreatePublisher([FromBody] CreatePublisherDto dto) => CreateAsync<Publisher, PublisherDto, CreatePublisherDto>(dto);

    [Authorize(Roles = "Administrator")]
    [HttpPut("publishers/{id:int}")]
    public Task<IActionResult> UpdatePublisher(int id, [FromBody] UpdatePublisherDto dto) => UpdateAsync<Publisher, UpdatePublisherDto>(id, dto);

    [Authorize(Roles = "Administrator")]
    [HttpDelete("publishers/{id:int}")]
    public Task<IActionResult> DeletePublisher(int id) => DeleteAsync<Publisher>(id);

    #endregion

    #region Genres

    [HttpGet("genres")]
    public Task<IActionResult> GetGenres([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<Genre, GenreDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s)),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name);
            });
    }

    [HttpGet("genres/list")]
    public Task<IActionResult> GetGenresList() => GetListAsync<Genre, GenreDto>();

    [HttpGet("genres/{id:int}")]
    public Task<IActionResult> GetGenre(int id) => GetByIdAsync<Genre, GenreDto>(id);

    [Authorize(Roles = "Administrator")]
    [HttpPost("genres")]
    public Task<IActionResult> CreateGenre([FromBody] CreateGenreDto dto) => CreateAsync<Genre, GenreDto, CreateGenreDto>(dto);

    [Authorize(Roles = "Administrator")]
    [HttpPut("genres/{id:int}")]
    public Task<IActionResult> UpdateGenre(int id, [FromBody] UpdateGenreDto dto) => UpdateAsync<Genre, UpdateGenreDto>(id, dto);

    [Authorize(Roles = "Administrator")]
    [HttpDelete("genres/{id:int}")]
    public Task<IActionResult> DeleteGenre(int id) => DeleteAsync<Genre>(id);

    #endregion

    #region Tags

    [HttpGet("tags")]
    public Task<IActionResult> GetTags([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<Tag, TagDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s)),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name);
            });
    }

    [HttpGet("tags/list")]
    public Task<IActionResult> GetTagsList() => GetListAsync<Tag, TagDto>();

    [HttpGet("tags/{id:int}")]
    public Task<IActionResult> GetTag(int id) => GetByIdAsync<Tag, TagDto>(id);

    [Authorize(Roles = "Administrator")]
    [HttpPost("tags")]
    public Task<IActionResult> CreateTag([FromBody] CreateTagDto dto) => CreateAsync<Tag, TagDto, CreateTagDto>(dto);

    [Authorize(Roles = "Administrator")]
    [HttpPut("tags/{id:int}")]
    public Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagDto dto) => UpdateAsync<Tag, UpdateTagDto>(id, dto);

    [Authorize(Roles = "Administrator")]
    [HttpDelete("tags/{id:int}")]
    public Task<IActionResult> DeleteTag(int id) => DeleteAsync<Tag>(id);

    #endregion

    #region Themes

    [HttpGet("themes")]
    public Task<IActionResult> GetThemes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<Theme, ThemeDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s)),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name);
            });
    }

    [HttpGet("themes/list")]
    public Task<IActionResult> GetThemesList() => GetListAsync<Theme, ThemeDto>();

    [HttpGet("themes/{id:int}")]
    public Task<IActionResult> GetTheme(int id) => GetByIdAsync<Theme, ThemeDto>(id);

    [Authorize(Roles = "Administrator")]
    [HttpPost("themes")]
    public Task<IActionResult> CreateTheme([FromBody] CreateThemeDto dto) => CreateAsync<Theme, ThemeDto, CreateThemeDto>(dto);

    [Authorize(Roles = "Administrator")]
    [HttpPut("themes/{id:int}")]
    public Task<IActionResult> UpdateTheme(int id, [FromBody] UpdateThemeDto dto) => UpdateAsync<Theme, UpdateThemeDto>(id, dto);

    [Authorize(Roles = "Administrator")]
    [HttpDelete("themes/{id:int}")]
    public Task<IActionResult> DeleteTheme(int id) => DeleteAsync<Theme>(id);

    #endregion

    #region Franchises

    [HttpGet("franchises")]
    public Task<IActionResult> GetFranchises([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<Franchise, FranchiseDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s)),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name);
            });
    }

    [HttpGet("franchises/list")]
    public Task<IActionResult> GetFranchisesList() => GetListAsync<Franchise, FranchiseDto>();

    [HttpGet("franchises/{id:int}")]
    public Task<IActionResult> GetFranchise(int id) => GetByIdAsync<Franchise, FranchiseDto>(id);

    [Authorize(Roles = "Administrator")]
    [HttpPost("franchises")]
    public Task<IActionResult> CreateFranchise([FromBody] CreateFranchiseDto dto) => CreateAsync<Franchise, FranchiseDto, CreateFranchiseDto>(dto);

    [Authorize(Roles = "Administrator")]
    [HttpPut("franchises/{id:int}")]
    public Task<IActionResult> UpdateFranchise(int id, [FromBody] UpdateFranchiseDto dto) => UpdateAsync<Franchise, UpdateFranchiseDto>(id, dto);

    [Authorize(Roles = "Administrator")]
    [HttpDelete("franchises/{id:int}")]
    public Task<IActionResult> DeleteFranchise(int id) => DeleteAsync<Franchise>(id);

    #endregion

    #region Series

    [HttpGet("series")]
    public Task<IActionResult> GetSeries([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<Series, SeriesDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s)),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name);
            });
    }

    [HttpGet("series/list")]
    public Task<IActionResult> GetSeriesList() => GetListAsync<Series, SeriesDto>();

    [HttpGet("series/{id:int}")]
    public Task<IActionResult> GetSeries(int id) => GetByIdAsync<Series, SeriesDto>(id);

    [Authorize(Roles = "Administrator")]
    [HttpPost("series")]
    public Task<IActionResult> CreateSeries([FromBody] CreateSeriesDto dto) => CreateAsync<Series, SeriesDto, CreateSeriesDto>(dto);

    [Authorize(Roles = "Administrator")]
    [HttpPut("series/{id:int}")]
    public Task<IActionResult> UpdateSeries(int id, [FromBody] UpdateSeriesDto dto) => UpdateAsync<Series, UpdateSeriesDto>(id, dto);

    [Authorize(Roles = "Administrator")]
    [HttpDelete("series/{id:int}")]
    public Task<IActionResult> DeleteSeries(int id) => DeleteAsync<Series>(id);

    #endregion
}
