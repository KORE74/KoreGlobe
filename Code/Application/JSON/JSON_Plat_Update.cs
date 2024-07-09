using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class PlatUpdate : JSONMessage
    {
        [JsonProperty("PlatName")]
        public string PlatName { get; set; }

        [JsonProperty("LatDegs")]
        public double LatDegs { get; set; }

        [JsonProperty("LongDegs")]
        public double LongDegs { get; set; }

        [JsonProperty("AltitudeMtrs")]
        public double AltitudeMtrs { get; set; }



        [JsonProperty("RollDegs")]
        public double RollDegs { get; set; }

        [JsonProperty("PitchDegs")]
        public double PitchDegs { get; set; }

        [JsonProperty("YawDegs")]
        public double YawDegs { get; set; }



        [JsonProperty("HeadingDegs")]
        public double HeadingDegs { get; set; }

        [JsonProperty("GrndSpeedMtrSec")]
        public double GrndSpeedMtrSec { get; set; }

        [JsonProperty("ClimbRateMtrSec")]
        public double ClimbRateMtrSec { get; set; }



        [JsonProperty("RollRateDegsSec")]
        public double RollRateDegsSec { get; set; }

        [JsonProperty("PitchRateDegsSec")]
        public double PitchRateDegsSec { get; set; }

        [JsonProperty("YawRateDegsSec")]
        public double YawRateDegsSec { get; set; }

        [JsonProperty("TurnRateDegsSec")]
        public double TurnRateDegsSec { get; set; }

        // --------------------------------------------------------------------

        [JsonIgnore]
        public GlobeLLA Pos
        {
            get { return new GlobeLLA() { LatDegs = LatDegs, LonDegs = LongDegs, AltMslM = AltitudeMtrs }; }
            set { LatDegs = value.LatDegs; LongDegs = value.LonDegs; AltitudeMtrs = value.AltMslM; }
        }

        [JsonIgnore]
        public GlobeAttitude Attitude
        {
            get { return new GlobeAttitude() { RollClockwiseDegs = RollDegs, PitchUpDegs = PitchDegs, YawClockwiseDegs = YawDegs }; }
            set { RollDegs = value.RollClockwiseDegs; PitchDegs = value.PitchUpDegs; }
        }

        [JsonIgnore]
        public GlobeCourse Course
        {
            get { return new GlobeCourse() { HeadingDegs = HeadingDegs, SpeedMps = GrndSpeedMtrSec}; }
            set { HeadingDegs = value.HeadingDegs; GrndSpeedMtrSec = value.SpeedMps; }
        }

        [JsonIgnore]
        public GlobeCourseDelta CourseDelta
        {
            get { return new GlobeCourseDelta() { SpeedChangeMpMps = 0, HeadingChangeClockwiseDegsSec = TurnRateDegsSec}; }
            set { TurnRateDegsSec = value.HeadingChangeClockwiseDegsSec; }
        }

        // --------------------------------------------------------------------

        public static PlatUpdate ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonContent = messageObj.GetValue("PlatUpdate");
                if (JsonContent != null)
                {
                    string readPlatName          = (string)JsonContent["PlatName"];

                    double readLatDegs           = (double)JsonContent["LatDegs"];
                    double readLongDegs          = (double)JsonContent["LongDegs"];
                    double readAltitudeMtrs      = (double)JsonContent["AltitudeMtrs"];

                    double readHeadingDegs       = (double)JsonContent["HeadingDegs"];
                    double readGrndSpeedMtrSec   = (double)JsonContent["GrndSpeedMtrSec"];
                    double readClimbRateMtrSec   = (double)JsonContent["ClimbRateMtrSec"];

                    double readRollDegs          = (double)JsonContent["RollDegs"];
                    double readPitchDegs         = (double)JsonContent["PitchDegs"];
                    double readYawDegs           = (double)JsonContent["YawDegs"];

                    double readRollRateDegsSec   = (double)JsonContent["RollRateDegsSec"];
                    double readPitchRateDegsSec  = (double)JsonContent["PitchRateDegsSec"];
                    double readYawRateDegsSec    = (double)JsonContent["YawRateDegsSec"];

                    double readTurnRateDegsSec   = (double)JsonContent["TurnRateDegsSec"];


                    PlatUpdate newMsg = new PlatUpdate() {
                        PlatName         = readPlatName,

                        LatDegs          = readLatDegs,
                        LongDegs         = readLongDegs,
                        AltitudeMtrs     = readAltitudeMtrs,

                        RollDegs         = readRollDegs,
                        PitchDegs        = readPitchDegs,
                        YawDegs          = readYawDegs,

                        GrndSpeedMtrSec = readGrndSpeedMtrSec,
                        ClimbRateMtrSec = readClimbRateMtrSec,
                        HeadingDegs      = readHeadingDegs,

                        RollRateDegsSec  = readRollRateDegsSec,
                        PitchRateDegsSec = readPitchRateDegsSec,
                        YawRateDegsSec   = readYawRateDegsSec,

                        TurnRateDegsSec  = readTurnRateDegsSec
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






