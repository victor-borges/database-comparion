using System.Text.Json.Serialization;

namespace DatabaseComparison.Responses.HeartRate;

public class HeartRateMonitorResponse
{
    [JsonPropertyName("id")]
    public DateTimeOffset Id { get; set; }
    
    [JsonPropertyName("status")]
    public int Status { get; set; } 
}
