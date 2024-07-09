using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class RxAntenna : JSONMessage
    {
        [JsonProperty("PlatName")]
        public string PlatName { get; set; }
        [JsonProperty("PortName")]
        public string PortName { get; set; }

        [JsonProperty("AzOffsetDegs")]
        public double AzOffsetDegs { get; set; }
        [JsonProperty("ElOffsetDegs")]
        public double ElOffsetDegs { get; set; }
        [JsonProperty("AzMinDegs")]
        public double AzMinDegs { get; set; }
        [JsonProperty("ElMinDegs")]
        public double ElMinDegs { get; set; }
        [JsonProperty("AzSpanDegs")]
        public double AzSpanDegs { get; set; }
        [JsonProperty("ElSpanDegs")]
        public double ElSpanDegs { get; set; }
        [JsonProperty("AzPointsCount")]
        public int AzPointsCount { get; set; }
        [JsonProperty("ElPointsCount")]
        public int ElPointsCount { get; set; }

        [JsonProperty("Pattern")]
        public double Pattern { get; set; }

        public static RxAntenna ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken jsonContent = messageObj.GetValue("RxAntenna");
                if (jsonContent != null)
                {
                    RxAntenna newMsg = new RxAntenna()
                    {
                        PlatName      = jsonContent["PlatName"]?.Value<string>() ?? "UnknownPlatName",
                        PortName      = jsonContent["PortName"]?.Value<string>() ?? "UnknownPlatName",
                        AzOffsetDegs  = jsonContent["AzOffsetDegs"]?.Value<double>() ?? 0.0,
                        ElOffsetDegs  = jsonContent["ElOffsetDegs"]?.Value<double>() ?? 0.0,
                        AzMinDegs     = jsonContent["AzMinDegs"]?.Value<double>() ?? 0.0,
                        ElMinDegs     = jsonContent["ElMinDegs"]?.Value<double>() ?? 0.0,
                        AzSpanDegs    = jsonContent["AzSpanDegs"]?.Value<double>() ?? 0.0,
                        ElSpanDegs    = jsonContent["ElSpanDegs"]?.Value<double>() ?? 0.0,
                        AzPointsCount = jsonContent["AzPointsCount"]?.Value<int>() ?? 0,
                        ElPointsCount = jsonContent["ElPointsCount"]?.Value<int>() ?? 0,
                        Pattern       = jsonContent["Pattern"]?.Value<double>() ?? 0.0
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






