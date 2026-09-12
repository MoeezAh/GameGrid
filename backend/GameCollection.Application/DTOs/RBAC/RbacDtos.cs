using System;
using System.Collections.Generic;

namespace GameCollection.Application.DTOs.RBAC;

public class PermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public string Category { get; set; } = null!;
}

public class PermissionGroupDto
{
    public string Category { get; set; } = null!;
    public List<PermissionDto> Permissions { get; set; } = new();
}

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public bool IsSuperAdmin { get; set; }
    public bool IsSystemRole { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public int UserCount { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
}

public class CreateRoleDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public List<int> PermissionIds { get; set; } = new();
}

public class UpdateRoleDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<int> PermissionIds { get; set; } = new();
}

public class UserRoleAssignmentDto
{
    public string UserId { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsSuperAdmin { get; set; }
    public List<int> RoleIds { get; set; } = new();
    public List<string> RoleNames { get; set; } = new();
}

public class AssignUserRolesRequest
{
    public List<int> RoleIds { get; set; } = new();
}

public class UserDetailDto
{
    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsSuperAdmin { get; set; }
    public List<RoleDto> Roles { get; set; } = new();
    public List<string> EffectivePermissions { get; set; } = new();
}
