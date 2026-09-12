using System;
using System.Security.Claims;
using System.Threading.Tasks;
using GameCollection.Application.Common.Interfaces;
using GameCollection.Application.Common.Security;
using GameCollection.Application.Features.Games.Commands.CreateGame;
using GameCollection.Application.Features.Games.Commands.DeleteGame;
using GameCollection.Application.Features.Games.Commands.UpdateGame;
using GameCollection.Application.Features.Games.Queries.GetGameById;
using GameCollection.Application.Features.Games.Queries.GetGames;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameCollection.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IFileStorageService _fileStorageService;

    public GamesController(ISender mediator, IFileStorageService fileStorageService)
    {
        _mediator = mediator;
        _fileStorageService = fileStorageService;
    }

    [HttpGet]
    [HasPermission(Permissions.Games.View)]
    public async Task<IActionResult> GetGames([FromQuery] GetGamesQuery query)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var queryWithUser = query with { UserId = userId };
        
        var result = await _mediator.Send(queryWithUser);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [HasPermission(Permissions.Games.View)]
    public async Task<IActionResult> GetGame(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var game = await _mediator.Send(new GetGameByIdQuery(id, userId));
        if (game == null) return NotFound($"Game with ID {id} not found.");

        return Ok(game);
    }

    [HttpPost]
    [HasPermission(Permissions.Games.Create)]
    public async Task<IActionResult> Create([FromBody] CreateGameCommand command)
    {
        var gameId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetGame), new { id = gameId }, new { id = gameId });
    }

    [HttpPut("{id:int}")]
    [HasPermission(Permissions.Games.Update)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGameCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in URL path does not match ID in request body.");
        }

        var succeeded = await _mediator.Send(command);
        if (!succeeded) return NotFound($"Game with ID {id} not found.");

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [HasPermission(Permissions.Games.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var succeeded = await _mediator.Send(new DeleteGameCommand(id));
        if (!succeeded) return NotFound($"Game with ID {id} not found.");

        return NoContent();
    }

    [HttpPost("upload")]
    [HasPermission(Permissions.Games.ManageMetadata)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadMedia([FromForm] UploadMediaRequest request)
    {
        var file = request.File;
        var folderName = request.FolderName;

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }

        var validFolders = new[] { "covers", "boxarts", "banners", "logos", "screenshots", "artworks", "fanarts" };
        if (string.IsNullOrWhiteSpace(folderName) || !Array.Exists(validFolders, f => f == folderName.ToLower()))
        {
            folderName = "general";
        }

        using var stream = file.OpenReadStream();
        var relativeUrl = await _fileStorageService.SaveFileAsync(stream, file.FileName, folderName);

        return Ok(new { url = relativeUrl });
    }
}

public class UploadMediaRequest
{
    public IFormFile File { get; set; } = null!;
    public string FolderName { get; set; } = null!;
}
