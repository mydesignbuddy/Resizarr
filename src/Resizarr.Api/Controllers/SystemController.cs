using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resizarr.Application.DTOs;
using Resizarr.Infrastructure.Data;

namespace Resizarr.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SystemController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SystemController> _logger;

    public SystemController(
        ApplicationDbContext context,
        ILogger<SystemController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("status")]
    public async Task<ActionResult<SystemStatusDto>> GetStatus()
    {
        var activeJobs = await _context.TranscodingJobs
            .CountAsync(j => j.Status == Core.Enums.TranscodingJobStatus.Running);

        var queuedJobs = await _context.TranscodingJobs
            .CountAsync(j => j.Status == Core.Enums.TranscodingJobStatus.Queued);

        var totalMediaFiles = await _context.MediaFiles.CountAsync();

        var status = new SystemStatusDto
        {
            Version = "1.0.0",
            IsHealthy = true,
            FFmpegVersion = "Unknown",
            FFprobeVersion = "Unknown",
            AvailableHardwareAcceleration = new List<string> { "Software" },
            ActiveJobs = activeJobs,
            QueuedJobs = queuedJobs,
            TotalMediaFiles = totalMediaFiles,
            StartTime = DateTime.UtcNow
        };

        return Ok(status);
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new { status = "healthy" });
    }

    [HttpGet("capabilities")]
    public IActionResult GetCapabilities()
    {
        return Ok(new
        {
            hardwareAcceleration = new[] { "Software", "NVENC", "QSV" },
            supportedFormats = new[] { "mp4", "mkv", "avi", "mov", "webm" },
            supportedCodecs = new[] { "h264", "hevc", "vp9", "av1" }
        });
    }
}
