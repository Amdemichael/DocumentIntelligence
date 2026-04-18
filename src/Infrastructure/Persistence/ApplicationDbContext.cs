using Microsoft.EntityFrameworkCore;
using Domain.Aggregates;
using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Documents");
            entity.HasKey(d => d.Id);

            // Configure DocumentName value object
            entity.Property(d => d.Name)
                .HasConversion(
                    v => v.ToString(),
                    v => DocumentName.Create(v))
                .HasMaxLength(255)
                .IsRequired();

            // Configure BlobUrl value object
            entity.Property(d => d.BlobUrl)
                .HasConversion(
                    v => v.ToString(),
                    v => BlobUrl.Create(v))
                .HasMaxLength(500)
                .IsRequired();

            // Configure DocumentStatus
            entity.Property(d => d.Status)
                .HasConversion(
                    v => v.Id,
                    v => DocumentStatus.FromValue(v))
                .IsRequired();

            // Configure Email value object
            entity.Property(d => d.UploadedBy)
                .HasConversion(
                    v => v.ToString(),
                    v => Email.Create(v))
                .HasMaxLength(100)
                .IsRequired();

            // Configure FileSize value object
            entity.Property(d => d.Size)
                .HasConversion(
                    v => v.Bytes,
                    v => FileSize.FromBytes(v))
                .IsRequired();

            // Regular properties - SQLite uses TEXT instead of nvarchar(max)
            entity.Property(d => d.Description)
                .HasColumnType("TEXT")
                .HasMaxLength(500);

            entity.Property(d => d.ProcessedBy)
                .HasColumnType("TEXT")
                .HasMaxLength(100);

            entity.Property(d => d.ApprovedBy)
                .HasColumnType("TEXT")
                .HasMaxLength(100);

            entity.Property(d => d.ApprovalComments)
                .HasColumnType("TEXT")
                .HasMaxLength(1000);

            // For SQLite, use TEXT instead of nvarchar(max)
            entity.Property(d => d.ExtractedData)
                .HasColumnType("TEXT");

            // Indexes for performance
            entity.HasIndex(d => new { d.Status, d.CreatedAt })
                .HasDatabaseName("IX_Documents_Status_CreatedAt");

            entity.HasIndex(d => d.CreatedBy)
                .HasDatabaseName("IX_Documents_CreatedBy");

            entity.HasIndex(d => d.CreatedAt)
                .HasDatabaseName("IX_Documents_CreatedAt");
        });

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update audit fields before saving
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}