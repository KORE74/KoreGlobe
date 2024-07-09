using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class ScenStart : JSONMessage
    {

        public ScenStart()
        {

        }

        // -----------------------

        public static ScenStart ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonToken = messageObj.GetValue("ScenStart");
                if (JsonToken != null)
                {
                    ScenStart newMsg = new ScenStart();

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






