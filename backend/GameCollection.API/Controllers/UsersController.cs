using System.Security.Claims;
using System.Threading.Tasks;
using GameCollection.Application.Common.Interfaces;
using GameCollection.Application.Common.Security;
using GameCollection.Application.DTOs.RBAC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCollection.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public UsersController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    [HasPermission(Permissions.Users.View)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _permissionService.GetAllUsersWithRolesAsync();
        return Ok(users);
    }

    [HttpPut("{id}/roles")]
    [HasPermission(Permissions.Users.AssignRoles)]
    public async Task<IActionResult> AssignRoles(string id, [FromBody] AssignUserRolesRequest request)
    {
        var (success, errors) = await _permissionService.AssignRolesToUserAsync(id, request.RoleIds);
        if (!success)
        {
            return BadRequest(new { errors });
        }

        return Ok(new { message = "User roles updated successfully." });
    }
}
