using GameCollection.Domain.Common;

namespace GameCollection.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public string Category { get; set; } = null!; // e.g. "Games", "Libraries", "GameRequests", "Users", "Roles", "Metadata"

    public ICollection<ApplicationRolePermission> RolePermissions { get; set; } = new List<ApplicationRolePermission>();
}
