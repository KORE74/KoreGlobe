using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class AntennaPattern : JSONMessage
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

        [JsonPropertyName("AzPointCount")]
        public int AzPointCount { get; set; } = 1;

        [JsonPropertyName("ElPointCount")]
        public int ElPointCount { get; set; } = 1;

        [JsonPropertyName("Pattern")]
        public List<double> Pattern { get; set; } = new List<double>();

        // -----------------------------------------------------
        // Complex accessors

        [JsonIgnore]
        public FssAzElBox AzElBox
        {
            get
            {
                return new FssAzElBox()
                {
                    MinAzDegs = AzMinDegs,
                    MaxAzDegs = AzMinDegs + AzSpanDegs,
                    MinElDegs = ElMinDegs,
                    MaxElDegs = ElMinDegs + ElSpanDegs
                };
            }
        }

        [JsonIgnore]
        public FssPolarOffset PolarOffset
        {
            get
            {
                return new FssPolarOffset()
                {
                    AzDegs = AzOffsetDegs,
                    ElDegs = ElOffsetDegs,
                    RangeM = 1.0
                };
            }
        }

        // -----------------------------------------------------

        public static AntennaPattern ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement jsonContent;
                    if (doc.RootElement.TryGetProperty("AntennaPattern", out jsonContent))
                    {
                        AntennaPattern newMsg = JsonSerializer.Deserialize<AntennaPattern>(jsonContent.GetRawText());
                        return newMsg;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

    } // end class
} // end namespace
