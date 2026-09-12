using System.Collections.Generic;
using System.Threading.Tasks;
using GameCollection.Application.DTOs.RBAC;

namespace GameCollection.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<HashSet<string>> GetUserPermissionsAsync(string userId);
    Task<List<string>> GetUserRoleNamesAsync(string userId);
    Task<bool> IsSuperAdminAsync(string userId);
    Task<bool> HasPermissionAsync(string userId, string permission);

    // Role Management
    Task<List<RoleDto>> GetAllRolesAsync();
    Task<RoleDto?> GetRoleByIdAsync(int id);
    Task<(bool Success, RoleDto? Role, string[] Errors)> CreateRoleAsync(CreateRoleDto request, string currentUserId);
    Task<(bool Success, RoleDto? Role, string[] Errors)> UpdateRoleAsync(int id, UpdateRoleDto request, string currentUserId);
    Task<(bool Success, string[] Errors)> DeleteRoleAsync(int id);
    Task<(bool Success, string[] Errors)> ToggleRoleStatusAsync(int id, bool isActive);

    // Permissions List
    Task<List<PermissionGroupDto>> GetAllPermissionsGroupedAsync();

    // User Role Management
    Task<List<UserRoleAssignmentDto>> GetAllUsersWithRolesAsync();
    Task<(bool Success, string[] Errors)> AssignRolesToUserAsync(string targetUserId, List<int> roleIds);
}
