using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GameCollection.Application.Common.Security;
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
            pageNumber,
            pageSize,
            totalPages,
            totalCount,
            hasPreviousPage = pageNumber > 1,
            hasNextPage = pageNumber < totalPages
        });
    }

    private async Task<IActionResult> GetByIdAsync<T, TDto>(int id) where T : class
    {
        var entity = await _unitOfWork.Repository<T>().GetByIdAsync(id);
        if (entity == null) return NotFound();
        return Ok(_mapper.Map<TDto>(entity));
    }

    private async Task<IActionResult> GetListAsync<T, TDto>() where T : class
    {
        var items = await _unitOfWork.Repository<T>().GetQueryable()
            .ProjectTo<TDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return Ok(items);
    }

    private async Task<IActionResult> CreateAsync<T, TDto, TCreateDto>(TCreateDto dto) where T : class
    {
        var entity = _mapper.Map<T>(dto);
        await _unitOfWork.Repository<T>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return Ok(_mapper.Map<TDto>(entity));
    }

    private async Task<IActionResult> UpdateAsync<T, TUpdateDto>(int id, TUpdateDto dto) where T : class
    {
        var entity = await _unitOfWork.Repository<T>().GetByIdAsync(id);
        if (entity == null) return NotFound();
        _mapper.Map(dto, entity);
        _unitOfWork.Repository<T>().Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }

    private async Task<IActionResult> DeleteAsync<T>(int id) where T : class
    {
        var entity = await _unitOfWork.Repository<T>().GetByIdAsync(id);
        if (entity == null) return NotFound();
        _unitOfWork.Repository<T>().Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }

    #endregion

    #region Platforms

    [HttpGet("platforms")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetPlatforms([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<Platform, PlatformDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s) || (x.Manufacturer != null && x.Manufacturer.ToLower().Contains(s))),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return col?.ToLower() switch
                {
                    "manufacturer" => desc ? q.OrderByDescending(x => x.Manufacturer) : q.OrderBy(x => x.Manufacturer),
                    "generation" => desc ? q.OrderByDescending(x => x.Generation) : q.OrderBy(x => x.Generation),
                    _ => desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name)
                };
            });
    }

    [HttpGet("platforms/list")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetPlatformsList() => GetListAsync<Platform, PlatformDto>();

    [HttpGet("platforms/{id:int}")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetPlatform(int id) => GetByIdAsync<Platform, PlatformDto>(id);

    [HttpPost("platforms")]
    [HasPermission(Permissions.Metadata.ManagePlatforms)]
    public Task<IActionResult> CreatePlatform([FromBody] CreatePlatformDto dto) => CreateAsync<Platform, PlatformDto, CreatePlatformDto>(dto);

    [HttpPut("platforms/{id:int}")]
    [HasPermission(Permissions.Metadata.ManagePlatforms)]
    public Task<IActionResult> UpdatePlatform(int id, [FromBody] UpdatePlatformDto dto) => UpdateAsync<Platform, UpdatePlatformDto>(id, dto);

    [HttpDelete("platforms/{id:int}")]
    [HasPermission(Permissions.Metadata.ManagePlatforms)]
    public Task<IActionResult> DeletePlatform(int id) => DeleteAsync<Platform>(id);

    #endregion

    #region Digital Services

    [HttpGet("services")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetServices([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<DigitalService, DigitalServiceDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s)),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name);
            });
    }

    [HttpGet("services/list")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetServicesList() => GetListAsync<DigitalService, DigitalServiceDto>();

    [HttpGet("services/{id:int}")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetService(int id) => GetByIdAsync<DigitalService, DigitalServiceDto>(id);

    [HttpPost("services")]
    [HasPermission(Permissions.Metadata.ManageServices)]
    public Task<IActionResult> CreateService([FromBody] CreateDigitalServiceDto dto) => CreateAsync<DigitalService, DigitalServiceDto, CreateDigitalServiceDto>(dto);

    [HttpPut("services/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageServices)]
    public Task<IActionResult> UpdateService(int id, [FromBody] UpdateDigitalServiceDto dto) => UpdateAsync<DigitalService, UpdateDigitalServiceDto>(id, dto);

    [HttpDelete("services/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageServices)]
    public Task<IActionResult> DeleteService(int id) => DeleteAsync<DigitalService>(id);

    #endregion

    #region Developers

    [HttpGet("developers")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetDevelopers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<Developer, DeveloperDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s) || (x.Country != null && x.Country.ToLower().Contains(s))),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return col?.ToLower() switch
                {
                    "country" => desc ? q.OrderByDescending(x => x.Country) : q.OrderBy(x => x.Country),
                    _ => desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name)
                };
            });
    }

    [HttpGet("developers/list")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetDevelopersList() => GetListAsync<Developer, DeveloperDto>();

    [HttpGet("developers/{id:int}")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetDeveloper(int id) => GetByIdAsync<Developer, DeveloperDto>(id);

    [HttpPost("developers")]
    [HasPermission(Permissions.Metadata.ManageDevelopers)]
    public Task<IActionResult> CreateDeveloper([FromBody] CreateDeveloperDto dto) => CreateAsync<Developer, DeveloperDto, CreateDeveloperDto>(dto);

    [HttpPut("developers/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageDevelopers)]
    public Task<IActionResult> UpdateDeveloper(int id, [FromBody] UpdateDeveloperDto dto) => UpdateAsync<Developer, UpdateDeveloperDto>(id, dto);

    [HttpDelete("developers/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageDevelopers)]
    public Task<IActionResult> DeleteDeveloper(int id) => DeleteAsync<Developer>(id);

    #endregion

    #region Publishers

    [HttpGet("publishers")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetPublishers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        return GetPagedAsync<Publisher, PublisherDto>(pageNumber, pageSize, search, sortBy, sortOrder,
            (q, s) => q.Where(x => x.Name.ToLower().Contains(s) || (x.Country != null && x.Country.ToLower().Contains(s))),
            (q, col, ord) =>
            {
                bool desc = string.Equals(ord, "desc", StringComparison.OrdinalIgnoreCase);
                return col?.ToLower() switch
                {
                    "country" => desc ? q.OrderByDescending(x => x.Country) : q.OrderBy(x => x.Country),
                    _ => desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name)
                };
            });
    }

    [HttpGet("publishers/list")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetPublishersList() => GetListAsync<Publisher, PublisherDto>();

    [HttpGet("publishers/{id:int}")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetPublisher(int id) => GetByIdAsync<Publisher, PublisherDto>(id);

    [HttpPost("publishers")]
    [HasPermission(Permissions.Metadata.ManagePublishers)]
    public Task<IActionResult> CreatePublisher([FromBody] CreatePublisherDto dto) => CreateAsync<Publisher, PublisherDto, CreatePublisherDto>(dto);

    [HttpPut("publishers/{id:int}")]
    [HasPermission(Permissions.Metadata.ManagePublishers)]
    public Task<IActionResult> UpdatePublisher(int id, [FromBody] UpdatePublisherDto dto) => UpdateAsync<Publisher, UpdatePublisherDto>(id, dto);

    [HttpDelete("publishers/{id:int}")]
    [HasPermission(Permissions.Metadata.ManagePublishers)]
    public Task<IActionResult> DeletePublisher(int id) => DeleteAsync<Publisher>(id);

    #endregion

    #region Genres

    [HttpGet("genres")]
    [HasPermission(Permissions.Metadata.View)]
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
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetGenresList() => GetListAsync<Genre, GenreDto>();

    [HttpGet("genres/{id:int}")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetGenre(int id) => GetByIdAsync<Genre, GenreDto>(id);

    [HttpPost("genres")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> CreateGenre([FromBody] CreateGenreDto dto) => CreateAsync<Genre, GenreDto, CreateGenreDto>(dto);

    [HttpPut("genres/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> UpdateGenre(int id, [FromBody] UpdateGenreDto dto) => UpdateAsync<Genre, UpdateGenreDto>(id, dto);

    [HttpDelete("genres/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> DeleteGenre(int id) => DeleteAsync<Genre>(id);

    #endregion

    #region Tags

    [HttpGet("tags")]
    [HasPermission(Permissions.Metadata.View)]
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
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetTagsList() => GetListAsync<Tag, TagDto>();

    [HttpGet("tags/{id:int}")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetTag(int id) => GetByIdAsync<Tag, TagDto>(id);

    [HttpPost("tags")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> CreateTag([FromBody] CreateTagDto dto) => CreateAsync<Tag, TagDto, CreateTagDto>(dto);

    [HttpPut("tags/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagDto dto) => UpdateAsync<Tag, UpdateTagDto>(id, dto);

    [HttpDelete("tags/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> DeleteTag(int id) => DeleteAsync<Tag>(id);

    #endregion

    #region Themes

    [HttpGet("themes")]
    [HasPermission(Permissions.Metadata.View)]
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
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetThemesList() => GetListAsync<Theme, ThemeDto>();

    [HttpGet("themes/{id:int}")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetTheme(int id) => GetByIdAsync<Theme, ThemeDto>(id);

    [HttpPost("themes")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> CreateTheme([FromBody] CreateThemeDto dto) => CreateAsync<Theme, ThemeDto, CreateThemeDto>(dto);

    [HttpPut("themes/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> UpdateTheme(int id, [FromBody] UpdateThemeDto dto) => UpdateAsync<Theme, UpdateThemeDto>(id, dto);

    [HttpDelete("themes/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> DeleteTheme(int id) => DeleteAsync<Theme>(id);

    #endregion

    #region Franchises

    [HttpGet("franchises")]
    [HasPermission(Permissions.Metadata.View)]
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
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetFranchisesList() => GetListAsync<Franchise, FranchiseDto>();

    [HttpGet("franchises/{id:int}")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetFranchise(int id) => GetByIdAsync<Franchise, FranchiseDto>(id);

    [HttpPost("franchises")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> CreateFranchise([FromBody] CreateFranchiseDto dto) => CreateAsync<Franchise, FranchiseDto, CreateFranchiseDto>(dto);

    [HttpPut("franchises/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> UpdateFranchise(int id, [FromBody] UpdateFranchiseDto dto) => UpdateAsync<Franchise, UpdateFranchiseDto>(id, dto);

    [HttpDelete("franchises/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> DeleteFranchise(int id) => DeleteAsync<Franchise>(id);

    #endregion

    #region Series

    [HttpGet("series")]
    [HasPermission(Permissions.Metadata.View)]
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
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetSeriesList() => GetListAsync<Series, SeriesDto>();

    [HttpGet("series/{id:int}")]
    [HasPermission(Permissions.Metadata.View)]
    public Task<IActionResult> GetSeries(int id) => GetByIdAsync<Series, SeriesDto>(id);

    [HttpPost("series")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> CreateSeries([FromBody] CreateSeriesDto dto) => CreateAsync<Series, SeriesDto, CreateSeriesDto>(dto);

    [HttpPut("series/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> UpdateSeries(int id, [FromBody] UpdateSeriesDto dto) => UpdateAsync<Series, UpdateSeriesDto>(id, dto);

    [HttpDelete("series/{id:int}")]
    [HasPermission(Permissions.Metadata.ManageTaxonomies)]
    public Task<IActionResult> DeleteSeries(int id) => DeleteAsync<Series>(id);

    #endregion
}
