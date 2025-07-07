using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GloJSON
{
    public class PlatDelete : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; }

        public static PlatDelete ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("PlatDelete", out JsonElement jsonContent))
                    {
                        PlatDelete newMsg = JsonSerializer.Deserialize<PlatDelete>(jsonContent.GetRawText());
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
