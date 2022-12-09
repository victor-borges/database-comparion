using System.Text.Json.Serialization;

namespace DatabaseComparison.Requests.HeartRate;

public class HeartRateMonitorRequest
{
    [JsonPropertyName("smart_watch_slug")]
    public required string SmartWatchSlug { get; set; }
    
    [JsonPropertyName("heart_rate_monitor")]
    public required List<HeartRateMonitorModel> HeartRateMonitor { get; set; }
}
