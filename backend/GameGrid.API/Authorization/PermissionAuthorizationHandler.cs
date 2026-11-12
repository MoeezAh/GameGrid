using System.Security.Claims;
using System.Threading.Tasks;
using GameGrid.Application.Common.Interfaces;
using GameGrid.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;

namespace GameGrid.API.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationHandler(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        // Super Admin bypasses all individual permission checks
        if (await _permissionService.IsSuperAdminAsync(userId))
        {
            context.Succeed(requirement);
            return;
        }

        // Check user effective permissions
        if (await _permissionService.HasPermissionAsync(userId, requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
