using System.Security.Claims;
using System.Threading.Tasks;
using GameCollection.Application.Common.Interfaces;
using GameCollection.Application.Common.Security;
using GameCollection.Application.DTOs.GameRequest;
using GameCollection.Application.Features.GameRequests.Commands.ApproveGameRequest;
using GameCollection.Application.Features.GameRequests.Commands.DeleteGameRequest;
using GameCollection.Application.Features.GameRequests.Commands.RejectGameRequest;
using GameCollection.Application.Features.GameRequests.Commands.SubmitGameRequest;
using GameCollection.Application.Features.GameRequests.Queries.CheckDuplicateGame;
using GameCollection.Application.Features.GameRequests.Queries.GetAllGameRequests;
using GameCollection.Application.Features.GameRequests.Queries.GetMyGameRequests;
using GameCollection.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCollection.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GameRequestsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IPermissionService _permissionService;

    public GameRequestsController(ISender mediator, IPermissionService permissionService)
    {
        _mediator = mediator;
        _permissionService = permissionService;
    }

    [HttpPost]
    [HasPermission(Permissions.GameRequests.Create)]
    public async Task<IActionResult> SubmitRequest([FromBody] SubmitGameRequestDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        var (success, id, errors) = await _mediator.Send(new SubmitGameRequestCommand(dto, currentUserId));
        if (!success)
        {
            return BadRequest(new { errors });
        }

        return Ok(new { id, message = "Game request submitted successfully." });
    }

    [HttpGet("my")]
    [HasPermission(Permissions.GameRequests.ViewOwn)]
    public async Task<IActionResult> GetMyRequests()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        var requests = await _mediator.Send(new GetMyGameRequestsQuery(currentUserId));
        return Ok(requests);
    }

    [HttpGet]
    [HasPermission(Permissions.GameRequests.ViewAny)]
    public async Task<IActionResult> GetAllRequests([FromQuery] GameRequestStatus? status = null)
    {
        var requests = await _mediator.Send(new GetAllGameRequestsQuery(status));
        return Ok(requests);
    }

    [HttpGet("check-duplicate")]
    [HasPermission(Permissions.GameRequests.Approve)]
    public async Task<IActionResult> CheckDuplicate([FromQuery] string title)
    {
        var result = await _mediator.Send(new CheckDuplicateGameQuery(title));
        return Ok(result);
    }

    [HttpPost("{id:int}/approve")]
    [HasPermission(Permissions.GameRequests.Approve)]
    public async Task<IActionResult> ApproveRequest(int id, [FromBody] ApproveGameRequestDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        var (success, gameId, errors) = await _mediator.Send(new ApproveGameRequestCommand(id, dto, currentUserId));
        if (!success)
        {
            return BadRequest(new { errors });
        }

        return Ok(new { message = "Game request approved successfully.", gameId });
    }

    [HttpPost("{id:int}/reject")]
    [HasPermission(Permissions.GameRequests.Reject)]
    public async Task<IActionResult> RejectRequest(int id, [FromBody] RejectGameRequestDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        var (success, errors) = await _mediator.Send(new RejectGameRequestCommand(id, dto, currentUserId));
        if (!success)
        {
            return BadRequest(new { errors });
        }

        return Ok(new { message = "Game request rejected." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRequest(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        var canDeleteAny = await _permissionService.HasPermissionAsync(currentUserId, Permissions.GameRequests.Delete);
        var success = await _mediator.Send(new DeleteGameRequestCommand(id, currentUserId, canDeleteAny));

        if (!success) return NotFound($"Game request with ID {id} not found or access denied.");

        return NoContent();
    }
}
