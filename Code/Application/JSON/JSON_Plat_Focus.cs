using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GloJSON
{
    public class PlatFocus : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; }

        public static PlatFocus ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("PlatFocus", out JsonElement jsonContent))
                    {
                        PlatFocus newMsg = JsonSerializer.Deserialize<PlatFocus>(jsonContent.GetRawText());
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
