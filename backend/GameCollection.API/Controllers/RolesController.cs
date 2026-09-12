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
public class RolesController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public RolesController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    [HasPermission(Permissions.Roles.View)]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _permissionService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("{id:int}")]
    [HasPermission(Permissions.Roles.View)]
    public async Task<IActionResult> GetRoleById(int id)
    {
        var role = await _permissionService.GetRoleByIdAsync(id);
        if (role == null) return NotFound($"Role with ID {id} not found.");

        return Ok(role);
    }

    [HttpGet("permissions")]
    [HasPermission(Permissions.Roles.View)]
    public async Task<IActionResult> GetAllPermissions()
    {
        var grouped = await _permissionService.GetAllPermissionsGroupedAsync();
        return Ok(grouped);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        // Enforce Super Admin only
        if (!await _permissionService.IsSuperAdminAsync(userId))
        {
            return Forbid();
        }

        var (success, role, errors) = await _permissionService.CreateRoleAsync(request, userId);
        if (!success)
        {
            return BadRequest(new { errors });
        }

        return CreatedAtAction(nameof(GetRoleById), new { id = role!.Id }, role);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        // Enforce Super Admin only
        if (!await _permissionService.IsSuperAdminAsync(userId))
        {
            return Forbid();
        }

        var (success, role, errors) = await _permissionService.UpdateRoleAsync(id, request, userId);
        if (!success)
        {
            return BadRequest(new { errors });
        }

        return Ok(role);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        // Enforce Super Admin only
        if (!await _permissionService.IsSuperAdminAsync(userId))
        {
            return Forbid();
        }

        var (success, errors) = await _permissionService.DeleteRoleAsync(id);
        if (!success)
        {
            return BadRequest(new { errors });
        }

        return NoContent();
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> ToggleRoleStatus(int id, [FromBody] ToggleStatusRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        // Enforce Super Admin only
        if (!await _permissionService.IsSuperAdminAsync(userId))
        {
            return Forbid();
        }

        var (success, errors) = await _permissionService.ToggleRoleStatusAsync(id, request.IsActive);
        if (!success)
        {
            return BadRequest(new { errors });
        }

        return Ok(new { message = $"Role status updated to {(request.IsActive ? "Active" : "Inactive")}." });
    }
}

public class ToggleStatusRequest
{
    public bool IsActive { get; set; }
}
