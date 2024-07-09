using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class PlatPosition : JSONMessage
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


        [JsonIgnore]
        public GlobeLLA Pos
        {
            get { return new GlobeLLA() { LatDegs = LatDegs, LonDegs = LongDegs, AltMslM = AltitudeMtrs }; }
            set { LatDegs = value.LatDegs; LongDegs = value.LonDegs; AltitudeMtrs = value.AltMslM; }
        }

        [JsonIgnore]
        public GlobeAttitude Attitude
        {
            get { return new GlobeAttitude() { RollClockwiseDegs = RollDegs, PitchUpDegs = PitchDegs, YawClockwiseDegs = 0.0 }; }
            set { RollDegs = value.RollClockwiseDegs; PitchDegs = value.PitchUpDegs;  YawDegs = value.YawClockwiseDegs; }
        }

        [JsonIgnore]
        public GlobeCourse Course
        {
            get { return new GlobeCourse() { HeadingDegs = YawDegs, SpeedMps = 0}; }
            set { YawDegs = value.HeadingDegs; }
        }



        public static PlatPosition ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonContent = messageObj.GetValue("PlatPosition");
                if (JsonContent != null)
                {
                    string readPlatName          = (string)JsonContent["PlatName"];

                    double readLatDegs           = (double)JsonContent["LatDegs"];
                    double readLongDegs          = (double)JsonContent["LongDegs"];
                    double readAltitudeMtrs      = (double)JsonContent["AltitudeMtrs"];

                    double readRollDegs          = (double)JsonContent["RollDegs"];
                    double readPitchDegs         = (double)JsonContent["PitchDegs"];
                    double readYawDegs           = (double)JsonContent["YawDegs"];

                    PlatPosition newMsg = new PlatPosition() {
                        PlatName          = readPlatName,

                        LatDegs           = readLatDegs,
                        LongDegs          = readLongDegs,
                        AltitudeMtrs      = readAltitudeMtrs,

                        RollDegs         = readRollDegs,
                        PitchDegs        = readPitchDegs,
                        YawDegs          = readYawDegs
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






