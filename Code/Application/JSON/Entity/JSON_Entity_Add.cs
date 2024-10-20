using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FssJSON
{
    public class EntityAdd : JSONMessage
    {
        // ----------------------------------------------------------------------------------------
        // MARK: JSON Properties
        // ----------------------------------------------------------------------------------------

        [JsonPropertyName("Name")]
        public string Name { get; set; } = "Default";

        [JsonPropertyName("LatDegs")]
        public double LatDegs { get; set; } = 0;

        [JsonPropertyName("LongDegs")]
        public double LongDegs { get; set; } = 0;

        [JsonPropertyName("AltMslMtrs")]
        public double AltitudeMtrs { get; set; } = 0;

        // ----------------------------------------------------------------------------------------
        // MARK: Complex Accessors
        // ----------------------------------------------------------------------------------------

        [JsonIgnore]
        public FssLLAPoint Pos
        {
            get { return new FssLLAPoint() { LatDegs = LatDegs, LonDegs = LongDegs, AltMslM = AltitudeMtrs }; }
            set { LatDegs = value.LatDegs; LongDegs = value.LonDegs; AltitudeMtrs = value.AltMslM; }
        }

        // ----------------------------------------------------------------------------------------
        // MARK: Constructors
        // ----------------------------------------------------------------------------------------

        public static EntityAdd ParseJSON(string json)
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
                    if (doc.RootElement.TryGetProperty("EntityAdd", out JsonElement jsonContent))
                    {
                        EntityAdd newMsg = JsonSerializer.Deserialize<EntityAdd>(jsonContent.GetRawText(), options);
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