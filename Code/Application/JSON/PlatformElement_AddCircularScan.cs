
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{

    // {"PlatformElement_AddCircularScan":{"PlatName":"Type26-1","ElementName":"Type26-1-Spinner", "RangeKm": 10.0, "HorizArcDegs": 5.0, "VertArcBotDegs": 5.0, "VertArcTopDegs": 60.0, "VertRotateDegsPerSec": 10.0, "VertRotateLimitDegs": 0.0, "OffsetXm": 0.0, "OffsetYm": 0.0, "OffsetZm": 0.0, "ColorStr": "XXX"}}
    public class PlatformElement_AddCircularScan : JSONMessage
    {
        [JsonProperty("PlatName")]
        public string PlatName { get; set; }

        [JsonProperty("ElementName")]
        public string ElementName { get; set; }

        // - - - - - - - - - - - - - -

        [JsonProperty("RangeKm")]
        public double RangeKm { get; set; }

        [JsonProperty("HorizArcDegs")]
        public double HorizArcDegs { get; set; }

        [JsonProperty("VertArcBotDegs")]
        public double VertArcBotDegs { get; set; }

        [JsonProperty("VertArcTopDegs")]
        public double VertArcTopDegs { get; set; }

        // - - - - - - - - - - - - - -

        [JsonProperty("VertRotateDegsPerSec")]
        public double VertRotateDegsPerSec { get; set; }

        [JsonProperty("VertRotateLimitDegs")]
        public double VertRotateLimitDegs { get; set; }

        // - - - - - - - - - - - - - -

        [JsonProperty("OffsetXm")]
        public double OffsetXm { get; set; }

        [JsonProperty("OffsetYm")]
        public double OffsetYm { get; set; }

        [JsonProperty("OffsetZm")]
        public double OffsetZm { get; set; }

        // - - - - - - - - - - - - - -

        [JsonProperty("ColorStr")]
        public string ColorStr { get; set; }

        // - - - - - - - - - - - - - -

        public static PlatformElement_AddCircularScan ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken jsonContent = messageObj.GetValue("PlatformElement_AddCircularScan");
                if (jsonContent != null)
                {

                    PlatformElement_AddCircularScan newMsg = new PlatformElement_AddCircularScan()
                    {
                        PlatName             = jsonContent["PlatName"]?.Value<string>() ?? "UnknownPlatName",
                        ElementName          = jsonContent["ElementName"]?.Value<string>() ?? "UnknownElementName",
                        RangeKm              = jsonContent["RangeKm"]?.Value<double>() ?? 0.0,
                        HorizArcDegs         = jsonContent["HorizArcDegs"]?.Value<double>() ?? 0.0,
                        VertArcBotDegs       = jsonContent["VertArcBotDegs"]?.Value<double>() ?? 0.0,
                        VertArcTopDegs       = jsonContent["VertArcTopDegs"]?.Value<double>() ?? 0.0,
                        VertRotateDegsPerSec = jsonContent["VertRotateDegsPerSec"]?.Value<double>() ?? 0.0,
                        VertRotateLimitDegs  = jsonContent["VertRotateLimitDegs"]?.Value<double>() ?? 0.0,
                        OffsetXm             = jsonContent["OffsetXm"]?.Value<double>() ?? 0.0,
                        OffsetYm             = jsonContent["OffsetYm"]?.Value<double>() ?? 0.0,
                        OffsetZm             = jsonContent["OffsetZm"]?.Value<double>() ?? 0.0
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

        public void SetupWedge(double rng, double h, double vBot, double vTop)
        {
            RangeKm         = rng;
            HorizArcDegs    = h;
            VertArcBotDegs  = vBot;
            VertArcTopDegs  = vTop;
        }

        public void SetupMotion(double dps, double limitDegs)
        {
            VertRotateDegsPerSec = dps;
            VertRotateLimitDegs  = limitDegs;
        }

        public void SetupOffset(double x, double y, double z)
        {
            OffsetXm = x;
            OffsetYm = y;
            OffsetZm = z;
        }

        public GlobeAzElRangeBox RangeBox()
        {
            double HalfAzDEgs = HorizArcDegs / 2.0;

            GlobeAzElRangeBox azElRangeBox = new GlobeAzElRangeBox()
            {
                MinAzDegs  = -HalfAzDEgs,
                MaxAzDegs  =  HalfAzDEgs,
                MinElDegs  = VertArcBotDegs,
                MaxElDegs  = VertArcTopDegs,
                MinRangeKm = 0.080,
                MaxRangeKm = RangeKm
            };

            return azElRangeBox;
        }

        public override string ToString()
        {
            return string.Format($"PlatformElement_AddWedge: {PlatName} {ElementName}  // {RangeKm} {HorizArcDegs} {VertArcBotDegs} {VertArcTopDegs}");
        }

    } // end class

} // end namespace






