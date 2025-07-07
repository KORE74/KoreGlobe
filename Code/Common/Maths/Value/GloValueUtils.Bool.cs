using System;

// GloValueUtils: A static class for common (bool) value routines, useful as helper routines for higher-level functionality.

public static partial class GloValueUtils
{
    // Usage: GloValueUtils.BoolToStr
    public static string BoolToStr(bool val)
    {
        if (val) return "True";
        return "False";
    }

    // Usage: GloValueUtils.StrToBool
    public static bool StrToBool(string str, bool fallback = false)
    {
        if (string.IsNullOrWhiteSpace(str))
            return fallback;

        string trimmedStr = str.Trim().ToLowerInvariant();

        switch (trimmedStr)
        {
            case "true":
            case "t":
            case "yes":
            case "y":
                return true;
            case "false":
            case "f":
            case "no":
            case "n":
                return false;
            default:
                return fallback;
        }
    }
}
