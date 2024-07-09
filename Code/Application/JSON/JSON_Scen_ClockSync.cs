using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class ClockSync : JSONMessage
    {
        [JsonProperty("ScenTimeHMS")]
        public string ScenTimeHMS { get; set; }

        public ClockSync()
        {

        }

        // -----------------------

        public float ScenTimeSecs
        {
            get { TimeSpan ts = TimeFunctions.ParseTimeString(ScenTimeHMS); return (float)ts.TotalSeconds; }
        }

        // -----------------------

        public static ClockSync ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonContent = messageObj.GetValue("ClockSync");
                if (JsonContent != null)
                {
                    string readScenTime = (string)JsonContent["ScenTimeHMS"];

                    ClockSync newMsg = new ClockSync();
                    newMsg.ScenTimeHMS = readScenTime;
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






