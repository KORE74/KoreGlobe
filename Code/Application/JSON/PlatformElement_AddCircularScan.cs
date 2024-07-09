using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class PlatformElement_AddCircularScan : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; } = "UnknownPlatName";

        [JsonPropertyName("ElementName")]
        public string ElementName { get; set; } = "UnknownElementName";

        [JsonPropertyName("RangeKm")]
        public double RangeKm { get; set; } = 0.0;

        [JsonPropertyName("HorizArcDegs")]
        public double HorizArcDegs { get; set; } = 0.0;

        [JsonPropertyName("VertArcBotDegs")]
        public double VertArcBotDegs { get; set; } = 0.0;

        [JsonPropertyName("VertArcTopDegs")]
        public double VertArcTopDegs { get; set; } = 0.0;

        [JsonPropertyName("VertRotateDegsPerSec")]
        public double VertRotateDegsPerSec { get; set; } = 0.0;

        [JsonPropertyName("VertRotateLimitDegs")]
        public double VertRotateLimitDegs { get; set; } = 0.0;

        [JsonPropertyName("OffsetXm")]
        public double OffsetXm { get; set; } = 0.0;

        [JsonPropertyName("OffsetYm")]
        public double OffsetYm { get; set; } = 0.0;

        [JsonPropertyName("OffsetZm")]
        public double OffsetZm { get; set; } = 0.0;

        [JsonPropertyName("ColorStr")]
        public string ColorStr { get; set; } = "XXX";

        public static PlatformElement_AddCircularScan ParseJSON(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    AllowTrailingCommas = true,
                };

                return JsonSerializer.Deserialize<PlatformElement_AddCircularScan>(json, options) ?? new PlatformElement_AddCircularScan();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SetupWedge(double rng, double h, double vBot, double vTop)
        {
            RangeKm = rng;
            HorizArcDegs = h;
            VertArcBotDegs = vBot;
            VertArcTopDegs = vTop;
        }

        public void SetupMotion(double dps, double limitDegs)
        {
            VertRotateDegsPerSec = dps;
            VertRotateLimitDegs = limitDegs;
        }

        public void SetupOffset(double x, double y, double z)
        {
            OffsetXm = x;
            OffsetYm = y;
            OffsetZm = z;
        }

        public FssAzElRangeBox RangeBox()
        {
            double HalfAzDEgs = HorizArcDegs / 2.0;

            return new FssAzElRangeBox()
            {
                MinAzDegs = -HalfAzDEgs,
                MaxAzDegs = HalfAzDEgs,
                MinElDegs = VertArcBotDegs,
                MaxElDegs = VertArcTopDegs,
                MinRangeKm = 0.080,
                MaxRangeKm = RangeKm
            };
        }

        public override string ToString()
        {
            return string.Format($"PlatformElement_AddWedge: {PlatName} {ElementName}  // {RangeKm} {HorizArcDegs} {VertArcBotDegs} {VertArcTopDegs}");
        }
    } // end class
} // end namespace
