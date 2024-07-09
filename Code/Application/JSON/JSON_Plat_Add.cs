using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class PlatAdd : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; }

        [JsonPropertyName("PlatCategory")]
        public string PlatCategory { get; set; }

        [JsonPropertyName("ThreatType")]
        public string ThreatType { get; set; }

        [JsonPropertyName("Display")]
        public bool Display { get; set; }

        [JsonPropertyName("PlatClass")]
        public string PlatClass { get; set; }

        [JsonPropertyName("PlatDispSymb")]
        public string PlatDispSymb { get; set; }

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

        [JsonPropertyName("HeadingDegs")]
        public double HeadingDegs { get; set; }

        [JsonIgnore]
        public FssLLAPoint Pos
        {
            get { return new FssLLAPoint() { LatDegs = LatDegs, LonDegs = LongDegs, AltMslM = AltitudeMtrs }; }
            set { LatDegs = value.LatDegs; LongDegs = value.LonDegs; AltitudeMtrs = value.AltMslM; }
        }

        [JsonIgnore]
        public FssAttitude Attitude
        {
            get { return new FssAttitude() { RollClockwiseDegs = RollDegs, PitchUpDegs = PitchDegs, YawClockwiseDegs = 0.0 }; }
            set { RollDegs = value.RollClockwiseDegs; PitchDegs = value.PitchUpDegs; }
        }

        [JsonIgnore]
        public FssCourse Course
        {
            get { return new FssCourse() { HeadingDegs = HeadingDegs }; }
            set { HeadingDegs = value.HeadingDegs; }
        }

        public static PlatAdd ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("PlatAdd", out JsonElement jsonContent))
                    {
                        PlatAdd newMsg = JsonSerializer.Deserialize<PlatAdd>(jsonContent.GetRawText());
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
