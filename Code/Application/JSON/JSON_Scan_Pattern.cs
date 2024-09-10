using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class ScanPattern : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; } = "UnknownPlatName";

        [JsonPropertyName("EmitName")]
        public string EmitName { get; set; } = "UnknownEmitName";

        [JsonPropertyName("BeamName")]
        public string BeamName { get; set; } = "UnknownBeamName";

        [JsonPropertyName("ScanType")]
        public string ScanType { get; set; } = "UnknownScanType";



        [JsonPropertyName("PeriodSecs")]
        public double PeriodSecs { get; set; } = 0.0;

        [JsonPropertyName("SegmentCount")]
        public int SegmentCount { get; set; } = 0;



        [JsonPropertyName("AzMinDegs")]
        public double AzMinDegs { get; set; } = 0.0;

        [JsonPropertyName("AzMaxDegs")]
        public double AzMaxDegs { get; set; } = 0.0;

        [JsonPropertyName("ElMinDegs")]
        public double ElMinDegs { get; set; } = 0.0;

        [JsonPropertyName("ElMaxDegs")]
        public double ElMaxDegs { get; set; } = 0.0;

        [JsonPropertyName("AzTrackOffsetDegs")]
        public double AzTrackOffsetDegs { get; set; } = 0.0;

        [JsonPropertyName("ElTrackOffsetDegs")]
        public double ElTrackOffsetDegs { get; set; } = 0.0;



        [JsonPropertyName("Clockwise")]
        public bool Clockwise { get; set; } = false;

        [JsonPropertyName("Up")]
        public bool Up { get; set; } = false;

        [JsonPropertyName("UniDir")]
        public bool UniDir { get; set; } = false;

        [JsonPropertyName("Reversed")]
        public bool Reversed { get; set; } = false;

        [JsonPropertyName("MinorScanType")]
        public string MinorScanType { get; set; } = "UnknownMinorScanType";

        [JsonPropertyName("MinorScanPeriodSecs")]
        public double MinorScanPeriodSecs { get; set; } = 0.0;

        [JsonPropertyName("MinorScanUniDir")]
        public bool MinorScanUniDir { get; set; } = false;

        [JsonPropertyName("MinorScanUp")]
        public bool MinorScanUp { get; set; } = false;

        public static ScanPattern ParseJSON(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    AllowTrailingCommas = true,
                };

                return JsonSerializer.Deserialize<ScanPattern>(json, options) ?? new ScanPattern();
            }
            catch (Exception)
            {
                return null;
            }
        }

        // ============================================================================================
        // Message Data Analysis
        // ============================================================================================

        public bool IsScanShapeHemisphere()
        {
            return ScanType?.IndexOf("Helical", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public bool IsScanShapeWedge()
        {
            return ScanType?.IndexOf("Raster", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public bool IsScanShapeConical()
        {
            return ScanType?.IndexOf("Conical", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // --------------------------------------------------------------------------------

        public FssAzElBox GetAzElBox()
        {
            return new FssAzElBox()
            {
                MinAzDegs = AzMinDegs,
                MaxAzDegs = AzMaxDegs,
                MinElDegs = ElMinDegs,
                MaxElDegs = ElMaxDegs,
            };
        }

        public FssPolarOffset GetTrackOffset()
        {
            return new FssPolarOffset()
            {
                AzDegs = AzTrackOffsetDegs,
                ElDegs = ElTrackOffsetDegs,
            };
        }

    } // end class
} // end namespace
