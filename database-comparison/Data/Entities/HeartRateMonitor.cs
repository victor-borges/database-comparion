namespace DatabaseComparison.Data.Entities;

public class HeartRateMonitor<TKey> where TKey : struct
{
    public TKey Id { get; set; }
    
    public required DateTimeOffset RegisteredAt { get; set; }
    
    public required int Average { get; set; }
    
    public required int Max { get; set; }
    
    public required int Min { get; set; }

    public required string SmartWatchId { get; set; }
}
