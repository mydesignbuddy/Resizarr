using Resizarr.Core.Enums;

namespace Resizarr.Application.DTOs;

public class QualityProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public string TargetResolution { get; set; } = string.Empty;
    public string TargetCodec { get; set; } = string.Empty;
    public int? TargetBitrate { get; set; }
    public int? MinBitrate { get; set; }
    public int? MaxBitrate { get; set; }
    public double? TargetFrameRate { get; set; }
    public string AudioCodec { get; set; } = string.Empty;
    public int? AudioBitrate { get; set; }
    public int? AudioChannels { get; set; }
    public HardwareAccelerationType HardwareAcceleration { get; set; }
    public int? MinSourceWidth { get; set; }
    public int? MinSourceHeight { get; set; }
    public string? SourceCodecs { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateQualityProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Enabled { get; set; } = true;
    public string TargetResolution { get; set; } = string.Empty;
    public string TargetCodec { get; set; } = string.Empty;
    public int? TargetBitrate { get; set; }
    public int? MinBitrate { get; set; }
    public int? MaxBitrate { get; set; }
    public double? TargetFrameRate { get; set; }
    public string AudioCodec { get; set; } = string.Empty;
    public int? AudioBitrate { get; set; }
    public int? AudioChannels { get; set; }
    public HardwareAccelerationType HardwareAcceleration { get; set; } = HardwareAccelerationType.Auto;
    public int? MinSourceWidth { get; set; }
    public int? MinSourceHeight { get; set; }
    public string? SourceCodecs { get; set; }
}
