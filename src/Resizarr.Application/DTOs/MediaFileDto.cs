using Resizarr.Core.Enums;

namespace Resizarr.Application.DTOs;

public class MediaFileDto
{
    public int Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FolderPath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? VideoCodec { get; set; }
    public int? VideoWidth { get; set; }
    public int? VideoHeight { get; set; }
    public int? VideoBitrate { get; set; }
    public double? VideoFrameRate { get; set; }
    public double? Duration { get; set; }
    public string? AudioCodec { get; set; }
    public int? AudioBitrate { get; set; }
    public int? AudioChannels { get; set; }
    public MediaFileStatus Status { get; set; }
    public int? QualityProfileId { get; set; }
    public string? QualityProfileName { get; set; }
    public DateTime DiscoveredAt { get; set; }
    public DateTime? LastScannedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
