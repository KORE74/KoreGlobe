using System;

// Static class to convert color structs

public static class GloColorIO
{
    // --------------------------------------------------------------------------------------------
    // MARK: Byte Util
    // --------------------------------------------------------------------------------------------

    // 0f = 0, 1f = 255, everything else is clamped to 0-255
    public static byte FloatToByte(float value)
    {
        if (value < 0f) return 0;
        if (value > 1f) return 255;
        return (byte)(value * 255);
    }

    public static float ByteToFloat(byte value)
    {
        return ((float)value) / 255f;
    }

    public static float LimitFloat(float value)
    {
        if (value < 0f) return 0f;
        if (value > 1f) return 1f;
        return value;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: To String
    // --------------------------------------------------------------------------------------------

    public static string RBGtoDecimalString(GloColorRGB rgb)
    {
        return $"R:{rgb.R:F2}, G:{rgb.G:F2}, B:{rgb.B:F2}, A:{rgb.A:F2}";
    }

    public static string HSVtoDecimalString(GloColorHSV hsv)
    {
        return $"H:{hsv.H:F2}, S:{hsv.S:F2}, V:{hsv.V:F2}, A:{hsv.A:F2}";
    }

    // ---------------------------------------------------------------------------------------------

    public static string RBGtoHexString(GloColorRGB rgb)
    {
        byte byteR = FloatToByte(rgb.R);
        byte byteG = FloatToByte(rgb.G);
        byte byteB = FloatToByte(rgb.B);
        byte byteA = FloatToByte(rgb.A);

        return $"#{byteR:X2}{byteG:X2}{byteB:X2}{byteA:X2}";
    }

    public static string RBGtoHexStringNoAlpha(GloColorRGB rgb)
    {
        return $"#{rgb.R:X2}{rgb.G:X2}{rgb.B:X2}";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: From String
    // --------------------------------------------------------------------------------------------

    // Usage: bool validStr = GloColorIO.IsValidRGBString("#112233");
    public static bool IsValidRGBString(string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;

        // Trim any whitespace or hash characters
        str = str.Trim().ToUpperInvariant();
        if (str.StartsWith("#"))
            str = str.Substring(1);

        if (str.Length != 6 && str.Length != 8)
            return false;

        // If any characters are not hex, return false
        foreach (char c in str)
        {
            if (!Uri.IsHexDigit(c))
                return false;
        }

        // No criteria to fail, return true
        return true;
    }

    // --------------------------------------------------------------------------------------------

    // An input like "#112233" or "#112233FF". Returns the zero color on any uncertainty.
    public static GloColorRGB HexStringToRGB(string hexString)
    {
        // Trim any whitespace or hash characters
        hexString = hexString.Trim().ToUpperInvariant();
        if (hexString.StartsWith("#"))
            hexString = hexString.Substring(1);

        // Check we have a 6 or 8 character string
        bool hasAlpha = (hexString.Length == 8);
        if (hexString.Length != 6 && hexString.Length != 8)
            return GloColorRGB.Zero;

        byte r = byte.Parse(hexString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hexString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hexString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        byte a = 255;
        if (hasAlpha)
            a = byte.Parse(hexString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

        return new GloColorRGB(r, g, b, a);
    }

}

