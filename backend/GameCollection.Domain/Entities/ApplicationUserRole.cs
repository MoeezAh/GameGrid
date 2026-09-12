using GameCollection.Domain.Common;

namespace GameCollection.Domain.Entities;

public class ApplicationUserRole : BaseEntity
{
    public string UserId { get; set; } = null!;
    public int RoleId { get; set; }
    public ApplicationRole Role { get; set; } = null!;
}
