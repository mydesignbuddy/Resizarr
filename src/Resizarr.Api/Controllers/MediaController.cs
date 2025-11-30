using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resizarr.Application.DTOs;
using Resizarr.Infrastructure.Data;

namespace Resizarr.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MediaController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MediaController> _logger;

    public MediaController(
        ApplicationDbContext context,
        ILogger<MediaController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MediaFileDto>>> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] int? qualityProfileId = null)
    {
        var query = _context.MediaFiles
            .Include(m => m.QualityProfile)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<Core.Enums.MediaFileStatus>(status, true, out var statusEnum))
            {
                query = query.Where(m => m.Status == statusEnum);
            }
        }

        if (qualityProfileId.HasValue)
        {
            query = query.Where(m => m.QualityProfileId == qualityProfileId.Value);
        }

        var mediaFiles = await query
            .OrderByDescending(m => m.DiscoveredAt)
            .ToListAsync();

        var dtos = mediaFiles.Select(m => new MediaFileDto
        {
            Id = m.Id,
            FilePath = m.FilePath,
            FileName = m.FileName,
            FolderPath = m.FolderPath,
            FileSize = m.FileSize,
            VideoCodec = m.VideoCodec,
            VideoWidth = m.VideoWidth,
            VideoHeight = m.VideoHeight,
            VideoBitrate = m.VideoBitrate,
            VideoFrameRate = m.VideoFrameRate,
            Duration = m.Duration,
            AudioCodec = m.AudioCodec,
            AudioBitrate = m.AudioBitrate,
            AudioChannels = m.AudioChannels,
            Status = m.Status,
            QualityProfileId = m.QualityProfileId,
            QualityProfileName = m.QualityProfile?.Name,
            DiscoveredAt = m.DiscoveredAt,
            LastScannedAt = m.LastScannedAt,
            ProcessedAt = m.ProcessedAt
        });

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MediaFileDto>> GetById(int id)
    {
        var mediaFile = await _context.MediaFiles
            .Include(m => m.QualityProfile)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mediaFile == null)
        {
            return NotFound();
        }

        var dto = new MediaFileDto
        {
            Id = mediaFile.Id,
            FilePath = mediaFile.FilePath,
            FileName = mediaFile.FileName,
            FolderPath = mediaFile.FolderPath,
            FileSize = mediaFile.FileSize,
            VideoCodec = mediaFile.VideoCodec,
            VideoWidth = mediaFile.VideoWidth,
            VideoHeight = mediaFile.VideoHeight,
            VideoBitrate = mediaFile.VideoBitrate,
            VideoFrameRate = mediaFile.VideoFrameRate,
            Duration = mediaFile.Duration,
            AudioCodec = mediaFile.AudioCodec,
            AudioBitrate = mediaFile.AudioBitrate,
            AudioChannels = mediaFile.AudioChannels,
            Status = mediaFile.Status,
            QualityProfileId = mediaFile.QualityProfileId,
            QualityProfileName = mediaFile.QualityProfile?.Name,
            DiscoveredAt = mediaFile.DiscoveredAt,
            LastScannedAt = mediaFile.LastScannedAt,
            ProcessedAt = mediaFile.ProcessedAt
        };

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var mediaFile = await _context.MediaFiles.FindAsync(id);

        if (mediaFile == null)
        {
            return NotFound();
        }

        _context.MediaFiles.Remove(mediaFile);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("scan")]
    public async Task<IActionResult> TriggerScan()
    {
        // This will be implemented when we create the folder monitoring service
        _logger.LogInformation("Manual scan triggered");
        return Accepted();
    }
}
