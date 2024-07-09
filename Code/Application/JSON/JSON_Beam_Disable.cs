using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class BeamDisable : JSONMessage
    {
        [JsonProperty("PlatName")]
        public string PlatName { get; set; } = "UnknownPlatName";

        [JsonProperty("EmitName")]
        public string EmitName { get; set; } = "UnknownEmitName";

        [JsonProperty("BeamName")]
        public string BeamName { get; set; } = "UnknownBeamName";

        public static BeamDisable ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken jsonContent = messageObj.GetValue("BeamDisable");
                if (jsonContent != null)
                {
                    BeamDisable newMsg = new BeamDisable()
                    {
                        PlatName = jsonContent["PlatName"]?.Value<string>() ?? "UnknownPlatName",
                        EmitName = jsonContent["EmitName"]?.Value<string>() ?? "UnknownEmitName",
                        BeamName = jsonContent["BeamName"]?.Value<string>() ?? "UnknownBeamName"
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
