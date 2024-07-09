using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class UpdateBeam : JSONMessage
    {
        [JsonProperty("PlatName")]
        public string PlatName { get; set; }

        [JsonProperty("EmitName")]
        public string EmitName { get; set; }

        [JsonProperty("BeamName")]
        public string BeamName { get; set; }

        [JsonProperty("TargetAz")]
        public double TargetAz { get; set; }

        [JsonProperty("TargetEl")]
        public double TargetEl { get; set; }

        public static UpdateBeam ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonContent = messageObj.GetValue("UpdateBeam");
                if (JsonContent != null)
                {
                    string readPlatName     = (string)JsonContent["PlatName"];
                    string readEmitName     = (string)JsonContent["EmitName"];
                    string readBeamName     = (string)JsonContent["BeamName"];
                    double readTargetAzDegs = (double)JsonContent["TargetAz"];
                    double readTargetElDegs = (double)JsonContent["TargetEl"];

                    UpdateBeam newMsg = new UpdateBeam() {
                        PlatName = readPlatName,
                        EmitName = readEmitName,
                        BeamName = readBeamName,
                        TargetAz = readTargetAzDegs,
                        TargetEl = readTargetElDegs
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

