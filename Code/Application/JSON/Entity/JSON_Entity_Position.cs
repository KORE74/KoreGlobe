using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class EntityPosition : JSONMessage
    {
        // ----------------------------------------------------------------------------------------
        // MARK: JSON Properties
        // ----------------------------------------------------------------------------------------

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        // Position
        [JsonPropertyName("LatDegs")]
        public double LatDegs { get; set; } = 0;

        [JsonPropertyName("LongDegs")]
        public double LongDegs { get; set; } = 0;

        [JsonPropertyName("AltMslMtrs")]
        public double AltitudeMtrs { get; set; } = 0;

        // Attitude
        [JsonPropertyName("RollClockwiseDegs")]
        public double RollClockwiseDegs { get; set; } = 0;

        [JsonPropertyName("PitchUpDegs")]
        public double PitchUpDegs { get; set; } = 0;

        [JsonPropertyName("YawClockwiseDegs")]
        public double YawClockwiseDegs { get; set; } = 0;

        // Course
        [JsonPropertyName("HeadingDegs")]
        public double HeadingDegs { get; set; } = 0;

        [JsonPropertyName("GrndSpeedMtrSec")]
        public double GrndSpeedMtrSec { get; set; } = 0;

        [JsonPropertyName("ClimbRateMtrSec")]
        public double ClimbRateMtrSec { get; set; } = 0;

        // ----------------------------------------------------------------------------------------
        // MARK: Complex Accessors
        // ----------------------------------------------------------------------------------------

        [JsonIgnore]
        public FssLLAPoint Pos
        {
            get { return new FssLLAPoint() { LatDegs = LatDegs, LonDegs = LongDegs, AltMslM = AltitudeMtrs }; }
            set { LatDegs = value.LatDegs; LongDegs = value.LonDegs; AltitudeMtrs = value.AltMslM; }
        }

        [JsonIgnore]
        public FssAttitude Attitude
        {
            get { return new FssAttitude() { RollClockwiseDegs = RollClockwiseDegs, PitchUpDegs = PitchUpDegs, YawClockwiseDegs = YawClockwiseDegs }; }
            set { RollClockwiseDegs = value.RollClockwiseDegs; PitchUpDegs = value.PitchUpDegs; YawClockwiseDegs = value.YawClockwiseDegs; }
        }

        [JsonIgnore]
        public FssCourse Course
        {
            get { return new FssCourse() { HeadingDegs = HeadingDegs, GroundSpeedMps = GrndSpeedMtrSec, ClimbRateMps = ClimbRateMtrSec }; }
            set { HeadingDegs = value.HeadingDegs; GrndSpeedMtrSec = value.GroundSpeedMps; ClimbRateMtrSec = value.ClimbRateMps; }
        }

        // ----------------------------------------------------------------------------------------
        // MARK: Constructors
        // ----------------------------------------------------------------------------------------

        public static EntityPosition ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("EntityPosition", out JsonElement jsonContent))
                    {
                        EntityPosition newMsg = JsonSerializer.Deserialize<EntityPosition>(jsonContent.GetRawText());
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
