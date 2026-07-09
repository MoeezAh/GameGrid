using System.Collections.Generic;
using GameCollection.Domain.Common;

namespace GameCollection.Domain.Entities;

public class Series : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Game> Games { get; set; } = new List<Game>();
}
