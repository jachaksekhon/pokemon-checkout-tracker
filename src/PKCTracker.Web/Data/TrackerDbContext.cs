using Microsoft.EntityFrameworkCore;
using PKCTracker.Web.Models.Entities;

namespace PKCTracker.Web.Data;

public class TrackerDbContext : DbContext
{
    public TrackerDbContext(DbContextOptions<TrackerDbContext> options)
        : base(options)
    {
    }
    public DbSet<Drop> Drops { get; set; }
    public DbSet<ProfileResult> ProfileResults { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProfileResult>()
        .HasIndex(p => new { p.DropId, p.Profile })
        .IsUnique();
    }
}