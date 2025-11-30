using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resizarr.Application.DTOs;
using Resizarr.Core.Entities;
using Resizarr.Infrastructure.Data;

namespace Resizarr.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class QualityProfilesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QualityProfilesController> _logger;

    public QualityProfilesController(
        ApplicationDbContext context,
        ILogger<QualityProfilesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QualityProfileDto>>> GetAll()
    {
        var profiles = await _context.QualityProfiles
            .OrderBy(p => p.Name)
            .ToListAsync();

        var dtos = profiles.Select(p => new QualityProfileDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Enabled = p.Enabled,
            TargetResolution = p.TargetResolution,
            TargetCodec = p.TargetCodec,
            TargetBitrate = p.TargetBitrate,
            MinBitrate = p.MinBitrate,
            MaxBitrate = p.MaxBitrate,
            TargetFrameRate = p.TargetFrameRate,
            AudioCodec = p.AudioCodec,
            AudioBitrate = p.AudioBitrate,
            AudioChannels = p.AudioChannels,
            HardwareAcceleration = p.HardwareAcceleration,
            MinSourceWidth = p.MinSourceWidth,
            MinSourceHeight = p.MinSourceHeight,
            SourceCodecs = p.SourceCodecs,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QualityProfileDto>> GetById(int id)
    {
        var profile = await _context.QualityProfiles.FindAsync(id);

        if (profile == null)
        {
            return NotFound();
        }

        var dto = new QualityProfileDto
        {
            Id = profile.Id,
            Name = profile.Name,
            Description = profile.Description,
            Enabled = profile.Enabled,
            TargetResolution = profile.TargetResolution,
            TargetCodec = profile.TargetCodec,
            TargetBitrate = profile.TargetBitrate,
            MinBitrate = profile.MinBitrate,
            MaxBitrate = profile.MaxBitrate,
            TargetFrameRate = profile.TargetFrameRate,
            AudioCodec = profile.AudioCodec,
            AudioBitrate = profile.AudioBitrate,
            AudioChannels = profile.AudioChannels,
            HardwareAcceleration = profile.HardwareAcceleration,
            MinSourceWidth = profile.MinSourceWidth,
            MinSourceHeight = profile.MinSourceHeight,
            SourceCodecs = profile.SourceCodecs,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<QualityProfileDto>> Create(CreateQualityProfileDto dto)
    {
        var profile = new QualityProfile
        {
            Name = dto.Name,
            Description = dto.Description,
            Enabled = dto.Enabled,
            TargetResolution = dto.TargetResolution,
            TargetCodec = dto.TargetCodec,
            TargetBitrate = dto.TargetBitrate,
            MinBitrate = dto.MinBitrate,
            MaxBitrate = dto.MaxBitrate,
            TargetFrameRate = dto.TargetFrameRate,
            AudioCodec = dto.AudioCodec,
            AudioBitrate = dto.AudioBitrate,
            AudioChannels = dto.AudioChannels,
            HardwareAcceleration = dto.HardwareAcceleration,
            MinSourceWidth = dto.MinSourceWidth,
            MinSourceHeight = dto.MinSourceHeight,
            SourceCodecs = dto.SourceCodecs
        };

        _context.QualityProfiles.Add(profile);
        await _context.SaveChangesAsync();

        var resultDto = new QualityProfileDto
        {
            Id = profile.Id,
            Name = profile.Name,
            Description = profile.Description,
            Enabled = profile.Enabled,
            TargetResolution = profile.TargetResolution,
            TargetCodec = profile.TargetCodec,
            TargetBitrate = profile.TargetBitrate,
            MinBitrate = profile.MinBitrate,
            MaxBitrate = profile.MaxBitrate,
            TargetFrameRate = profile.TargetFrameRate,
            AudioCodec = profile.AudioCodec,
            AudioBitrate = profile.AudioBitrate,
            AudioChannels = profile.AudioChannels,
            HardwareAcceleration = profile.HardwareAcceleration,
            MinSourceWidth = profile.MinSourceWidth,
            MinSourceHeight = profile.MinSourceHeight,
            SourceCodecs = profile.SourceCodecs,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = profile.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateQualityProfileDto dto)
    {
        var profile = await _context.QualityProfiles.FindAsync(id);

        if (profile == null)
        {
            return NotFound();
        }

        profile.Name = dto.Name;
        profile.Description = dto.Description;
        profile.Enabled = dto.Enabled;
        profile.TargetResolution = dto.TargetResolution;
        profile.TargetCodec = dto.TargetCodec;
        profile.TargetBitrate = dto.TargetBitrate;
        profile.MinBitrate = dto.MinBitrate;
        profile.MaxBitrate = dto.MaxBitrate;
        profile.TargetFrameRate = dto.TargetFrameRate;
        profile.AudioCodec = dto.AudioCodec;
        profile.AudioBitrate = dto.AudioBitrate;
        profile.AudioChannels = dto.AudioChannels;
        profile.HardwareAcceleration = dto.HardwareAcceleration;
        profile.MinSourceWidth = dto.MinSourceWidth;
        profile.MinSourceHeight = dto.MinSourceHeight;
        profile.SourceCodecs = dto.SourceCodecs;
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var profile = await _context.QualityProfiles.FindAsync(id);

        if (profile == null)
        {
            return NotFound();
        }

        _context.QualityProfiles.Remove(profile);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
