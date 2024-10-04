using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public struct FssFreqRange
{
    public double MinFreqHz;
    public double MaxFreqHz;

    public FssFreqRange(double minHz, double maxHz)
    {
        MinFreqHz = minHz;
        MaxFreqHz = maxHz;
    }
}

namespace FssJSON
{
    public class AddBeam : JSONMessage
    {
        [JsonPropertyName("PlatName")]           public string PlatName           { get; set; } = "UnknownPlatName";
        [JsonPropertyName("EmitName")]           public string EmitName           { get; set; } = "UnknownEmitName";
        [JsonPropertyName("ELNOT")]              public string ELNOT              { get; set; } = "UnknownELNOT";
        [JsonPropertyName("BeamName")]           public string BeamName           { get; set; } = "UnknownBeamName";
        [JsonPropertyName("BeamType")]           public string BeamType           { get; set; } = "UnknownBeamType";
        [JsonPropertyName("Signal")]             public string Signal             { get; set; } = "UnknownSignal";
        [JsonPropertyName("ScanType")]           public string ScanType           { get; set; } = "UnknownScanType";
        [JsonPropertyName("Channel")]            public string Channel            { get; set; } = "UnknownChannel";
        [JsonPropertyName("ERPdBW")]             public string ERPdBW             { get; set; } = "UnknownERPdBW";
        [JsonPropertyName("FreqMinHz")]          public double FreqMinHz          { get; set; } = 0.0;
        [JsonPropertyName("FreqMaxHz")]          public double FreqMaxHz          { get; set; } = 0.0;
        [JsonPropertyName("AzMinDegs")]          public double AzMinDegs          { get; set; } = 0.0;
        [JsonPropertyName("AzMaxDegs")]          public double AzMaxDegs          { get; set; } = 0.0;
        [JsonPropertyName("ElMinDegs")]          public double ElMinDegs          { get; set; } = 0.0;
        [JsonPropertyName("ElMaxDegs")]          public double ElMaxDegs          { get; set; } = 0.0;
        [JsonPropertyName("Targeted")]           public string Targeted           { get; set; } = "UnknownTargeted";
        [JsonPropertyName("DetectionRangeMtrs")] public double DetectionRangeMtrs { get; set; } = 0.0;
        [JsonPropertyName("AntennaPattern")]     public string AntennaPattern     { get; set; } = "UnknownAntennaPattern";
        [JsonPropertyName("HasHW")]              public string HasHW              { get; set; } = "UnknownHasHW";

        // ----------------------------------------------------------------------------------------

        [JsonIgnore] // Ignore this property for JSON serialization
        public FssAzElBox AzElBox
        {
            get
            {
                return new FssAzElBox()
                {
                    MinAzDegs = AzMinDegs,
                    MaxAzDegs = AzMaxDegs,
                    MinElDegs = ElMinDegs,
                    MaxElDegs = ElMaxDegs
                };
            }
            set
            {
                AzMinDegs = value.MinAzDegs;
                AzMaxDegs = value.MaxAzDegs;
                ElMinDegs = value.MinElDegs;
                ElMaxDegs = value.MaxElDegs;
            }
        }

        // ----------------------------------------------------------------------------------------

        public static AddBeam ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement jsonContent;
                    if (doc.RootElement.TryGetProperty("AddBeam", out jsonContent))
                    {
                        AddBeam newMsg = JsonSerializer.Deserialize<AddBeam>(jsonContent.GetRawText());
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
