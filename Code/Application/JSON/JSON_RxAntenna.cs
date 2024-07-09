using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class RxAntenna : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; } = "UnknownPlatName";

        [JsonPropertyName("PortName")]
        public string PortName { get; set; } = "UnknownPortName";

        [JsonPropertyName("AzOffsetDegs")]
        public double AzOffsetDegs { get; set; } = 0.0;

        [JsonPropertyName("ElOffsetDegs")]
        public double ElOffsetDegs { get; set; } = 0.0;

        [JsonPropertyName("AzMinDegs")]
        public double AzMinDegs { get; set; } = 0.0;

        [JsonPropertyName("ElMinDegs")]
        public double ElMinDegs { get; set; } = 0.0;

        [JsonPropertyName("AzSpanDegs")]
        public double AzSpanDegs { get; set; } = 0.0;

        [JsonPropertyName("ElSpanDegs")]
        public double ElSpanDegs { get; set; } = 0.0;

        [JsonPropertyName("AzPointsCount")]
        public int AzPointsCount { get; set; } = 0;

        [JsonPropertyName("ElPointsCount")]
        public int ElPointsCount { get; set; } = 0;

        [JsonPropertyName("Pattern")]
        public double Pattern { get; set; } = 0.0;

        public static RxAntenna ParseJSON(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                AllowTrailingCommas = true
            };

            return JsonSerializer.Deserialize<RxAntenna>(json, options) ?? new RxAntenna();
        }
    } // end class
} // end namespace
