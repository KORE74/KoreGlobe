using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class UpdateBeam : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; }

        [JsonPropertyName("EmitName")]
        public string EmitName { get; set; }

        [JsonPropertyName("BeamName")]
        public string BeamName { get; set; }

        [JsonPropertyName("TargetAz")]
        public double TargetAz { get; set; }

        [JsonPropertyName("TargetEl")]
        public double TargetEl { get; set; }

        public static UpdateBeam ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("UpdateBeam", out JsonElement jsonContent))
                    {
                        UpdateBeam newMsg = JsonSerializer.Deserialize<UpdateBeam>(jsonContent.GetRawText());
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
