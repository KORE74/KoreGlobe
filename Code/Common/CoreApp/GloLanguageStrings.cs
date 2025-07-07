using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GloLanguageStrings
{
    private static GloLanguageStrings _instance;
    private static readonly object _lock = new object();

    // Static constructor
    static GloLanguageStrings()
    {
        // Access the instance to ensure it is initialized
        _ = Instance;
    }

    private Dictionary<string, string> ConfigData;
    private string JSONFilename      = "LanguageStrings.json";
    private string ActiveLanguageKey = "ActiveLanguage";

    public enum AppLanguage
    {
        English,
        French,
        German
    };

    private AppLanguage ActiveLanguage = AppLanguage.English; // default ahead of being assigned from a config value

    private Dictionary<AppLanguage, string> printableAppLanguage = new Dictionary<AppLanguage, string>()
    {
        { AppLanguage.English, "English" },
        { AppLanguage.French,  "French" },
        { AppLanguage.German,  "German" }
    };

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor and Singleton
    // --------------------------------------------------------------------------------------------

    // Private constructor to prevent direct instantiation
    private GloLanguageStrings()
    {
        LoadOrCreateJSONConfig(JSONFilename);

        if (HasParam(ActiveLanguageKey))
            ActiveLanguage = StringToLanguage(GetParam(ActiveLanguageKey));
        else
            SetActiveLanguage(AppLanguage.English);
    }

    // Public static method to provide access to the singleton instance
    public static GloLanguageStrings Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new GloLanguageStrings();
                }
                return _instance;
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: JSON Config Management
    // --------------------------------------------------------------------------------------------

    private void LoadOrCreateJSONConfig(string filename)
    {
        if (File.Exists(filename))
        {
            string jsonString = File.ReadAllText(filename);
            ConfigData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
        }
        else
        {
            ConfigData = new Dictionary<string, string>();
            SaveJSONConfig(filename);
        }
    }

    private void SaveJSONConfig(string filename)
    {
        string jsonString = JsonSerializer.Serialize(ConfigData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filename, jsonString);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Language Name Lookup
    // --------------------------------------------------------------------------------------------

    public string LanguageToString(AppLanguage lang)
    {
        return printableAppLanguage[lang];
    }

    public AppLanguage StringToLanguage(string langStr)
    {
        foreach (KeyValuePair<AppLanguage, string> entry in printableAppLanguage)
        {
            if (entry.Value.Equals(langStr))
            {
                return entry.Key;
            }
        }

        GloCentralLog.AddEntry($"Language string '{langStr}' not found");
        return default(AppLanguage); // return English if not found
    }

    public string LookupName(string name)
    {
        return $"{name}_{LanguageToString(ActiveLanguage)}";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Active Language Setting
    // --------------------------------------------------------------------------------------------

    public void SetActiveLanguage(string langStr)
    {
        SetActiveLanguage(StringToLanguage(langStr));
    }

    public void SetActiveLanguage(AppLanguage lang)
    {
        ActiveLanguage = lang;
        SetParam(ActiveLanguageKey, LanguageToString(lang));

        // Write the active language to config file
        GloCentralConfig.Instance.SetParam("ActiveLanguage", GloLanguageStrings.Instance.CurrActiveLanguage());
        GloCentralConfig.Instance.WriteToFile();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Active Language Management
    // --------------------------------------------------------------------------------------------

    public void NextActiveLanguage()
    {
        int nextLanguageIndex = ((int)ActiveLanguage + 1) % System.Enum.GetValues(typeof(AppLanguage)).Length;
        SetActiveLanguage((AppLanguage)nextLanguageIndex);
    }

    public void PrevActiveLanguage()
    {
        int prevLanguageIndex = ((int)ActiveLanguage - 1 + System.Enum.GetValues(typeof(AppLanguage)).Length) % System.Enum.GetValues(typeof(AppLanguage)).Length;
        SetActiveLanguage((AppLanguage)prevLanguageIndex);
    }

    public string CurrActiveLanguage()
    {
        return LanguageToString(ActiveLanguage);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Param Access
    // --------------------------------------------------------------------------------------------

    public void SetParam(string name, string value, bool WriteOnAssign = true)
    {
        ConfigData[LookupName(name)] = value;
        if (WriteOnAssign)
        {
            SaveJSONConfig(JSONFilename);
        }
    }

    public bool HasParam(string name)
    {
        return ConfigData.ContainsKey(LookupName(name));
    }

    public string GetParam(string name)
    {
        // If the requested parameter doesn't exist, create it
        if (!ConfigData.ContainsKey(LookupName(name)))
        {
            SetParam(name, "undefined string", false);
            SaveJSONConfig(JSONFilename);
        }
        return ConfigData[LookupName(name)];
    }
}
