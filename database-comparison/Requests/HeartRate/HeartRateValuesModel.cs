using System.Text.Json.Serialization;

namespace DatabaseComparison.Requests.HeartRate;

public class HeartRateValuesModel
{
    [JsonPropertyName("registered_at")]
    public required DateTimeOffset RegisteredAt { get; set; }
    
    [JsonPropertyName("heart_rate")]
    public required int AverageHeartRate { get; set; }
    
    [JsonPropertyName("max")]
    public required int MaxHeartRate { get; set; }
    
    [JsonPropertyName("min")]
    public required int MinHeartRate { get; set; }
}