using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GloJSON
{
    public class BeamDelete : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; } = "UnknownPlatName";

        [JsonPropertyName("EmitName")]
        public string EmitName { get; set; } = "UnknownEmitName";

        [JsonPropertyName("BeamName")]
        public string BeamName { get; set; } = "UnknownBeamName";

        public static BeamDelete ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement jsonContent;
                    if (doc.RootElement.TryGetProperty("BeamDelete", out jsonContent))
                    {
                        BeamDelete newMsg = JsonSerializer.Deserialize<BeamDelete>(jsonContent.GetRawText());
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
