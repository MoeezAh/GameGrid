using System.Collections.Generic;
using GameGrid.Domain.Common;

namespace GameGrid.Domain.Entities;

public class Tag : BaseAuditableEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Game> Games { get; set; } = new List<Game>();
}
