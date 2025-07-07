using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GloJSON
{
    public class PlatPosition : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; }

        [JsonPropertyName("LatDegs")]
        public double LatDegs { get; set; }

        [JsonPropertyName("LongDegs")]
        public double LongDegs { get; set; }

        [JsonPropertyName("AltitudeMtrs")]
        public double AltitudeMtrs { get; set; }

        [JsonPropertyName("RollDegs")]
        public double RollDegs { get; set; }

        [JsonPropertyName("PitchDegs")]
        public double PitchDegs { get; set; }

        [JsonPropertyName("YawDegs")]
        public double YawDegs { get; set; }

        // ------------------------------------------------------------------------------------------------------------

        // Note: We currently receive a Yaw value that is actually the heading, so this is a fudge that
        // will ultimately need to be resolved.

        [JsonIgnore]
        public GloLLAPoint Pos
        {
            get { return new GloLLAPoint() { LatDegs = LatDegs, LonDegs = LongDegs, AltMslM = AltitudeMtrs }; }
            set { LatDegs = value.LatDegs; LongDegs = value.LonDegs; AltitudeMtrs = value.AltMslM; }
        }

        [JsonIgnore]
        public GloAttitude Attitude
        {
            get { return new GloAttitude() { RollClockwiseDegs = RollDegs, PitchUpDegs = PitchDegs, YawClockwiseDegs = 0 }; }
            set { RollDegs = value.RollClockwiseDegs; PitchDegs = value.PitchUpDegs; YawDegs = value.YawClockwiseDegs; }
        }

        [JsonIgnore]
        public GloCourse Course
        {
            get { return new GloCourse() { HeadingDegs = YawDegs, SpeedMps = 0 }; }
            set { YawDegs = value.HeadingDegs; }
        }

        // ------------------------------------------------------------------------------------------------------------

        public static PlatPosition ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("PlatPosition", out JsonElement jsonContent))
                    {
                        PlatPosition newMsg = JsonSerializer.Deserialize<PlatPosition>(jsonContent.GetRawText());
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
