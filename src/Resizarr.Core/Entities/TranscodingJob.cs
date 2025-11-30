using Resizarr.Core.Enums;

namespace Resizarr.Core.Entities;

public class TranscodingJob : BaseEntity
{
    public int MediaFileId { get; set; }
    public int QualityProfileId { get; set; }

    // Job Configuration
    public string SourcePath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public string TempPath { get; set; } = string.Empty;
    public string FFmpegCommand { get; set; } = string.Empty;

    // Status
    public TranscodingJobStatus Status { get; set; } = TranscodingJobStatus.Queued;
    public int Priority { get; set; } = 0;
    public double Progress { get; set; } = 0; // 0-100

    // Performance Metrics
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? ProcessingTime { get; set; } // in seconds
    public long? OutputFileSize { get; set; }

    // Error Handling
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; } = 0;
    public int MaxRetries { get; set; } = 3;

    // Timestamps
    public DateTime QueuedAt { get; set; }

    // Navigation Properties
    public MediaFile MediaFile { get; set; } = null!;
    public QualityProfile QualityProfile { get; set; } = null!;
    public ICollection<JobHistory> JobHistories { get; set; } = new List<JobHistory>();

    public TranscodingJob()
    {
        QueuedAt = DateTime.UtcNow;
    }
}
