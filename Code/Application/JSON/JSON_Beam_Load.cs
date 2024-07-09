using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class BeamLoad : JSONMessage
    {
        [JsonProperty("PlatName")]
        public string PlatName { get; set; }

        [JsonProperty("EmitName")]
        public string EmitName { get; set; }

        [JsonProperty("ELNOT")]
        public string ELNOT { get; set; }

        [JsonProperty("BeamName")]
        public string BeamName { get; set; }

        [JsonProperty("Signal")]
        public string Signal { get; set; }

        [JsonProperty("ScanType")]
        public string ScanType { get; set; }

        [JsonProperty("Channel")]
        public int Channel { get; set; }

        [JsonProperty("ERPdBW")]
        public string ERPdBW { get; set; }


        [JsonProperty("FreqMinHz")]
        public double FreqMinHz { get; set; }

        [JsonProperty("FreqMaxHz")]
        public double FreqMaxHz { get; set; }

        [JsonProperty("AzMinDegs")]
        public double AzMinDegs { get; set; }

        [JsonProperty("AzMaxDegs")]
        public double AzMaxDegs { get; set; }

        [JsonProperty("ElMinDegs")]
        public double ElMinDegs { get; set; }

        [JsonProperty("ElMaxDegs")]
        public double ElMaxDegs { get; set; }

        [JsonProperty("PortRollDegs")]
        public double PortRollDegs { get; set; }
        [JsonProperty("PortPitchDegs")]
        public double PortPitchDegs { get; set; }
        [JsonProperty("PortYawDegs")]
        public double PortYawDegs { get; set; }

        [JsonProperty("Targeted")]
        public bool Targeted { get; set; }

        [JsonProperty("TargetPlatName")]
        public string TargetPlatName { get; set; }

        [JsonProperty("AzTrackOffsetDegs")]
        public double AzTrackOffsetDegs { get; set; }
        [JsonProperty("ElTrackOffsetDegs")]
        public double ElTrackOffsetDegs { get; set; }

        [JsonProperty("DetectionRangeMtrs")]
        public double DetectionRangeMtrs { get; set; }

        [JsonProperty("DetectionRangeRxMtrs")]
        public double DetectionRangeRxMtrs { get; set; }

        [JsonProperty("AntennaPattern")]
        public string AntennaPattern { get; set; }

        [JsonProperty("HasHW")]
        public bool HasHW { get; set; }

        // ============================================================================================
        // Attribute Helper Routines
        // ============================================================================================

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

        public static BeamLoad ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken jsonContent = messageObj.GetValue("BeamLoad");
                if (jsonContent != null)
                {
                    BeamLoad newMsg = new BeamLoad()
                    {
                        PlatName             = jsonContent["PlatName"]?.Value<string>() ?? "UnknownPlatName",
                        EmitName             = jsonContent["EmitName"]?.Value<string>() ?? "UnknownEmitName",
                        ELNOT                = jsonContent["ELNOT"]?.Value<string>() ?? "UnknownELNOT",
                        BeamName             = jsonContent["BeamName"]?.Value<string>() ?? "UnknownBeamName",
                        Signal               = jsonContent["Signal"]?.Value<string>() ?? "UnknownSignal",
                        ScanType             = jsonContent["ScanType"]?.Value<string>() ?? "UnknownScanType",
                        Channel              = jsonContent["Channel"]?.Value<int>() ?? 0,
                        ERPdBW               = jsonContent["ERPdBW"]?.Value<string>() ?? "UnknownERPdBW",
                        FreqMinHz            = jsonContent["FreqMinHz"]?.Value<double>() ?? 0.0,
                        FreqMaxHz            = jsonContent["FreqMaxHz"]?.Value<double>() ?? 0.0,
                        AzMinDegs            = jsonContent["AzMinDegs"]?.Value<double>() ?? 0.0,
                        AzMaxDegs            = jsonContent["AzMaxDegs"]?.Value<double>() ?? 0.0,
                        ElMinDegs            = jsonContent["ElMinDegs"]?.Value<double>() ?? 0.0,
                        ElMaxDegs            = jsonContent["ElMaxDegs"]?.Value<double>() ?? 0.0,
                        PortRollDegs         = jsonContent["PortRollDegs"]?.Value<double>() ?? 0.0,
                        PortPitchDegs        = jsonContent["PortPitchDegs"]?.Value<double>() ?? 0.0,
                        PortYawDegs          = jsonContent["PortYawDegs"]?.Value<double>() ?? 0.0,
                        Targeted             = jsonContent["Targeted"]?.Value<bool>() ?? false,
                        TargetPlatName       = jsonContent["TargetPlatName"]?.Value<string>() ?? "UnknownTargetPlatName",
                        AzTrackOffsetDegs    = jsonContent["AzTrackOffsetDegs"]?.Value<double>() ?? 0.0,
                        ElTrackOffsetDegs    = jsonContent["ElTrackOffsetDegs"]?.Value<double>() ?? 0.0,
                        DetectionRangeMtrs   = jsonContent["DetectionRangeMtrs"]?.Value<double>() ?? 0.0,
                        DetectionRangeRxMtrs = jsonContent["DetectionRangeRxMtrs"]?.Value<double>() ?? 0.0,
                        AntennaPattern       = jsonContent["AntennaPattern"]?.Value<string>() ?? "UnknownAntennaPattern",
                        HasHW                = jsonContent["HasHW"]?.Value<bool>() ?? false
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






