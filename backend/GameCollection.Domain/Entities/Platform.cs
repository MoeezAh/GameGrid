using System;
using System.Collections.Generic;
using GameCollection.Domain.Common;

namespace GameCollection.Domain.Entities;

public class Platform : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Manufacturer { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
    public int? Generation { get; set; }
    public string? Notes { get; set; }

    public ICollection<Game> Games { get; set; } = new List<Game>();
}
