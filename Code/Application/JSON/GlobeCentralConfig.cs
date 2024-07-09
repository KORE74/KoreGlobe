using System;
using System.Collections;
using System.Collections.Generic;

// Static class wrapper for a single/central instance of a GlobeConfig.

public static class GlobeCentralConfig
{
    private static GlobeConfig configData;

    static GlobeCentralConfig()
    {
        configData = new GlobeConfig();
        configData.LoadOrCreateJSONConfig("CentralConfig.json");

        // Setup default configs if not present
        if (!GlobeCentralConfig.HasParam("MapRootDir")) GlobeCentralConfig.SetParam("MapRootDir", "C:/Util/GlobeLibrary/Maps/", false);
        if (!GlobeCentralConfig.HasParam("CaptureDir")) GlobeCentralConfig.SetParam("CaptureDir", "C:/Util/GlobeCapture/", false);
    }

    public static void SetParam(string name, string value, bool WriteOnAssign = true)
    {
        configData.SetParam(name, value, WriteOnAssign);
    }

    public static bool HasParam(string name)
    {
        return configData.HasParam(name);
    }

    public static string GetParam(string name)
    {
        return configData.GetParam(name);
    }

    public static int GetParamAsInt(string name)
    {
        string value = GetParam(name);
        if (value != null && int.TryParse(value, out int result))
        {
            return result;
        }
        else
        {
            return 0;
        }
    }

    // --------------------------------------------------------------------------------------

    public static string BoolToString(bool value)
    {
        return value ? "True" : "False";
    }

    public static bool StringToBool(string value)
    {
        return string.Equals(value.Trim(), "True", StringComparison.OrdinalIgnoreCase);
    }

}
