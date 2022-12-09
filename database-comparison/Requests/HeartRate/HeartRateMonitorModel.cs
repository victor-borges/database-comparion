using System.Text.Json.Serialization;

namespace DatabaseComparison.Requests.HeartRate;

public class HeartRateMonitorModel
{
    [JsonPropertyName("date")]
    public required DateTimeOffset ValuesDateTime { get; set; }
    
    [JsonPropertyName("avgValues")]
    public required List<HeartRateValuesModel> HeartRateValues { get; set; }
}