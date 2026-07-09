using System;
using System.Collections.Generic;
using GameCollection.Domain.Common;

namespace GameCollection.Domain.Entities;

public class Publisher : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Website { get; set; }
    public string? Country { get; set; }
    public DateTimeOffset? FoundedDate { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }

    public ICollection<Game> Games { get; set; } = new List<Game>();
}
