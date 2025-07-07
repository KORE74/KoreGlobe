using System;
using System.Globalization;

public static class GloStringDictionaryOperations
{
    // --------------------------------------------------------------------------------------------
    // MARK: Boolean
    // --------------------------------------------------------------------------------------------

    public static void WriteBool(GloStringDictionary dict, string key, bool value)
    {
        dict.Set(key, GloValueUtils.BoolToStr(value));
    }

    public static bool ReadBool(GloStringDictionary dict, string key, bool fallback = false)
    {
        return GloValueUtils.StrToBool(dict.Get(key), fallback);
    }

    public static bool ConsumeBool(GloStringDictionary dict, string key, out bool value)
    {
        if (!dict.Has(key))
        {
            value = false;
            return false;
        }

        value = GloValueUtils.StrToBool(dict.Get(key));
        return true;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Int
    // --------------------------------------------------------------------------------------------

    public static void WriteInt(GloStringDictionary dict, string key, int value)
    {
        dict.Set(key, value.ToString(CultureInfo.InvariantCulture));
    }

    public static int ReadInt(GloStringDictionary dict, string key, int fallback = -1)
    {
        var raw = dict.Get(key);
        return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : fallback;
    }

    public static bool ConsumeInt(GloStringDictionary dict, string key, out int value)
    {
        var raw = dict.Get(key);
        if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
        {
            dict.Remove(key);
            return true;
        }

        value = -1;
        return false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Float
    // --------------------------------------------------------------------------------------------

    public static void WriteFloat(GloStringDictionary dict, string key, float value, int sigFig = 4)
    {
        string format = $"F{sigFig}";
        dict.Set(key, value.ToString(format, CultureInfo.InvariantCulture));
    }

    public static float ReadFloat(GloStringDictionary dict, string key, float fallback = -1f)
    {
        var raw = dict.Get(key);
        return float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : fallback;
    }

    public static bool ConsumeFloat(GloStringDictionary dict, string key, out float value)
    {
        var raw = dict.Get(key);
        if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            dict.Remove(key);
            return true;
        }

        value = -1f;
        return false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Double
    // --------------------------------------------------------------------------------------------

    public static void WriteDouble(GloStringDictionary dict, string key, double value, int sigFig = 7)
    {
        string format = $"F{sigFig}";
        dict.Set(key, value.ToString(format, CultureInfo.InvariantCulture));
    }

    public static double ReadDouble(GloStringDictionary dict, string key, double fallback = -1)
    {
        var raw = dict.Get(key);
        return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : fallback;
    }

    public static bool ConsumeDouble(GloStringDictionary dict, string key, out double value)
    {
        var raw = dict.Get(key);
        if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            dict.Remove(key);
            return true;
        }

        value = -1;
        return false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Colour
    // --------------------------------------------------------------------------------------------

    public static void WriteColour(GloStringDictionary dict, string key, GloColorRGB value)
    {
        dict.Set(key, GloColorIO.RBGtoHexString(value));
    }

    public static GloColorRGB ReadColour(GloStringDictionary dict, string key, GloColorRGB fallback)
    {
        var raw = dict.Get(key);

        if (!GloColorIO.IsValidRGBString(raw)) return fallback;

        return GloColorIO.HexStringToRGB(raw);
    }

    public static bool ConsumeColour(GloStringDictionary dict, string key, out GloColorRGB value)
    {
        var raw = dict.Get(key);
        if (GloColorIO.IsValidRGBString(raw))
        {
            value = GloColorIO.HexStringToRGB(raw);
            dict.Remove(key);
            return true;
        }

        value = GloColorRGB.Zero;
        return false;
    }

}
