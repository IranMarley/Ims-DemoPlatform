using Ims.DemoPlatform.Tasks.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ims.DemoPlatform.Tasks.API.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }

    public DbSet<ProjectTask> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(2000);
            entity.HasIndex(t => t.ProjectId);
            entity.HasIndex(t => t.AssignedUserId);

            entity.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(50);
        });
    }
}

