using System.Collections.Generic;
using GameCollection.Domain.Common;

namespace GameCollection.Domain.Entities;

public class Tag : BaseAuditableEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Game> Games { get; set; } = new List<Game>();
}
