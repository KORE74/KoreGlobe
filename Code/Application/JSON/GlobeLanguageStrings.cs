using System.Collections;
using System.Collections.Generic;

// Static class wrapper for a single/central instance of a FssConfig.

public class FssLanguageStrings
{
    private static FssConfig configData;

    private static string JSONFilename = "language_strings.json";
    private static string ActiveLanguageKey = "ActiveLanguage";

    public enum AppLanguage {
        English,
        French,
        German
    };

    private static AppLanguage ActiveLanguage;

    private static Dictionary<AppLanguage, string> printableAppLanguage = new Dictionary<AppLanguage, string>()
    {
        { AppLanguage.English, "English" },
        { AppLanguage.French,  "French" },
        { AppLanguage.German,  "German" }
    };

    // --------------------------------------------------------------------------------------------

    static FssLanguageStrings()
    {
        configData = new FssConfig();
        configData.LoadOrCreateJSONConfig(JSONFilename);

        if (configData.HasParam(ActiveLanguageKey))
            ActiveLanguage = StringToLanguage(GetParam(ActiveLanguageKey));
        else
            SetActiveLanguage(AppLanguage.English);
    }

    // --------------------------------------------------------------------------------------------

    public static string LanguageToString(AppLanguage lang)
    {
        return printableAppLanguage[lang];
    }

    public static AppLanguage StringToLanguage(string langStr)
    {
        foreach (KeyValuePair<AppLanguage, string> entry in printableAppLanguage)
        {
            if (entry.Value.Equals(langStr))
            {
                return entry.Key;
            }
        }

        return default(AppLanguage); // return English if not found
    }

    public static void SetActiveLanguage(AppLanguage lang)
    {
        ActiveLanguage = lang;
        SetParam(ActiveLanguageKey, LanguageToString(lang));
    }

    public static string LookupName(string name)
    {
        return $"{name}_{LanguageToString(ActiveLanguage)}";
    }

    // --------------------------------------------------------------------------------------------

    public static void NextActiveLanguage()
    {
        int nextLanguageIndex = ((int)ActiveLanguage + 1) % System.Enum.GetValues(typeof(AppLanguage)).Length;
        SetActiveLanguage((AppLanguage)nextLanguageIndex);
    }

    public static void PrevActiveLanguage()
    {
        int prevLanguageIndex = ((int)ActiveLanguage - 1 + System.Enum.GetValues(typeof(AppLanguage)).Length) % System.Enum.GetValues(typeof(AppLanguage)).Length;
        SetActiveLanguage((AppLanguage)prevLanguageIndex);
    }

    public static string CurrActiveLanguage()
    {
        return LanguageToString(ActiveLanguage);
    }

    // --------------------------------------------------------------------------------------------

    public static void SetParam(string name, string value, bool WriteOnAssign = true)
    {
        configData.SetParam(LookupName(name), value, WriteOnAssign);
    }

    public static bool HasParam(string name)
    {
        return configData.HasParam(LookupName(name));
    }

    public static string GetParam(string name)
    {
        // If the requested parameter doesn't exist, create it
        if (!configData.HasParam(LookupName(name)))
        {
            SetParam(name, "undefined string");
        }
        return configData.GetParam(LookupName(name));
    }

}
