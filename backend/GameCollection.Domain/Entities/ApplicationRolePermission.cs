using GameCollection.Domain.Common;

namespace GameCollection.Domain.Entities;

public class ApplicationRolePermission : BaseEntity
{
    public int RoleId { get; set; }
    public ApplicationRole Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
