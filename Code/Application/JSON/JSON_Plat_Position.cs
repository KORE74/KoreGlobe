using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
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

        [JsonIgnore]
        public FssLLAPoint Pos
        {
            get { return new FssLLAPoint() { LatDegs = LatDegs, LonDegs = LongDegs, AltMslM = AltitudeMtrs }; }
            set { LatDegs = value.LatDegs; LongDegs = value.LonDegs; AltitudeMtrs = value.AltMslM; }
        }

        [JsonIgnore]
        public FssAttitude Attitude
        {
            get { return new FssAttitude() { RollClockwiseDegs = RollDegs, PitchUpDegs = PitchDegs, YawClockwiseDegs = YawDegs }; }
            set { RollDegs = value.RollClockwiseDegs; PitchDegs = value.PitchUpDegs; YawDegs = value.YawClockwiseDegs; }
        }

        [JsonIgnore]
        public FssCourse Course
        {
            get { return new FssCourse() { HeadingDegs = YawDegs, SpeedMps = 0 }; }
            set { YawDegs = value.HeadingDegs; }
        }

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
