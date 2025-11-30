namespace Resizarr.Application.DTOs;

public class SystemStatusDto
{
    public string Version { get; set; } = "1.0.0";
    public bool IsHealthy { get; set; }
    public string FFmpegVersion { get; set; } = string.Empty;
    public string FFprobeVersion { get; set; } = string.Empty;
    public List<string> AvailableHardwareAcceleration { get; set; } = new();
    public int ActiveJobs { get; set; }
    public int QueuedJobs { get; set; }
    public int TotalMediaFiles { get; set; }
    public DateTime StartTime { get; set; }
}

public class DashboardStatsDto
{
    public int TotalMediaFiles { get; set; }
    public int PendingFiles { get; set; }
    public int ProcessingFiles { get; set; }
    public int CompletedFiles { get; set; }
    public int FailedFiles { get; set; }
    public int ActiveJobs { get; set; }
    public int QueuedJobs { get; set; }
    public long TotalBytesProcessed { get; set; }
    public long TotalBytesSaved { get; set; }
}
