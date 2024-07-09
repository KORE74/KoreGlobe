using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class BeamDisable : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; } = "UnknownPlatName";

        [JsonPropertyName("EmitName")]
        public string EmitName { get; set; } = "UnknownEmitName";

        [JsonPropertyName("BeamName")]
        public string BeamName { get; set; } = "UnknownBeamName";

        public static BeamDisable ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement jsonContent;
                    if (doc.RootElement.TryGetProperty("BeamDisable", out jsonContent))
                    {
                        BeamDisable newMsg = JsonSerializer.Deserialize<BeamDisable>(jsonContent.GetRawText());
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
