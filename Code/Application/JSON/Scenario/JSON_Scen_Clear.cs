using System;
using System.Text.Json;

namespace FssJSON
{
    public class ScenClear : JSONMessage
    {
        public ScenClear()
        {
        }

        // -----------------------

        public static ScenClear ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("ScenClear", out JsonElement jsonToken))
                    {
                        ScenClear newMsg = JsonSerializer.Deserialize<ScenClear>(jsonToken.GetRawText());
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
