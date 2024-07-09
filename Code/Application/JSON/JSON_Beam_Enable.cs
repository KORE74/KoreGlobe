using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class BeamEnable : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; } = "UnknownPlatName";

        [JsonPropertyName("EmitName")]
        public string EmitName { get; set; } = "UnknownEmitName";

        [JsonPropertyName("BeamName")]
        public string BeamName { get; set; } = "UnknownBeamName";

        public static BeamEnable ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("BeamEnable", out JsonElement jsonContent))
                    {
                        BeamEnable newMsg = JsonSerializer.Deserialize<BeamEnable>(jsonContent.GetRawText());
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
