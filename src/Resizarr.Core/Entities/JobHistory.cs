using Resizarr.Core.Enums;

namespace Resizarr.Core.Entities;

public class JobHistory
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public int TranscodingJobId { get; set; }

    public JobHistoryAction Action { get; set; }
    public string? Message { get; set; }
    public string? Metadata { get; set; } // JSON for additional data

    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public MediaFile MediaFile { get; set; } = null!;
    public TranscodingJob TranscodingJob { get; set; } = null!;

    public JobHistory()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
