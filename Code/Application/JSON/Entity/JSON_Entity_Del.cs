using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{

    public class EntityDelete : JSONMessage
    {
        // ----------------------------------------------------------------------------------------
        // MARK: JSON Properties
        // ----------------------------------------------------------------------------------------

        [JsonPropertyName("Name")]
        public string PlatName { get; set; }

        // ----------------------------------------------------------------------------------------
        // MARK: Constructors
        // ----------------------------------------------------------------------------------------

        public static EntityDelete ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("EntityDelete", out JsonElement jsonContent))
                    {
                        EntityDelete newMsg = JsonSerializer.Deserialize<EntityDelete>(jsonContent.GetRawText());
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
