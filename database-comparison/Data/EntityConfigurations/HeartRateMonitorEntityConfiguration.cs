using DatabaseComparison.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseComparison.Data.EntityConfigurations;

public class HeartRateMonitorEntityConfiguration<TKey> : IEntityTypeConfiguration<HeartRateMonitor<TKey>> where TKey : struct
{
    public void Configure(EntityTypeBuilder<HeartRateMonitor<TKey>> builder)
    {
        builder.HasIndex(hrm => hrm.SmartWatchId);
        builder.HasIndex(hrm => hrm.RegisteredAt);
    }
}
