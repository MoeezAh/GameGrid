using System;

namespace GameCollection.Application.DTOs.Metadata;

// Developer DTOs
public class DeveloperDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Website { get; set; }
    public string? Country { get; set; }
    public DateTimeOffset? FoundedDate { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }
}

public class CreateDeveloperDto
{
    public string Name { get; set; } = null!;
    public string? Website { get; set; }
    public string? Country { get; set; }
    public DateTimeOffset? FoundedDate { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }
}

public class UpdateDeveloperDto : CreateDeveloperDto
{
    public int Id { get; set; }
}

// Publisher DTOs
public class PublisherDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Website { get; set; }
    public string? Country { get; set; }
    public DateTimeOffset? FoundedDate { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }
}

public class CreatePublisherDto
{
    public string Name { get; set; } = null!;
    public string? Website { get; set; }
    public string? Country { get; set; }
    public DateTimeOffset? FoundedDate { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }
}

public class UpdatePublisherDto : CreatePublisherDto
{
    public int Id { get; set; }
}

// Platform DTOs
public class PlatformDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Manufacturer { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
    public int? Generation { get; set; }
    public string? Notes { get; set; }
}

public class CreatePlatformDto
{
    public string Name { get; set; } = null!;
    public string? Manufacturer { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
    public int? Generation { get; set; }
    public string? Notes { get; set; }
}

public class UpdatePlatformDto : CreatePlatformDto
{
    public int Id { get; set; }
}

// DigitalService DTOs
public class DigitalServiceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Website { get; set; }
    public string? Notes { get; set; }
}

public class CreateDigitalServiceDto
{
    public string Name { get; set; } = null!;
    public string? Website { get; set; }
    public string? Notes { get; set; }
}

public class UpdateDigitalServiceDto : CreateDigitalServiceDto
{
    public int Id { get; set; }
}

// Genre DTOs
public class GenreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class CreateGenreDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateGenreDto : CreateGenreDto
{
    public int Id { get; set; }
}

// Tag DTOs
public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class CreateTagDto
{
    public string Name { get; set; } = null!;
}

public class UpdateTagDto : CreateTagDto
{
    public int Id { get; set; }
}

// Theme DTOs
public class ThemeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class CreateThemeDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateThemeDto : CreateThemeDto
{
    public int Id { get; set; }
}

// Franchise DTOs
public class FranchiseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class CreateFranchiseDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateFranchiseDto : CreateFranchiseDto
{
    public int Id { get; set; }
}

// Series DTOs
public class SeriesDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class CreateSeriesDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateSeriesDto : CreateSeriesDto
{
    public int Id { get; set; }
}
