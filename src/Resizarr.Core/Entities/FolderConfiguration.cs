namespace Resizarr.Core.Entities;

public class FolderConfiguration : BaseEntity
{
    public string Path { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public bool Recursive { get; set; } = true;
    public int? QualityProfileId { get; set; }

    // Filters
    public string? FileExtensions { get; set; } // Comma-separated: "mp4,mkv,avi,mov"
    public string? ExcludePatterns { get; set; } // Regex patterns to exclude

    // Processing Options
    public bool DeleteSourceAfterSuccess { get; set; } = false;

    // Navigation Properties
    public QualityProfile? QualityProfile { get; set; }
}
