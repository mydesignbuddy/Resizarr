namespace Resizarr.Application.DTOs;

public class FolderConfigurationDto
{
    public int Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public bool Recursive { get; set; }
    public int? QualityProfileId { get; set; }
    public string? QualityProfileName { get; set; }
    public string? FileExtensions { get; set; }
    public string? ExcludePatterns { get; set; }
    public bool DeleteSourceAfterSuccess { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateFolderConfigurationDto
{
    public string Path { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public bool Recursive { get; set; } = true;
    public int? QualityProfileId { get; set; }
    public string? FileExtensions { get; set; }
    public string? ExcludePatterns { get; set; }
    public bool DeleteSourceAfterSuccess { get; set; } = false;
}
