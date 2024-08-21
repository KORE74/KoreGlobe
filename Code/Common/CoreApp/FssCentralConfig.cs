using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public sealed class FssCentralConfig
{
    // Lazy initialization of the singleton instance
    private static readonly Lazy<FssCentralConfig> _instance =
        new Lazy<FssCentralConfig>(() => new FssCentralConfig());

    // The path where the configuration file will be stored
    private const string ConfigFilePath = "config.json";

    // Dictionary to store configuration parameters
    private Dictionary<string, JsonElement> _parameters;

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor and Singleton
    // --------------------------------------------------------------------------------------------

    // Static constructor to ensure the singleton is initialized on startup
    static FssCentralConfig()
    {
        // Force the instance to be created
        _ = Instance;

        // Force the app language
        //FssLanguageStrings.Instance.SetActiveLanguage( FssCentralConfig.Instance.GetParam<string>("ActiveLanguage") );
    }

    // Private constructor to prevent external instantiation
    private FssCentralConfig()
    {
        _parameters = new Dictionary<string, JsonElement>();
        ReadFromFile(); // Load existing configuration if the file exists
    }

    // Public property to access the singleton instance
    public static FssCentralConfig Instance => _instance.Value;

    // --------------------------------------------------------------------------------------------
    // MARK: Params
    // --------------------------------------------------------------------------------------------

    // Method to set a parameter value
    public void SetParam(string key, object value)
    {
        var jsonElement = JsonSerializer.SerializeToElement(value);
        _parameters[key] = jsonElement;
    }

    // Generic method to get a parameter value with a default fallback
    public T GetParam<T>(string key, T defaultValue = default)
    {
        if (_parameters.TryGetValue(key, out JsonElement value))
        {
            return value.Deserialize<T>();
        }

        // If the key doesn't exist, store and return the default value
        SetParam(key, defaultValue);
        return defaultValue;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Files
    // --------------------------------------------------------------------------------------------


    // Method to save the current parameters to the configuration file
    public void WriteToFile()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(_parameters, options);
        File.WriteAllText(ConfigFilePath, json);
    }

    // Method to load parameters from the configuration file
    private void ReadFromFile()
    {
        if (File.Exists(ConfigFilePath))
        {
            string json = File.ReadAllText(ConfigFilePath);
            _parameters = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new Dictionary<string, JsonElement>();
        }
    }
}
