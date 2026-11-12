using System.Collections.Generic;
using GameGrid.Domain.Common;

namespace GameGrid.Domain.Entities;

public class DigitalService : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Website { get; set; }
    public string? Notes { get; set; }

    public ICollection<Game> Games { get; set; } = new List<Game>();
}
