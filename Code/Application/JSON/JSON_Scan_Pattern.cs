using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class ScanPattern : JSONMessage
    {
        [JsonProperty("PlatName")]
        public string PlatName { get; set; }

        [JsonProperty("EmitName")]
        public string EmitName { get; set; }

        [JsonProperty("BeamName")]
        public string BeamName { get; set; }

        [JsonProperty("ScanType")]
        public string ScanType { get; set; }

        [JsonProperty("PeriodSecs")]
        public double PeriodSecs { get; set; }

        [JsonProperty("SegmentCount")]
        public int SegmentCount { get; set; }

        [JsonProperty("AzMinDegs")]
        public double AzMinDegs { get; set; }

        [JsonProperty("AzMaxDegs")]
        public double AzMaxDegs { get; set; }

        [JsonProperty("ElMinDegs")]
        public double ElMinDegs { get; set; }

        [JsonProperty("ElMaxDegs")]
        public double ElMaxDegs { get; set; }

        [JsonProperty("AzTrackOffsetDegs")]
        public double AzTrackOffsetDegs { get; set; }

        [JsonProperty("ElTrackOffsetDegs")]
        public double ElTrackOffsetDegs { get; set; }

        [JsonProperty("Clockwise")]
        public bool Clockwise { get; set; }

        [JsonProperty("Up")]
        public bool Up { get; set; }

        [JsonProperty("UniDir")]
        public bool UniDir { get; set; }

        [JsonProperty("Reversed")]
        public bool Reversed { get; set; }

        [JsonProperty("MinorScanType")]
        public string MinorScanType { get; set; }

        [JsonProperty("MinorScanPeriodSecs")]
        public double MinorScanPeriodSecs { get; set; }

        [JsonProperty("MinorScanUniDir")]
        public bool MinorScanUniDir { get; set; }

        [JsonProperty("MinorScanUp")]
        public bool MinorScanUp { get; set; }

        // ============================================================================================
        // Attribute Helper Routines
        // ============================================================================================

        /*
        public double DetectionRangeKms
        {
            get { return (DetectionRangeMtrs * GlobeConsts.MetresToKmMultiplier); }
            set { DetectionRangeMtrs = (value * GlobeConsts.KmToMetresMultiplier); }
        }

        public double DetectionRangeRxKms
        {
            get { return (DetectionRangeRxMtrs * GlobeConsts.MetresToKmMultiplier); }
            set { DetectionRangeRxMtrs = (value * GlobeConsts.KmToMetresMultiplier); }
        }

        public GlobeAzElBox AzElBox()
        {
            GlobeAzElBox azElBox = new GlobeAzElBox() { MinAzDegs = AzMinDegs, MaxAzDegs = AzMaxDegs, MinElDegs = ElMinDegs, MaxElDegs = ElMaxDegs };
            return azElBox;
        }

        public void SetAzElBox(GlobeAzElBox azElBox)
        {
            AzMinDegs = azElBox.MinAzDegs;
            AzMaxDegs = azElBox.MaxAzDegs;
            ElMinDegs = azElBox.MinElDegs;
            ElMaxDegs = azElBox.MaxElDegs;
        }
        */
        // ============================================================================================
        // Message Data Analysis
        // ============================================================================================

        public bool IsScanShapeHemisphere()
        {
            if (ScanType?.IndexOf("Helical", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }

        public bool IsScanShapeWedge()
        {
            if (ScanType?.IndexOf("Raster", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }

        public bool IsScanShapeConical()
        {
            if (ScanType?.IndexOf("Conical", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }

        /*
        public GlobeCourse GetCourse()
        {
            GlobeCourse cor = new GlobeCourse();
            cor.HeadingDegs = HeadingDegs;
            cor.SpeedKph    = SpeedKph;

            return cor;
        }

        public GlobeAttitude GetAttitude()
        {
            GlobeAttitude att = new GlobeAttitude();
            att.RollClockwiseDegs = RollClockwiseDegs;
            att.PitchUpDegs       = PitchUpDegs;
            att.YawClockwiseDegs  = YawClockwiseDegs;

            return att;
        }
        */

        public static ScanPattern ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken jsonContent = messageObj.GetValue("ScanPattern");
                if (jsonContent != null)
                {
                    ScanPattern newMsg = new ScanPattern()
                    {
                        PlatName             = jsonContent["PlatName"]?.Value<string>() ?? "UnknownPlatName",
                        EmitName             = jsonContent["EmitName"]?.Value<string>() ?? "UnknownEmitName",
                        BeamName             = jsonContent["BeamName"]?.Value<string>() ?? "UnknownBeamName",
                        ScanType             = jsonContent["ScanType"]?.Value<string>() ?? "UnknownScanType",
                        PeriodSecs           = jsonContent["PeriodSecs"]?.Value<double>() ?? 0.0,
                        AzMinDegs            = jsonContent["AzMinDegs"]?.Value<double>() ?? 0.0,
                        AzMaxDegs            = jsonContent["AzMaxDegs"]?.Value<double>() ?? 0.0,
                        ElMinDegs            = jsonContent["ElMinDegs"]?.Value<double>() ?? 0.0,
                        ElMaxDegs            = jsonContent["ElMaxDegs"]?.Value<double>() ?? 0.0,
                        AzTrackOffsetDegs    = jsonContent["AzTrackOffsetDegs"]?.Value<double>() ?? 0.0,
                        ElTrackOffsetDegs    = jsonContent["ElTrackOffsetDegs"]?.Value<double>() ?? 0.0,
                        Clockwise            = jsonContent["Clockwise"]?.Value<bool>() ?? false,
                        Up                   = jsonContent["Up"]?.Value<bool>() ?? false,
                        UniDir               = jsonContent["UniDir"]?.Value<bool>() ?? false,
                        Reversed             = jsonContent["Reversed"]?.Value<bool>() ?? false,
                        MinorScanType        = jsonContent["MinorScanType"]?.Value<string>() ?? "UnknownMinorScanType",
                        MinorScanPeriodSecs  = jsonContent["MinorScanPeriodSecs"]?.Value<double>() ?? 0.0,
                        MinorScanUniDir      = jsonContent["MinorScanUniDir"]?.Value<bool>() ?? false,
                        MinorScanUp          = jsonContent["MinorScanUp"]?.Value<bool>() ?? false
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






