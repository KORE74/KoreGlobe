using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class ScenPause : JSONMessage
    {
        [JsonPropertyName("ScenTime")]
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
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("ScenPause", out JsonElement jsonContent))
                    {
                        ScenPause newMsg = JsonSerializer.Deserialize<ScenPause>(jsonContent.GetRawText());
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
