using Resizarr.Core.Enums;

namespace Resizarr.Core.Entities;

public class MediaFile : BaseEntity
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FolderPath { get; set; } = string.Empty;

    // File Information
    public long FileSize { get; set; }
    public string? FileHash { get; set; }

    // Video Metadata
    public string? VideoCodec { get; set; }
    public int? VideoWidth { get; set; }
    public int? VideoHeight { get; set; }
    public int? VideoBitrate { get; set; }
    public double? VideoFrameRate { get; set; }
    public double? Duration { get; set; } // in seconds

    // Audio Metadata
    public string? AudioCodec { get; set; }
    public int? AudioBitrate { get; set; }
    public int? AudioChannels { get; set; }
    public int? AudioSampleRate { get; set; }

    // Processing Status
    public MediaFileStatus Status { get; set; } = MediaFileStatus.Pending;
    public int? QualityProfileId { get; set; }

    // Timestamps
    public DateTime DiscoveredAt { get; set; }
    public DateTime? LastScannedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }

    // Navigation Properties
    public QualityProfile? QualityProfile { get; set; }
    public ICollection<TranscodingJob> TranscodingJobs { get; set; } = new List<TranscodingJob>();

    public MediaFile()
    {
        DiscoveredAt = DateTime.UtcNow;
    }
}
