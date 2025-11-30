using Resizarr.Core.Enums;

namespace Resizarr.Application.DTOs;

public class TranscodingJobDto
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public string MediaFileName { get; set; } = string.Empty;
    public int QualityProfileId { get; set; }
    public string QualityProfileName { get; set; } = string.Empty;
    public string SourcePath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public TranscodingJobStatus Status { get; set; }
    public int Priority { get; set; }
    public double Progress { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? ProcessingTime { get; set; }
    public long? OutputFileSize { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTime QueuedAt { get; set; }
}

public class CreateTranscodingJobDto
{
    public int MediaFileId { get; set; }
    public int QualityProfileId { get; set; }
    public int Priority { get; set; } = 0;
}
