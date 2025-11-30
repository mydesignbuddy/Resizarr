using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resizarr.Application.DTOs;
using Resizarr.Infrastructure.Data;

namespace Resizarr.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class JobsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<JobsController> _logger;

    public JobsController(
        ApplicationDbContext context,
        ILogger<JobsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TranscodingJobDto>>> GetAll(
        [FromQuery] string? status = null)
    {
        var query = _context.TranscodingJobs
            .Include(j => j.MediaFile)
            .Include(j => j.QualityProfile)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<Core.Enums.TranscodingJobStatus>(status, true, out var statusEnum))
            {
                query = query.Where(j => j.Status == statusEnum);
            }
        }

        var jobs = await query
            .OrderByDescending(j => j.Priority)
            .ThenBy(j => j.QueuedAt)
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

    [HttpGet("{id}")]
    public async Task<ActionResult<TranscodingJobDto>> GetById(int id)
    {
        var job = await _context.TranscodingJobs
            .Include(j => j.MediaFile)
            .Include(j => j.QualityProfile)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            return NotFound();
        }

        var dto = new TranscodingJobDto
        {
            Id = job.Id,
            MediaFileId = job.MediaFileId,
            MediaFileName = job.MediaFile.FileName,
            QualityProfileId = job.QualityProfileId,
            QualityProfileName = job.QualityProfile.Name,
            SourcePath = job.SourcePath,
            OutputPath = job.OutputPath,
            Status = job.Status,
            Priority = job.Priority,
            Progress = job.Progress,
            StartedAt = job.StartedAt,
            CompletedAt = job.CompletedAt,
            ProcessingTime = job.ProcessingTime,
            OutputFileSize = job.OutputFileSize,
            ErrorMessage = job.ErrorMessage,
            RetryCount = job.RetryCount,
            QueuedAt = job.QueuedAt
        };

        return Ok(dto);
    }

    [HttpGet("queue")]
    public async Task<ActionResult<object>> GetQueueStatus()
    {
        var queuedCount = await _context.TranscodingJobs
            .CountAsync(j => j.Status == Core.Enums.TranscodingJobStatus.Queued);

        var runningCount = await _context.TranscodingJobs
            .CountAsync(j => j.Status == Core.Enums.TranscodingJobStatus.Running);

        var completedCount = await _context.TranscodingJobs
            .CountAsync(j => j.Status == Core.Enums.TranscodingJobStatus.Completed);

        var failedCount = await _context.TranscodingJobs
            .CountAsync(j => j.Status == Core.Enums.TranscodingJobStatus.Failed);

        return Ok(new
        {
            Queued = queuedCount,
            Running = runningCount,
            Completed = completedCount,
            Failed = failedCount
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var job = await _context.TranscodingJobs.FindAsync(id);

        if (job == null)
        {
            return NotFound();
        }

        _context.TranscodingJobs.Remove(job);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
