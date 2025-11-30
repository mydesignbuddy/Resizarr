using Microsoft.EntityFrameworkCore;
using Resizarr.Core.Entities;

namespace Resizarr.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<QualityProfile> QualityProfiles { get; set; } = null!;
    public DbSet<MediaFile> MediaFiles { get; set; } = null!;
    public DbSet<TranscodingJob> TranscodingJobs { get; set; } = null!;
    public DbSet<FolderConfiguration> FolderConfigurations { get; set; } = null!;
    public DbSet<JobHistory> JobHistories { get; set; } = null!;
    public DbSet<SystemConfiguration> SystemConfigurations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // QualityProfile
        modelBuilder.Entity<QualityProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TargetResolution).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TargetCodec).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AudioCodec).IsRequired().HasMaxLength(50);
            entity.Property(e => e.HardwareAcceleration)
                .HasConversion<string>();
        });

        // MediaFile
        modelBuilder.Entity<MediaFile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.FilePath).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FolderPath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasConversion<string>();
            
            entity.HasOne(e => e.QualityProfile)
                .WithMany(q => q.MediaFiles)
                .HasForeignKey(e => e.QualityProfileId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // TranscodingJob
        modelBuilder.Entity<TranscodingJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Priority);
            entity.Property(e => e.SourcePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.OutputPath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.TempPath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasConversion<string>();
            
            entity.HasOne(e => e.MediaFile)
                .WithMany(m => m.TranscodingJobs)
                .HasForeignKey(e => e.MediaFileId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.QualityProfile)
                .WithMany(q => q.TranscodingJobs)
                .HasForeignKey(e => e.QualityProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // FolderConfiguration
        modelBuilder.Entity<FolderConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Path).IsUnique();
            entity.Property(e => e.Path).IsRequired().HasMaxLength(500);
            
            entity.HasOne(e => e.QualityProfile)
                .WithMany(q => q.FolderConfigurations)
                .HasForeignKey(e => e.QualityProfileId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // JobHistory
        modelBuilder.Entity<JobHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CreatedAt);
            entity.Property(e => e.Action)
                .HasConversion<string>();
            
            entity.HasOne(e => e.MediaFile)
                .WithMany()
                .HasForeignKey(e => e.MediaFileId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.TranscodingJob)
                .WithMany(t => t.JobHistories)
                .HasForeignKey(e => e.TranscodingJobId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // SystemConfiguration
        modelBuilder.Entity<SystemConfiguration>(entity =>
        {
            entity.HasKey(e => e.Key);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired();
        });
    }
}
