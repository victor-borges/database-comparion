using DatabaseComparison.Data.Entities;
using DatabaseComparison.Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace DatabaseComparison.Data;

public class PostgresDbContext : DbContext
{
    public DbSet<HeartRateMonitor<Guid>> HeartRateMonitors { get; set; } = default!;

    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new HeartRateMonitorEntityConfiguration<Guid>());
    }
}
