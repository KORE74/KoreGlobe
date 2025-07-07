using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GloJSON
{
    public class ScenLoad : JSONMessage
    {
        [JsonPropertyName("ScenName")]
        public string ScenName { get; set; }

        [JsonPropertyName("Lat")]
        public double Lat { get; set; }

        [JsonPropertyName("Long")]
        public double Long { get; set; }

        [JsonPropertyName("EarthModel")]
        public string EarthModel { get; set; }

        [JsonPropertyName("RxName")]
        public string RxName { get; set; }

        [JsonPropertyName("DFModel")]
        public string DFModel { get; set; }

        // --------------------------------------------------------------------------------------------

        public GloLLAPoint ScenPos { get { return new GloLLAPoint() { LatDegs = Lat, LonDegs = Long }; } }

        // --------------------------------------------------------------------------------------------

        public static ScenLoad ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("ScenLoad", out JsonElement jsonContent))
                    {
                        ScenLoad newMsg = JsonSerializer.Deserialize<ScenLoad>(jsonContent.GetRawText());
                        return newMsg;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    } // end class
} // end namespace
