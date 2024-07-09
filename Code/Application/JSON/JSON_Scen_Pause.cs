using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class ScenPause : JSONMessage
    {

        [JsonProperty("ScenTime")]
        public string ScenTime { get; set; }

        public ScenPause()
        {

        }

        // -----------------------

        public float ScenTimeSecs
        {
            get { TimeSpan ts = TimeFunctions.ParseTimeString(ScenTime); return (float)ts.TotalSeconds; }
        }

        // -----------------------

        public static ScenPause ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonContent = messageObj.GetValue("ScenPause");
                if (JsonContent != null)
                {

                    string readScenTime = (string)JsonContent["ScenTime"];

                    ScenPause newMsg = new ScenPause();
                    newMsg.ScenTime = readScenTime;
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






