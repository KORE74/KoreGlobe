using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class EntityMotionDelta : JSONMessage
    {
        // ----------------------------------------------------------------------------------------
        // MARK: JSON Properties
        // ----------------------------------------------------------------------------------------

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        // Attitude Delta
        [JsonPropertyName("RollRateDegsSec")]
        public double RollRateDegsSec { get; set; }

        [JsonPropertyName("PitchRateDegsSec")]
        public double PitchRateDegsSec { get; set; }

        [JsonPropertyName("YawRateDegsSec")]
        public double YawRateDegsSec { get; set; }

        // Course Delta
        [JsonPropertyName("HeadingChangeClockwiseDegsSec")]
        public double HeadingChangeClockwiseDegsSec { get; set; }

        [JsonPropertyName("SpeedChangeMpMps")]
        public double SpeedChangeMpMps { get; set; }

        // ----------------------------------------------------------------------------------------
        // MARK: Complex Accessors
        // ----------------------------------------------------------------------------------------

        [JsonIgnore]
        public FssAttitudeDelta AttitudeDelta
        {
            get
            {
                return new FssAttitudeDelta() {
                    RollRateClockwiseRadsPerSec = RollRateDegsSec,
                    PitchRateUpRadsPerSec       = PitchRateDegsSec,
                    YawRateClockwiseRadsPerSec  = YawRateDegsSec };
            }

            set
            {
                RollRateDegsSec  = value.RollRateClockwiseRadsPerSec;
                PitchRateDegsSec = value.PitchRateUpRadsPerSec;
                YawRateDegsSec   = value.YawRateClockwiseRadsPerSec;
            }
        }

        [JsonIgnore]
        public FssCourseDelta CourseDelta
        {
            get
            {
                return new FssCourseDelta() {
                    SpeedChangeMpMps              = SpeedChangeMpMps,
                    HeadingChangeClockwiseDegsSec = HeadingChangeClockwiseDegsSec };
            }
            set
            {
                SpeedChangeMpMps              = value.SpeedChangeMpMps;
                HeadingChangeClockwiseDegsSec = value.HeadingChangeClockwiseDegsSec;
            }
        }

        // ----------------------------------------------------------------------------------------
        // MARK: Constructors
        // ----------------------------------------------------------------------------------------

        public static EntityMotionDelta ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("EntityMotionDelta", out JsonElement jsonContent))
                    {
                        EntityMotionDelta newMsg = JsonSerializer.Deserialize<EntityMotionDelta>(jsonContent.GetRawText());
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
