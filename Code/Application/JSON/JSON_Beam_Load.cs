using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class BeamLoad : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; } = "DefaultPlatName";  // Default value

        [JsonPropertyName("EmitName")]
        public string EmitName { get; set; } = "DefaultEmitName";  // Default value

        [JsonPropertyName("ELNOT")]
        public string ELNOT { get; set; } = "DefaultELNOT";  // Default value

        [JsonPropertyName("BeamName")]
        public string BeamName { get; set; } = "DefaultBeamName";  // Default value

        [JsonPropertyName("Signal")]
        public string Signal { get; set; } = "DefaultSignal";  // Default value

        [JsonPropertyName("Channel")]
        public int Channel { get; set; } = 0;  // Default value

        [JsonPropertyName("ERPdBW")]
        public double ERPdBW { get; set; } = 0.0;  // Default value


        [JsonPropertyName("FreqMinHz")]
        public double FreqMinHz { get; set; } = 1000.0;  // Default frequency

        [JsonPropertyName("FreqMaxHz")]
        public double FreqMaxHz { get; set; } = 2000.0;  // Default frequency


        [JsonPropertyName("PortRollDegs")]
        public double PortRollDegs { get; set; } = 0.0;  // Default rotation

        [JsonPropertyName("PortPitchDegs")]
        public double PortPitchDegs { get; set; } = 0.0;  // Default rotation

        [JsonPropertyName("PortYawDegs")]
        public double PortYawDegs { get; set; } = 0.0;  // Default rotation


        [JsonPropertyName("Targeted")]
        public bool Targeted { get; set; } = false;  // Default targeted status

        [JsonPropertyName("TargetPlatName")]
        public string TargetPlatName { get; set; } = "DefaultTarget";  // Default value

        [JsonPropertyName("DetectionRangeMtrs")]
        public double DetectionRangeMtrs { get; set; } = 1000.0;  // Default range

        [JsonPropertyName("DetectionRangeRxMtrs")]
        public double DetectionRangeRxMtrs { get; set; } = 1000.0;  // Default range

        [JsonPropertyName("AntennaPattern")]
        public string AntennaPattern { get; set; } = "DefaultPattern";  // Default value

        [JsonPropertyName("HasHW")]
        public bool HasHW { get; set; } = false;  // Default hardware status

        // ============================================================================================
        // Attribute Helper Routines
        // ============================================================================================

        [JsonIgnore]
        public double DetectionRangeKms
        {
            get { return (DetectionRangeMtrs * FssPosConsts.MetresToKmMultiplier); }
            set { DetectionRangeMtrs = (value * FssPosConsts.KmToMetresMultiplier); }
        }

        [JsonIgnore]
        public double DetectionRangeRxKms
        {
            get { return (DetectionRangeRxMtrs * FssPosConsts.MetresToKmMultiplier); }
            set { DetectionRangeRxMtrs = (value * FssPosConsts.KmToMetresMultiplier); }
        }

        [JsonIgnore]
        public FssAttitude PortAttitude
        {
            get { return new FssAttitude() { RollClockwiseDegs = PortRollDegs, PitchUpDegs = PortPitchDegs, YawClockwiseDegs = PortYawDegs }; }
            set
            {
                PortRollDegs  = value.RollClockwiseDegs;
                PortPitchDegs = value.PitchUpDegs;
                PortYawDegs   = value.YawClockwiseDegs;
            }
        }

        // ============================================================================================
        // Message Parsing
        // ============================================================================================

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

            // try
            // {
            //     // var options = new JsonSerializerOptions
            //     // {
            //     //     PropertyNameCaseInsensitive = true
            //     //     //IgnoreUnknownProperties = true  // Ignore extra parameters
            //     // };

            //     BeamLoad newMsg = JsonSerializer.Deserialize<BeamLoad>(json);

            //     // Ensure defaults for missing parameters
            //     // newMsg.PlatName       ??= "DefaultPlatName";
            //     // newMsg.EmitName       ??= "DefaultEmitName";
            //     // newMsg.ELNOT          ??= "DefaultELNOT";
            //     // newMsg.BeamName       ??= "DefaultBeamName";
            //     // newMsg.Signal         ??= "DefaultSignal";
            //     // newMsg.TargetPlatName ??= "DefaultTarget";
            //     // newMsg.AntennaPattern ??= "DefaultPattern";

            //     return newMsg;
            // }
            // catch (Exception)
            // {
            //     return null;  // Handle parsing error
            // }
        }
    }
}
