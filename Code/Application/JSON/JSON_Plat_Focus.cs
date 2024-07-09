using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class PlatFocus : JSONMessage
    {
        [JsonProperty("PlatName")]
        public string PlatName { get; set; }






        public static PlatFocus ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonContent = messageObj.GetValue("PlatFocus");
                if (JsonContent != null)
                {
                    string readPlatName          = (string)JsonContent["PlatName"];


                    PlatFocus newMsg = new PlatFocus() {
                        PlatName          = readPlatName
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






