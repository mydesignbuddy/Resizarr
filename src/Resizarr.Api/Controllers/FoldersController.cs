using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resizarr.Application.DTOs;
using Resizarr.Core.Entities;
using Resizarr.Infrastructure.Data;

namespace Resizarr.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FoldersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FoldersController> _logger;

    public FoldersController(
        ApplicationDbContext context,
        ILogger<FoldersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FolderConfigurationDto>>> GetAll()
    {
        var folders = await _context.FolderConfigurations
            .Include(f => f.QualityProfile)
            .OrderBy(f => f.Path)
            .ToListAsync();

        var dtos = folders.Select(f => new FolderConfigurationDto
        {
            Id = f.Id,
            Path = f.Path,
            Enabled = f.Enabled,
            Recursive = f.Recursive,
            QualityProfileId = f.QualityProfileId,
            QualityProfileName = f.QualityProfile?.Name,
            FileExtensions = f.FileExtensions,
            ExcludePatterns = f.ExcludePatterns,
            DeleteSourceAfterSuccess = f.DeleteSourceAfterSuccess,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt
        });

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FolderConfigurationDto>> GetById(int id)
    {
        var folder = await _context.FolderConfigurations
            .Include(f => f.QualityProfile)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (folder == null)
        {
            return NotFound();
        }

        var dto = new FolderConfigurationDto
        {
            Id = folder.Id,
            Path = folder.Path,
            Enabled = folder.Enabled,
            Recursive = folder.Recursive,
            QualityProfileId = folder.QualityProfileId,
            QualityProfileName = folder.QualityProfile?.Name,
            FileExtensions = folder.FileExtensions,
            ExcludePatterns = folder.ExcludePatterns,
            DeleteSourceAfterSuccess = folder.DeleteSourceAfterSuccess,
            CreatedAt = folder.CreatedAt,
            UpdatedAt = folder.UpdatedAt
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<FolderConfigurationDto>> Create(CreateFolderConfigurationDto dto)
    {
        var folder = new FolderConfiguration
        {
            Path = dto.Path,
            Enabled = dto.Enabled,
            Recursive = dto.Recursive,
            QualityProfileId = dto.QualityProfileId,
            FileExtensions = dto.FileExtensions,
            ExcludePatterns = dto.ExcludePatterns,
            DeleteSourceAfterSuccess = dto.DeleteSourceAfterSuccess
        };

        _context.FolderConfigurations.Add(folder);
        await _context.SaveChangesAsync();

        var resultDto = new FolderConfigurationDto
        {
            Id = folder.Id,
            Path = folder.Path,
            Enabled = folder.Enabled,
            Recursive = folder.Recursive,
            QualityProfileId = folder.QualityProfileId,
            FileExtensions = folder.FileExtensions,
            ExcludePatterns = folder.ExcludePatterns,
            DeleteSourceAfterSuccess = folder.DeleteSourceAfterSuccess,
            CreatedAt = folder.CreatedAt,
            UpdatedAt = folder.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = folder.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateFolderConfigurationDto dto)
    {
        var folder = await _context.FolderConfigurations.FindAsync(id);

        if (folder == null)
        {
            return NotFound();
        }

        folder.Path = dto.Path;
        folder.Enabled = dto.Enabled;
        folder.Recursive = dto.Recursive;
        folder.QualityProfileId = dto.QualityProfileId;
        folder.FileExtensions = dto.FileExtensions;
        folder.ExcludePatterns = dto.ExcludePatterns;
        folder.DeleteSourceAfterSuccess = dto.DeleteSourceAfterSuccess;
        folder.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var folder = await _context.FolderConfigurations.FindAsync(id);

        if (folder == null)
        {
            return NotFound();
        }

        _context.FolderConfigurations.Remove(folder);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/scan")]
    public async Task<IActionResult> TriggerScan(int id)
    {
        var folder = await _context.FolderConfigurations.FindAsync(id);

        if (folder == null)
        {
            return NotFound();
        }

        _logger.LogInformation("Manual scan triggered for folder: {Path}", folder.Path);
        return Accepted();
    }
}
