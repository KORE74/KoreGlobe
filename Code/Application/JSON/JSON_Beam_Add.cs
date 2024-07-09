using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public struct GlobeFreqRange
{
    public double MinFreqHz;
    public double MaxFreqHz;

    public GlobeFreqRange(double minHz, double maxHz)
    {
        MinFreqHz = minHz;
        MaxFreqHz = maxHz;
    }
}



namespace GlobeJSON
{
    public class AddBeam : JSONMessage
    {
        [JsonProperty("PlatName")]           public string PlatName { get; set; }  = "UnknownPlatName";
        [JsonProperty("EmitName")]           public string EmitName { get; set; } = "UnknownEmitName";
        [JsonProperty("ELNOT")]              public string ELNOT { get; set; } = "UnknownELNOT";
        [JsonProperty("BeamName")]           public string BeamName { get; set; } = "UnknownBeamName";
        [JsonProperty("BeamType")]           public string BeamType { get; set; } = "UnknownBeamType";
        [JsonProperty("Signal")]             public string Signal { get; set; } = "UnknownSignal";
        [JsonProperty("ScanType")]           public string ScanType { get; set; } = "UnknownScanType";
        [JsonProperty("Channel")]            public string Channel { get; set; } = "UnknownChannel";
        [JsonProperty("ERPdBW")]             public string ERPdBW { get; set; } = "UnknownERPdBW";
        [JsonProperty("FreqMinHz")]          public double FreqMinHz { get; set; } = 0.0;
        [JsonProperty("FreqMaxHz")]          public double FreqMaxHz { get; set; } = 0.0;
        [JsonProperty("AzMinDegs")]          public double AzMinDegs { get; set; } = 0.0;
        [JsonProperty("AzMaxDegs")]          public double AzMaxDegs { get; set; } = 0.0;
        [JsonProperty("ElMinDegs")]          public double ElMinDegs { get; set; } = 0.0;
        [JsonProperty("ElMaxDegs")]          public double ElMaxDegs { get; set; } = 0.0;
        [JsonProperty("Targeted")]           public string Targeted { get; set; } = "UnknownTargeted";
        [JsonProperty("DetectionRangeMtrs")] public double DetectionRangeMtrs { get; set; } = 0.0;
        [JsonProperty("AntennaPattern")]     public string AntennaPattern { get; set; } = "UnknownAntennaPattern";
        [JsonProperty("HasHW")]              public string HasHW { get; set; } = "UnknownHasHW";

        // ----------------------------------------------------------------------------------------

        [JsonIgnore] // Ignore this property for JSON serialization
        public GlobeAzElBox AzElBox
        {
            get
            {
                return new GlobeAzElBox()
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
                JObject messageObj = JObject.Parse(json);
                JToken jsonContent = messageObj.GetValue("AddBeam");
                if (jsonContent != null)
                {
                    AddBeam newMsg = new AddBeam
                    {
                        PlatName           = jsonContent["PlatName"]?.Value<string>() ?? "UnknownPlatName",
                        EmitName           = jsonContent["EmitName"]?.Value<string>() ?? "UnknownEmitName",
                        ELNOT              = jsonContent["ELNOT"]?.Value<string>() ?? "UnknownELNOT",
                        BeamName           = jsonContent["BeamName"]?.Value<string>() ?? "UnknownBeamName",
                        BeamType           = jsonContent["BeamType"]?.Value<string>() ?? "UnknownBeamType",
                        Signal             = jsonContent["Signal"]?.Value<string>() ?? "UnknownSignal",
                        ScanType           = jsonContent["ScanType"]?.Value<string>() ?? "UnknownScanType",
                        Channel            = jsonContent["Channel"]?.Value<string>() ?? "UnknownChannel",
                        ERPdBW             = jsonContent["ERPdBW"]?.Value<string>() ?? "UnknownERPdBW",
                        FreqMinHz          = jsonContent["FreqMinHz"]?.Value<double>() ?? 0.0,
                        FreqMaxHz          = jsonContent["FreqMaxHz"]?.Value<double>() ?? 0.0,
                        AzMinDegs          = jsonContent["AzMinDegs"]?.Value<double>() ?? 0.0,
                        AzMaxDegs          = jsonContent["AzMaxDegs"]?.Value<double>() ?? 0.0,
                        ElMinDegs          = jsonContent["ElMinDegs"]?.Value<double>() ?? 0.0,
                        ElMaxDegs          = jsonContent["ElMaxDegs"]?.Value<double>() ?? 0.0,
                        Targeted           = jsonContent["Targeted"]?.Value<string>() ?? "UnknownTargeted",
                        DetectionRangeMtrs = jsonContent["DetectionRangeMtrs"]?.Value<double>() ?? 0.0,
                        AntennaPattern     = jsonContent["AntennaPattern"]?.Value<string>() ?? "UnknownAntennaPattern",
                        HasHW              = jsonContent["HasHW"]?.Value<string>() ?? "UnknownHasHW"
                    };

                    return newMsg;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    } // end class

} // end namespace






