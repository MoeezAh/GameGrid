using System.Security.Claims;
using System.Threading.Tasks;
using GameCollection.Application.Common.Interfaces;
using GameCollection.Application.Common.Security;
using GameCollection.Application.DTOs.Library;
using GameCollection.Application.Features.Libraries.Commands.AddGameToLibrary;
using GameCollection.Application.Features.Libraries.Commands.RemoveGameFromLibrary;
using GameCollection.Application.Features.Libraries.Commands.UpdateLibraryEntry;
using GameCollection.Application.Features.Libraries.Queries.GetLibraryEntryById;
using GameCollection.Application.Features.Libraries.Queries.GetMyLibrary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCollection.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LibrariesController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IPermissionService _permissionService;

    public LibrariesController(ISender mediator, IPermissionService permissionService)
    {
        _mediator = mediator;
        _permissionService = permissionService;
    }

    [HttpGet]
    [HasPermission(Permissions.Libraries.ViewOwn)]
    public async Task<IActionResult> GetMyLibrary([FromQuery] GetMyLibraryQuery query)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        var queryWithUser = query with { UserId = currentUserId };
        var result = await _mediator.Send(queryWithUser);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [HasPermission(Permissions.Libraries.ViewOwn)]
    public async Task<IActionResult> GetLibraryEntry(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        var canViewAny = await _permissionService.HasPermissionAsync(currentUserId, Permissions.Libraries.ViewAny);
        var entry = await _mediator.Send(new GetLibraryEntryByIdQuery(id, currentUserId, canViewAny));

        if (entry == null) return NotFound($"Library entry with ID {id} not found or access denied.");

        return Ok(entry);
    }

    [HttpPost]
    [HasPermission(Permissions.Libraries.AddGame)]
    public async Task<IActionResult> AddToLibrary([FromBody] AddGameToLibraryDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        var (success, id, errors) = await _mediator.Send(new AddGameToLibraryCommand(dto, currentUserId));
        if (!success)
        {
            return BadRequest(new { errors });
        }

        return CreatedAtAction(nameof(GetLibraryEntry), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [HasPermission(Permissions.Libraries.UpdateOwn)]
    public async Task<IActionResult> UpdateLibraryEntry(int id, [FromBody] UpdateLibraryEntryDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        var canUpdateAny = await _permissionService.HasPermissionAsync(currentUserId, Permissions.Libraries.UpdateAny);
        var success = await _mediator.Send(new UpdateLibraryEntryCommand(id, dto, currentUserId, canUpdateAny));

        if (!success) return NotFound($"Library entry with ID {id} not found or access denied.");

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [HasPermission(Permissions.Libraries.RemoveGame)]
    public async Task<IActionResult> RemoveFromLibrary(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        var canRemoveAny = await _permissionService.HasPermissionAsync(currentUserId, Permissions.Libraries.UpdateAny);
        var success = await _mediator.Send(new RemoveGameFromLibraryCommand(id, currentUserId, canRemoveAny));

        if (!success) return NotFound($"Library entry with ID {id} not found or access denied.");

        return NoContent();
    }
}
