using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class ElementModel : JSONMessage
    {
        // ----------------------------------------------------------------------------------------
        // MARK: JSON Properties
        // ----------------------------------------------------------------------------------------

        [JsonPropertyName("Name")]
        public string Name { get; set; } = "Default";

        [JsonPropertyName("ModelName")]
        public string ModelName { get; set; } = "Default";

        // ----------------------------------------------------------------------------------------
        // MARK: Constructors
        // ----------------------------------------------------------------------------------------

        public static ElementModel ParseJSON(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true, // Optional: to allow trailing commas.
                    PropertyNameCaseInsensitive = false, // Optional: to ignore case sensitivity.
                };

                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("ElementModel", out JsonElement jsonContent))
                    {
                        ElementModel newMsg = JsonSerializer.Deserialize<ElementModel>(jsonContent.GetRawText(), options);
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

    }
}