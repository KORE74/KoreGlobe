using System;

// FssValueUtils: A static class for common (bool) value routines, useful as helper routines for higher-level functionality.

public static partial class FssValueUtils
{
    public static string BoolToStr(bool val)
    {
        if (val) return "True";
        return "False";
    }

    public static bool StrToBool(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return false;
    
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
                return false;
        }
    }

}
