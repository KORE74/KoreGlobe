
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

// Class to manage a list of UI parameters dictating the size of various elements such and button and font sizes
// for the purpose of scaling the UI to different screen sizes.

// --------------------------------------------------------------------------------------------
// --------------------------------------------------------------------------------------------
// --------------------------------------------------------------------------------------------

// Custom JsonConverter for floats
public class CustomFloatConverter : JsonConverter<float>
{
    public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetSingle();
    }

    public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
    {
        // If its sufficiently close to an integer, write it as an integer
        if (FssValueUtils.IsInteger(value))
            writer.WriteNumberValue((int)value);
        else
            writer.WriteNumberValue(Math.Round(value, 2));
    }
}

// --------------------------------------------------------------------------------------------
// --------------------------------------------------------------------------------------------
// --------------------------------------------------------------------------------------------

public class FssUILayoutParams
{
    public Dictionary<string, float> Params = new Dictionary<string, float>();
    private const string ConfigFilePath = "layoutparams.json";

    public FssUILayoutParams()
    {
        DefaultParams();
    }

    // Set default values in the dictionary
    public void DefaultParams()
    {
        Params.Clear();
        Params["ButtonSize"]   = 24f;
        Params["ButtonMargin"] =  4f;
        Params["FontSize"]     =  4f;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Access
    // --------------------------------------------------------------------------------------------

    // Get the value of a parameter
    public float GetParam(string key)
    {
        if (Params.ContainsKey(key))
            return Params[key];
        else
            return 0f;
    }

    public void SetParam(string key, float value)
    {
        Params[key] = value;
    }

    public void SetScaledSizes(float scale)
    {
        DefaultParams();
        foreach (string key in Params.Keys.ToList())
            Params[key] *= scale;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Serialise
    // --------------------------------------------------------------------------------------------

    // Method to save the current parameters to the configuration file
    public void WriteToFile()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        options.Converters.Add(new CustomFloatConverter());
        string json = JsonSerializer.Serialize(Params, options);
        File.WriteAllText(ConfigFilePath, json);
    }

    // Load parameters from the configuration file
    public void ReadFromFile()
    {
        if (File.Exists(ConfigFilePath))
        {
            string json = File.ReadAllText(ConfigFilePath);
            Dictionary<string, float> NewParams = JsonSerializer.Deserialize<Dictionary<string, float>>(json) ?? new Dictionary<string, float>();

            // Now copy the params across into the new dictionary - so we don't remove any that are not in the file
            // in the case of typos, corruption etc.

            foreach (KeyValuePair<string, float> kvp in NewParams)
                Params[kvp.Key] = kvp.Value;
        }
    }
}