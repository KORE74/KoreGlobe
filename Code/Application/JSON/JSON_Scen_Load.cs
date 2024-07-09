using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class ScenLoad : JSONMessage
    {
        [JsonProperty("ScenName")]
        public string ScenName { get; set; }

        [JsonProperty("Lat")]
        public double Lat { get; set; }

        [JsonProperty("Long")]
        public double Long { get; set; }

        [JsonProperty("EarthModel")]
        public string EarthModel { get; set; }

        [JsonProperty("RxName")]
        public string RxName { get; set; }

        [JsonProperty("DFModel")]
        public string DFModel { get; set; }

        // ------------------------------------------------------------------------

        public GlobeLLA ScenPos { get { return new GlobeLLA() { LatDegs = Lat, LonDegs = Long }; } }

        // ------------------------------------------------------------------------

        public static ScenLoad ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonContent = messageObj.GetValue("ScenLoad");
                if (JsonContent != null)
                {
                    ScenLoad newMsg     = new ScenLoad();
                    string readScenTime = (string)JsonContent["ScenName"];
                    double Lat          = (double)JsonContent["LatDegs"];
                    double Long         = (double)JsonContent["LongDegs"];
                    string EarthModel   = (string)JsonContent["EarthModel"];
                    string RxName       = (string)JsonContent["RxName"];
                    string DFModel      = (string)JsonContent["DFModel"];

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


