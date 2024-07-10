using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class BeamLoad : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; }

        [JsonPropertyName("EmitName")]
        public string EmitName { get; set; }

        [JsonPropertyName("ELNOT")]
        public string ELNOT { get; set; }

        [JsonPropertyName("BeamName")]
        public string BeamName { get; set; }

        [JsonPropertyName("Signal")]
        public string Signal { get; set; }

        [JsonPropertyName("ScanType")]
        public string ScanType { get; set; }

        [JsonPropertyName("Channel")]
        public int Channel { get; set; }

        [JsonPropertyName("ERPdBW")]
        public string ERPdBW { get; set; }

        [JsonPropertyName("FreqMinHz")]
        public double FreqMinHz { get; set; }

        [JsonPropertyName("FreqMaxHz")]
        public double FreqMaxHz { get; set; }

        [JsonPropertyName("AzMinDegs")]
        public double AzMinDegs { get; set; }

        [JsonPropertyName("AzMaxDegs")]
        public double AzMaxDegs { get; set; }

        [JsonPropertyName("ElMinDegs")]
        public double ElMinDegs { get; set; }

        [JsonPropertyName("ElMaxDegs")]
        public double ElMaxDegs { get; set; }

        [JsonPropertyName("PortRollDegs")]
        public double PortRollDegs { get; set; }

        [JsonPropertyName("PortPitchDegs")]
        public double PortPitchDegs { get; set; }

        [JsonPropertyName("PortYawDegs")]
        public double PortYawDegs { get; set; }

        [JsonPropertyName("Targeted")]
        public bool Targeted { get; set; }

        [JsonPropertyName("TargetPlatName")]
        public string TargetPlatName { get; set; }

        [JsonPropertyName("AzTrackOffsetDegs")]
        public double AzTrackOffsetDegs { get; set; }

        [JsonPropertyName("ElTrackOffsetDegs")]
        public double ElTrackOffsetDegs { get; set; }

        [JsonPropertyName("DetectionRangeMtrs")]
        public double DetectionRangeMtrs { get; set; }

        [JsonPropertyName("DetectionRangeRxMtrs")]
        public double DetectionRangeRxMtrs { get; set; }

        [JsonPropertyName("AntennaPattern")]
        public string AntennaPattern { get; set; }

        [JsonPropertyName("HasHW")]
        public bool HasHW { get; set; }

        // ============================================================================================
        // Attribute Helper Routines
        // ============================================================================================

        public double DetectionRangeKms
        {
            get { return (DetectionRangeMtrs * FssPosConsts.MetresToKmMultiplier); }
            set { DetectionRangeMtrs = (value * FssPosConsts.KmToMetresMultiplier); }
        }

        public double DetectionRangeRxKms
        {
            get { return (DetectionRangeRxMtrs * FssPosConsts.MetresToKmMultiplier); }
            set { DetectionRangeRxMtrs = (value * FssPosConsts.KmToMetresMultiplier); }
        }

        public FssAzElBox AzElBox()
        {
            FssAzElBox azElBox = new FssAzElBox() { MinAzDegs = AzMinDegs, MaxAzDegs = AzMaxDegs, MinElDegs = ElMinDegs, MaxElDegs = ElMaxDegs };
            return azElBox;
        }

        public void SetAzElBox(FssAzElBox azElBox)
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

        public static BeamLoad ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("BeamLoad", out JsonElement jsonContent))
                    {
                        BeamLoad newMsg = JsonSerializer.Deserialize<BeamLoad>(jsonContent.GetRawText());
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
