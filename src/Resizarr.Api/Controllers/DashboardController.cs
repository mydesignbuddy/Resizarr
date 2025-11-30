using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resizarr.Application.DTOs;
using Resizarr.Infrastructure.Data;

namespace Resizarr.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        ApplicationDbContext context,
        ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        var totalMediaFiles = await _context.MediaFiles.CountAsync();
        var pendingFiles = await _context.MediaFiles
            .CountAsync(m => m.Status == Core.Enums.MediaFileStatus.Pending);
        var processingFiles = await _context.MediaFiles
            .CountAsync(m => m.Status == Core.Enums.MediaFileStatus.Processing);
        var completedFiles = await _context.MediaFiles
            .CountAsync(m => m.Status == Core.Enums.MediaFileStatus.Completed);
        var failedFiles = await _context.MediaFiles
            .CountAsync(m => m.Status == Core.Enums.MediaFileStatus.Failed);

        var activeJobs = await _context.TranscodingJobs
            .CountAsync(j => j.Status == Core.Enums.TranscodingJobStatus.Running);
        var queuedJobs = await _context.TranscodingJobs
            .CountAsync(j => j.Status == Core.Enums.TranscodingJobStatus.Queued);

        var completedJobs = await _context.TranscodingJobs
            .Where(j => j.Status == Core.Enums.TranscodingJobStatus.Completed)
            .ToListAsync();

        long totalBytesProcessed = completedJobs.Sum(j => j.OutputFileSize ?? 0);

        var stats = new DashboardStatsDto
        {
            TotalMediaFiles = totalMediaFiles,
            PendingFiles = pendingFiles,
            ProcessingFiles = processingFiles,
            CompletedFiles = completedFiles,
            FailedFiles = failedFiles,
            ActiveJobs = activeJobs,
            QueuedJobs = queuedJobs,
            TotalBytesProcessed = totalBytesProcessed,
            TotalBytesSaved = 0
        };

        return Ok(stats);
    }

    [HttpGet("queue")]
    public async Task<ActionResult<IEnumerable<TranscodingJobDto>>> GetQueue()
    {
        var jobs = await _context.TranscodingJobs
            .Include(j => j.MediaFile)
            .Include(j => j.QualityProfile)
            .Where(j => j.Status == Core.Enums.TranscodingJobStatus.Queued ||
                       j.Status == Core.Enums.TranscodingJobStatus.Running)
            .OrderByDescending(j => j.Priority)
            .ThenBy(j => j.QueuedAt)
            .Take(20)
            .ToListAsync();

        var dtos = jobs.Select(j => new TranscodingJobDto
        {
            Id = j.Id,
            MediaFileId = j.MediaFileId,
            MediaFileName = j.MediaFile.FileName,
            QualityProfileId = j.QualityProfileId,
            QualityProfileName = j.QualityProfile.Name,
            SourcePath = j.SourcePath,
            OutputPath = j.OutputPath,
            Status = j.Status,
            Priority = j.Priority,
            Progress = j.Progress,
            StartedAt = j.StartedAt,
            CompletedAt = j.CompletedAt,
            ProcessingTime = j.ProcessingTime,
            OutputFileSize = j.OutputFileSize,
            ErrorMessage = j.ErrorMessage,
            RetryCount = j.RetryCount,
            QueuedAt = j.QueuedAt
        });

        return Ok(dtos);
    }
}
