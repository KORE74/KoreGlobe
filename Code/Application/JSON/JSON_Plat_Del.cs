using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class PlatDelete : JSONMessage
    {
        [JsonProperty("PlatName")]
        public string PlatName { get; set; }

        /*
        public JSONPlatDel(string n)
        {
            PlatName = n;
        }
        */

        public static PlatDelete ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonContent = messageObj.GetValue("PlatDelete");
                if (JsonContent != null)
                {
                    string readPlatName = (string)JsonContent["PlatName"];

                    PlatDelete newMsg = new PlatDelete() {
                        PlatName = readPlatName
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






