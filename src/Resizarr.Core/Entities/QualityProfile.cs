using Resizarr.Core.Enums;

namespace Resizarr.Core.Entities;

public class QualityProfile : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Enabled { get; set; } = true;

    // Video Settings
    public string TargetResolution { get; set; } = string.Empty; // e.g., "1920x1080"
    public string TargetCodec { get; set; } = string.Empty; // e.g., "h264", "hevc"
    public int? TargetBitrate { get; set; } // in kbps
    public int? MinBitrate { get; set; }
    public int? MaxBitrate { get; set; }
    public double? TargetFrameRate { get; set; } // null means keep source
    public string? PixelFormat { get; set; }

    // Audio Settings
    public string AudioCodec { get; set; } = string.Empty; // e.g., "aac", "ac3"
    public int? AudioBitrate { get; set; }
    public int? AudioChannels { get; set; }

    // Hardware Acceleration
    public HardwareAccelerationType HardwareAcceleration { get; set; } = HardwareAccelerationType.Auto;

    // Trigger Conditions
    public int? MinSourceWidth { get; set; }
    public int? MinSourceHeight { get; set; }
    public string? SourceCodecs { get; set; } // Comma-separated list

    // Navigation Properties
    public ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
    public ICollection<TranscodingJob> TranscodingJobs { get; set; } = new List<TranscodingJob>();
    public ICollection<FolderConfiguration> FolderConfigurations { get; set; } = new List<FolderConfiguration>();
}
