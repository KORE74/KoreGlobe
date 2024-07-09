using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class ScenStop : JSONMessage
    {

        public ScenStop()
        {

        }

        // -----------------------

        public static ScenStop ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonToken = messageObj.GetValue("ScenStop");
                if (JsonToken != null)
                {
                    ScenStop newMsg = new ScenStop();

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






