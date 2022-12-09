using DatabaseComparison.Data.Entities;
using DatabaseComparison.Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace DatabaseComparison.Data;

public class MySqlDbContext : DbContext
{
    public DbSet<HeartRateMonitor<long>> HeartRateMonitors { get; set; } = default!;

    public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new HeartRateMonitorEntityConfiguration<long>());
    }
}
