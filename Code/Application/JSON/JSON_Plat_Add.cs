using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class PlatAdd : JSONMessage
    {
        [JsonProperty("PlatName")]
        public string PlatName { get; set; }

        [JsonProperty("PlatCategory")]
        public string PlatCategory { get; set; }

        [JsonProperty("ThreatType")]
        public string ThreatType { get; set; }

        [JsonProperty("Display")]
        public bool Display { get; set; }

        [JsonProperty("PlatClass")]
        public string PlatClass { get; set; }

        [JsonProperty("PlatDispSymb")]
        public string PlatDispSymb { get; set; }





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

        [JsonProperty("HeadingDegs")]
        public double HeadingDegs { get; set; }



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
            set { RollDegs = value.RollClockwiseDegs; PitchDegs = value.PitchUpDegs; }
        }

        [JsonIgnore]
        public GlobeCourse Course
        {
            get { return new GlobeCourse() { HeadingDegs = HeadingDegs }; }
            set { HeadingDegs = value.HeadingDegs; }
        }


        /*
        public JSONPlatAdd(string n, string t, GlobeLLA pos, GlobeCourse cor, GlobeAttitude att)
        {
            PlatformName = n;
            PlatformType = t;

            LatDegs           = pos.LatDegs;
            LonDegs           = pos.LonDegs;
            AltMslKm          = pos.AltMslKm;
            HeadingDegs       = cor.HeadingDegs;
            SpeedKph          = cor.SpeedKph;
            RollClockwiseDegs = att.RollClockwiseDegs;
            PitchUpDegs       = att.PitchUpDegs;
            YawClockwiseDegs  = att.YawClockwiseDegs;
        }

        public GlobeLLA GetPos()
        {
            GlobeLLA pos = new GlobeLLA();
            pos.LatDegs = LatDegs;
            pos.LonDegs = LonDegs;
            pos.AltMslKm = AltMslKm;

            return pos;
        }

        public GlobeCourse GetCourse()
        {
            GlobeCourse cor = new GlobeCourse();
            cor.HeadingDegs = HeadingDegs;
            cor.SpeedKph    = SpeedKph;

            return cor;
        }

        public GlobeAttitude GetAttitude()
        {
            GlobeAttitude att = new GlobeAttitude();
            att.RollClockwiseDegs = RollClockwiseDegs;
            att.PitchUpDegs       = PitchUpDegs;
            att.YawClockwiseDegs  = YawClockwiseDegs;

            return att;
        }
        */

        public static PlatAdd ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonContent = messageObj.GetValue("PlatAdd");
                if (JsonContent != null)
                {
                    string readPlatName          = (string)JsonContent["PlatName"];
                    string readPlatCategory      = (string)JsonContent["PlatCategory"];
                    string readThreatType        = (string)JsonContent["ThreatType"];
                    bool  readDisplay            =   (bool)JsonContent["Display"];
                    string readPlatClass         = (string)JsonContent["PlatClass"];
                    string readPlatDispSymb      = (string)JsonContent["PlatDispSymb"];

                    double readLatDegs           = (double)JsonContent["LatDegs"];
                    double readLongDegs          = (double)JsonContent["LongDegs"];
                    double readAltitudeMtrs      = (double)JsonContent["AltitudeMtrs"];

                    double readRollDegs          = (double)JsonContent["RollDegs"];
                    double readPitchDegs         = (double)JsonContent["PitchDegs"];
                    double readHeadingDegs       = (double)JsonContent["HeadingDegs"];

                    PlatAdd newMsg = new PlatAdd() {
                        PlatName          = readPlatName,
                        PlatCategory      = readPlatCategory,
                        ThreatType        = readThreatType,
                        Display           = readDisplay,
                        PlatClass         = readPlatClass,
                        PlatDispSymb      = readPlatDispSymb,

                        LatDegs           = readLatDegs,
                        LongDegs          = readLongDegs,
                        AltitudeMtrs      = readAltitudeMtrs,

                        RollDegs         = readRollDegs,
                        PitchDegs        = readPitchDegs,
                        HeadingDegs      = readHeadingDegs
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






