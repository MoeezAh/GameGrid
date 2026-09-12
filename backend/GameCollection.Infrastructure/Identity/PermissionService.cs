using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCollection.Application.Common.Interfaces;
using GameCollection.Application.DTOs.RBAC;
using GameCollection.Domain.Entities;
using GameCollection.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Infrastructure.Identity;

public class PermissionService : IPermissionService
{
    private readonly GameDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public PermissionService(GameDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> IsSuperAdminAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return await _context.ApplicationUserRoles
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.UserId == userId && ur.Role.IsActive && !ur.Role.IsDeleted && ur.Role.IsSuperAdmin);
    }

    public async Task<HashSet<string>> GetUserPermissionsAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return new HashSet<string>();

        // Super Admin gets all permissions dynamically
        if (await IsSuperAdminAsync(userId))
        {
            var allPermissions = await _context.Permissions.Select(p => p.Name).ToListAsync();
            return new HashSet<string>(allPermissions, StringComparer.OrdinalIgnoreCase);
        }

        // Union of permissions from all active assigned roles
        var permissions = await _context.ApplicationUserRoles
            .Where(ur => ur.UserId == userId && ur.Role.IsActive && !ur.Role.IsDeleted)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync();

        return new HashSet<string>(permissions, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<List<string>> GetUserRoleNamesAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return new List<string>();

        return await _context.ApplicationUserRoles
            .Where(ur => ur.UserId == userId && ur.Role.IsActive && !ur.Role.IsDeleted)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission)
    {
        if (string.IsNullOrEmpty(userId)) return false;
        if (await IsSuperAdminAsync(userId)) return true;

        var permissions = await GetUserPermissionsAsync(userId);
        return permissions.Contains(permission);
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _context.ApplicationRoles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Include(r => r.UserRoles)
            .Where(r => !r.IsDeleted)
            .OrderByDescending(r => r.IsSuperAdmin)
            .ThenBy(r => r.Name)
            .ToListAsync();

        return roles.Select(MapToRoleDto).ToList();
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        var role = await _context.ApplicationRoles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        return role == null ? null : MapToRoleDto(role);
    }

    public async Task<(bool Success, RoleDto? Role, string[] Errors)> CreateRoleAsync(CreateRoleDto request, string currentUserId)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return (false, null, new[] { "Role name is required." });
        }

        var normalizedName = request.Name.Trim();
        var exists = await _context.ApplicationRoles
            .AnyAsync(r => r.Name.ToLower() == normalizedName.ToLower() && !r.IsDeleted);

        if (exists)
        {
            return (false, null, new[] { $"A role with name '{normalizedName}' already exists." });
        }

        var role = new ApplicationRole
        {
            Name = normalizedName,
            Description = request.Description?.Trim(),
            IsActive = request.IsActive,
            IsSuperAdmin = false,
            IsSystemRole = false,
            CreatedBy = currentUserId,
            CreatedDate = DateTimeOffset.UtcNow
        };

        if (request.PermissionIds != null && request.PermissionIds.Any())
        {
            var validPermissionIds = await _context.Permissions
                .Where(p => request.PermissionIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            foreach (var permId in validPermissionIds)
            {
                role.RolePermissions.Add(new ApplicationRolePermission
                {
                    PermissionId = permId
                });
            }
        }

        await _context.ApplicationRoles.AddAsync(role);
        await _context.SaveChangesAsync();

        var created = await GetRoleByIdAsync(role.Id);
        return (true, created, Array.Empty<string>());
    }

    public async Task<(bool Success, RoleDto? Role, string[] Errors)> UpdateRoleAsync(int id, UpdateRoleDto request, string currentUserId)
    {
        var role = await _context.ApplicationRoles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (role == null)
        {
            return (false, null, new[] { "Role not found." });
        }

        var normalizedName = request.Name.Trim();
        var nameConflict = await _context.ApplicationRoles
            .AnyAsync(r => r.Id != id && r.Name.ToLower() == normalizedName.ToLower() && !r.IsDeleted);

        if (nameConflict)
        {
            return (false, null, new[] { $"A role with name '{normalizedName}' already exists." });
        }

        // Prevent modifying system Super Admin role name/status
        if (role.IsSuperAdmin && !request.IsActive)
        {
            return (false, null, new[] { "The Super Admin role cannot be deactivated." });
        }

        role.Name = normalizedName;
        role.Description = request.Description?.Trim();
        role.IsActive = request.IsActive;
        role.UpdatedBy = currentUserId;
        role.UpdatedDate = DateTimeOffset.UtcNow;

        // Update permissions (for non-SuperAdmin roles)
        if (!role.IsSuperAdmin)
        {
            _context.ApplicationRolePermissions.RemoveRange(role.RolePermissions);

            if (request.PermissionIds != null && request.PermissionIds.Any())
            {
                var validPermissionIds = await _context.Permissions
                    .Where(p => request.PermissionIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync();

                foreach (var permId in validPermissionIds)
                {
                    _context.ApplicationRolePermissions.Add(new ApplicationRolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permId
                    });
                }
            }
        }

        await _context.SaveChangesAsync();

        var updated = await GetRoleByIdAsync(role.Id);
        return (true, updated, Array.Empty<string>());
    }

    public async Task<(bool Success, string[] Errors)> DeleteRoleAsync(int id)
    {
        var role = await _context.ApplicationRoles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (role == null)
        {
            return (false, new[] { "Role not found." });
        }

        if (role.IsSuperAdmin || role.IsSystemRole)
        {
            return (false, new[] { "System roles cannot be deleted." });
        }

        if (role.UserRoles.Any())
        {
            return (false, new[] { $"Cannot delete role '{role.Name}' because it is currently assigned to {role.UserRoles.Count} user(s). Remove the role from all users first." });
        }

        role.IsDeleted = true;
        role.UpdatedDate = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

        return (true, Array.Empty<string>());
    }

    public async Task<(bool Success, string[] Errors)> ToggleRoleStatusAsync(int id, bool isActive)
    {
        var role = await _context.ApplicationRoles.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (role == null)
        {
            return (false, new[] { "Role not found." });
        }

        if (role.IsSuperAdmin && !isActive)
        {
            return (false, new[] { "The Super Admin role cannot be deactivated." });
        }

        role.IsActive = isActive;
        role.UpdatedDate = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

        return (true, Array.Empty<string>());
    }

    public async Task<List<PermissionGroupDto>> GetAllPermissionsGroupedAsync()
    {
        var permissions = await _context.Permissions
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .ToListAsync();

        return permissions
            .GroupBy(p => p.Category)
            .Select(g => new PermissionGroupDto
            {
                Category = g.Key,
                Permissions = g.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    DisplayName = p.DisplayName,
                    Description = p.Description,
                    Category = p.Category
                }).ToList()
            })
            .ToList();
    }

    public async Task<List<UserRoleAssignmentDto>> GetAllUsersWithRolesAsync()
    {
        var users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
        var userRoles = await _context.ApplicationUserRoles
            .Include(ur => ur.Role)
            .Where(ur => !ur.Role.IsDeleted)
            .ToListAsync();

        var result = new List<UserRoleAssignmentDto>();

        foreach (var user in users)
        {
            var assigned = userRoles.Where(ur => ur.UserId == user.Id).ToList();
            bool isSuperAdmin = string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase) ||
                               assigned.Any(ur => ur.Role.IsSuperAdmin);

            result.Add(new UserRoleAssignmentDto
            {
                UserId = user.Id,
                Username = user.UserName ?? "Unknown",
                Email = user.Email ?? "Unknown",
                IsSuperAdmin = isSuperAdmin,
                RoleIds = assigned.Select(ur => ur.RoleId).ToList(),
                RoleNames = assigned.Select(ur => ur.Role.Name).ToList()
            });
        }

        return result;
    }

    public async Task<(bool Success, string[] Errors)> AssignRolesToUserAsync(string targetUserId, List<int> roleIds)
    {
        var user = await _userManager.FindByIdAsync(targetUserId);
        if (user == null)
        {
            return (false, new[] { "User not found." });
        }

        if (roleIds == null || !roleIds.Any())
        {
            return (false, new[] { "Every user must have at least one role assigned." });
        }

        var validRoles = await _context.ApplicationRoles
            .Where(r => roleIds.Contains(r.Id) && r.IsActive && !r.IsDeleted)
            .ToListAsync();

        if (validRoles.Count != roleIds.Distinct().Count())
        {
            return (false, new[] { "One or more selected roles are invalid or inactive." });
        }

        // Remove existing role assignments for this user
        var existingAssignments = await _context.ApplicationUserRoles
            .Where(ur => ur.UserId == targetUserId)
            .ToListAsync();

        _context.ApplicationUserRoles.RemoveRange(existingAssignments);

        // Add new assignments
        foreach (var role in validRoles)
        {
            _context.ApplicationUserRoles.Add(new ApplicationUserRole
            {
                UserId = targetUserId,
                RoleId = role.Id
            });
        }

        await _context.SaveChangesAsync();

        return (true, Array.Empty<string>());
    }

    private static RoleDto MapToRoleDto(ApplicationRole role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            IsSuperAdmin = role.IsSuperAdmin,
            IsSystemRole = role.IsSystemRole,
            CreatedDate = role.CreatedDate,
            UpdatedDate = role.UpdatedDate,
            UserCount = role.UserRoles.Count,
            Permissions = role.RolePermissions.Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name,
                DisplayName = rp.Permission.DisplayName,
                Description = rp.Permission.Description,
                Category = rp.Permission.Category
            }).ToList()
        };
    }
}
