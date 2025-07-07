using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GloJSON
{
    public class PlatUpdate : JSONMessage
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

        [JsonPropertyName("HeadingDegs")]
        public double HeadingDegs { get; set; }

        [JsonPropertyName("GrndSpeedMtrSec")]
        public double GrndSpeedMtrSec { get; set; }

        [JsonPropertyName("ClimbRateMtrSec")]
        public double ClimbRateMtrSec { get; set; }

        [JsonPropertyName("RollRateDegsSec")]
        public double RollRateDegsSec { get; set; }

        [JsonPropertyName("PitchRateDegsSec")]
        public double PitchRateDegsSec { get; set; }

        [JsonPropertyName("YawRateDegsSec")]
        public double YawRateDegsSec { get; set; }

        [JsonPropertyName("TurnRateDegsSec")]
        public double TurnRateDegsSec { get; set; }

        [JsonIgnore]
        public GloLLAPoint Pos
        {
            get { return new GloLLAPoint() { LatDegs = LatDegs, LonDegs = LongDegs, AltMslM = AltitudeMtrs }; }
            set { LatDegs = value.LatDegs; LongDegs = value.LonDegs; AltitudeMtrs = value.AltMslM; }
        }

        [JsonIgnore]
        public GloAttitude Attitude
        {
            get { return new GloAttitude() { RollClockwiseDegs = RollDegs, PitchUpDegs = PitchDegs, YawClockwiseDegs = YawDegs }; }
            set { RollDegs = value.RollClockwiseDegs; PitchDegs = value.PitchUpDegs; YawDegs = value.YawClockwiseDegs; }
        }

        [JsonIgnore]
        public GloCourse Course
        {
            get { return new GloCourse() { HeadingDegs = HeadingDegs, SpeedMps = GrndSpeedMtrSec, ClimbRateMps = ClimbRateMtrSec }; }
            set { HeadingDegs = value.HeadingDegs; GrndSpeedMtrSec = value.SpeedMps; }
        }

        [JsonIgnore]
        public GloCourseDelta CourseDelta
        {
            get { return new GloCourseDelta() { SpeedChangeMpMps = 0, HeadingChangeClockwiseDegsSec = -TurnRateDegsSec }; }
            set { TurnRateDegsSec = -value.HeadingChangeClockwiseDegsSec; }
        }

        public static PlatUpdate ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("PlatUpdate", out JsonElement jsonContent))
                    {
                        PlatUpdate newMsg = JsonSerializer.Deserialize<PlatUpdate>(jsonContent.GetRawText());
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
