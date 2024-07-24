using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

/*
Usage:

    var config = FssCentralConfig.Instance;

    // Set parameters
    config.SetParameter("AppName", "MyApplication");
    config.SetParameter("MaxRetries", 5);
    config.SetParameter("IsEnabled", true);
    config.SetParameter("Timeout", 2.5);

    // Write parameters to file
    config.WriteToFile();

    // Read parameters
    string appName = config.GetParameter("AppName", "DefaultAppName");
    int maxRetries = config.GetParameter("MaxRetries", 3);
    bool isEnabled = config.GetParameter("IsEnabled", false);
    double timeout = config.GetParameter("Timeout", 1.0);
*/

public sealed class FssCentralConfig
{
    private static readonly Lazy<FssCentralConfig> _instance =
        new Lazy<FssCentralConfig>(() => new FssCentralConfig());

    private const string ConfigFilePath = "config.json";
    private Dictionary<string, JsonElement> _parameters;

    private FssCentralConfig()
    {
        _parameters = new Dictionary<string, JsonElement>();
        ReadFromFile();
    }

    public static FssCentralConfig Instance => _instance.Value;

    public void SetParameter(string key, object value)
    {
        var jsonElement = JsonSerializer.SerializeToElement(value);
        _parameters[key] = jsonElement;
    }

    public T GetParameter<T>(string key, T defaultValue = default)
    {
        // If the key exists, return the value
        if (_parameters.TryGetValue(key, out JsonElement value))
        {
            return value.Deserialize<T>();
        }

        // Otherwise, set the default value and return it
        SetParameter(key, defaultValue);
        return defaultValue;
    }

    public void WriteToFile()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(_parameters, options);
        File.WriteAllText(ConfigFilePath, json);
    }

    private void ReadFromFile()
    {
        if (File.Exists(ConfigFilePath))
        {
            string json = File.ReadAllText(ConfigFilePath);
            _parameters = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        }
    }
}
