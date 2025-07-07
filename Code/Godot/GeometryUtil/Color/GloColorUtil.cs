using Godot;
using System.Collections.Generic;

public static class GloColorUtil
{
    // --------------------------------------------------------------------------------------------
    // MARK: Color manipulation functions
    // --------------------------------------------------------------------------------------------

    // Function to output a new color with a random noise factor on each RGB channel

    // GloColorUtil.ColorWithRGBNoise(new Color(0.5f, 0.5f, 0.5f), 0.1f);

    public static Color ColorWithRGBNoise(Color color, float fractionNoise)
    {
        float r = Mathf.Clamp(color.R + (GD.Randf() - 0.5f) * fractionNoise, 0f, 1f);
        float g = Mathf.Clamp(color.G + (GD.Randf() - 0.5f) * fractionNoise, 0f, 1f);
        float b = Mathf.Clamp(color.B + (GD.Randf() - 0.5f) * fractionNoise, 0f, 1f);
        return new Color(r, g, b, color.A);
    }

    // Function to output a new color with a random noise factor on the overall brightness of th RGB.
    public static Color ColorwithBrightnessNoise(Color color, float fractionNoise)
    {
        // Determine the adjustment multiplier
        float brightnessAdj = (GD.Randf() - 0.5f) * fractionNoise;

        // Apply the adjustment
        return new Color(
            color.R * brightnessAdj,
            color.G * brightnessAdj,
            color.B * brightnessAdj,
            color.A);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String To Color
    // --------------------------------------------------------------------------------------------

    // Take a string, and reduce it down to an index value into the GeometryColors list.
    // Allows us to select/re-select a consistent color for a defined string.

    // Usage: Color colObject = GloColorUtil.StringToColor("ObjName");

    public static Color StringToColor(string input, float alpha = 1f)
    {
        // Loop through each char in the input string, summing them up and modulo-ing it into an index value into the color array.
        int sum = 0;
        foreach (char c in input)
            sum += (int)c;
        int index = sum % GeometryColors.Count;

        // Get the color and apply any alpha value (default 1)
        Color retCol = GeometryColors[index];
        retCol.A = alpha;

        return retCol;
    }

    public static Color LookupColor(string colorName, float alpha = 1f)
    {
        Color retCol;

        if (Colors.TryGetValue(colorName, out retCol))
        {
            retCol.A = alpha;
            return retCol;
        }
        return Colors["Magenta"];
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Internal static list of named colors
    // --------------------------------------------------------------------------------------------

    // Usage: GloColorUtil.Colors["Black"];

    public static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>
    {
        // Primary colors
        { "Red",         new Color(1.0f, 0.0f, 0.0f) },
        { "Green",       new Color(0.0f, 1.0f, 0.0f) },
        { "Blue",        new Color(0.0f, 0.0f, 1.0f) },
        { "Yellow",      new Color(1.0f, 1.0f, 0.0f) },
        { "Magenta",     new Color(1.0f, 0.0f, 1.0f) },
        { "Cyan",        new Color(0.0f, 1.0f, 1.0f) },

        // Monochrome colors
        { "Black",       new Color(0.0f, 0.0f, 0.0f) },
        { "DarkGrey",    new Color(0.25f, 0.25f, 0.25f) },
        { "Grey",        new Color(0.5f, 0.5f, 0.5f) },
        { "LightGrey",   new Color(0.75f, 0.75f, 0.75f) },
        { "OffWhite",    new Color(0.9f, 0.9f, 0.9f) },
        { "White",       new Color(1.0f, 1.0f, 1.0f) },

        { "Grey05pct",   new Color(0.05f, 0.05f, 0.05f) },
        { "Grey10pct",   new Color(0.10f, 0.10f, 0.10f) },
        { "Grey15pct",   new Color(0.15f, 0.15f, 0.15f) },
        { "Grey20pct",   new Color(0.20f, 0.20f, 0.20f) },
        { "Grey25pct",   new Color(0.25f, 0.25f, 0.25f) },

        // Secondary colors
        { "Navy",        new Color(0.0f, 0.0f, 0.5f) },
        { "Teal",        new Color(0.0f, 0.5f, 0.5f) },
        { "Azure",       new Color(0.0f, 0.5f, 1.0f) },
        { "SpringGreen", new Color(0.0f, 1.0f, 0.5f) },
        { "Maroon",      new Color(0.5f, 0.0f, 0.0f) },
        { "Purple",      new Color(0.5f, 0.0f, 0.5f) },
        { "Violet",      new Color(0.5f, 0.0f, 1.0f) },
        { "Chartreuse",  new Color(0.5f, 1.0f, 0.0f) },
        { "Olive",       new Color(0.5f, 0.5f, 0.0f) },
        { "Orange",      new Color(1.0f, 0.5f, 0.0f) },
        { "Rose",        new Color(1.0f, 0.0f, 0.5f) },

        // Light Mid and Dark variations on colors
        { "LightRed",    new Color(1.0f, 0.5f, 0.5f) },
        { "MidRed",      new Color(0.5f, 0.0f, 0.0f) },
        { "DarkRed",     new Color(0.25f, 0.0f, 0.0f) },
        { "LightBlue",   new Color(0.5f, 0.5f, 1.0f) },
        { "MidBlue",     new Color(0.0f, 0.0f, 0.5f) },
        { "DarkBlue",    new Color(0.0f, 0.0f, 0.25f) },
        { "LightGreen",  new Color(0.5f, 1.0f, 0.5f) },
        { "MidGreen",    new Color(0.0f,  0.5f, 0.0f) },
        { "DarkGreen",   new Color(0.0f, 0.25f, 0,0f) },

        { "LightYellow", new Color(1.0f, 1.0f, 0.5f) },
        { "MidYellow",   new Color(0.5f, 0.5f, 0.0f) },
        { "DarkYellow",  new Color(0.25f, 0.25f, 0.0f) },
        { "LightCyan",   new Color(0.5f, 1.0f, 1.0f) },
        { "MidCyan",     new Color(0.0f, 0.5f, 0.5f) },
        { "DarkCyan",    new Color(0.0f, 0.25f, 0.25f) },
        { "LightMagenta",new Color(1.0f, 0.5f, 1.0f) },
        { "MidMagenta",  new Color(0.5f, 0.0f, 0.5f) },
        { "DarkMagenta", new Color(0.25f, 0.0f, 0.25f) },

        // Other notable colors
        { "Brown",       new Color(0.6f, 0.4f, 0.2f) },
        { "Pink",        new Color(1.0f, 0.75f, 0.8f) },
        { "Lime",        new Color(0.75f, 1.0f, 0.0f) },
        { "Indigo",      new Color(0.29f, 0.0f, 0.51f) },
        { "Gold",        new Color(1.0f, 0.84f, 0.0f) },
        { "OffYellow",   new Color(0.8f, 0.8f, 0.3f) },
        { "OffGreen",    new Color(0.3f, 0.8f, 0.3f) },
        { "OffBlue",     new Color(0.3f, 0.3f, 0.9f) },
        { "OffRed",      new Color(0.8f, 0.3f, 0.3f) },
        { "OffPurple",   new Color(0.8f, 0.3f, 0.8f) },
        { "OffCyan",     new Color(0.3f, 0.8f, 0.8f) },
        { "OffOrange",   new Color(0.8f, 0.5f, 0.3f) },
        { "OffPink",     new Color(0.8f, 0.5f, 0.5f) },
        { "OffBrown",    new Color(0.8f, 0.6f, 0.4f) },
        { "OffLime",     new Color(0.5f, 0.8f, 0.3f) },
        { "OffIndigo",   new Color(0.3f, 0.0f, 0.5f) },
        { "OffGold",     new Color(0.8f, 0.7f, 0.0f) }
    };

    public static readonly List<Color> GeometryColors = new List<Color>
    {
        Colors["MidRed"],
        Colors["MidBlue"],
        Colors["MidYellow"],
        Colors["DarkMagenta"],
        Colors["MidCyan"],
        Colors["DarkCyan"],
        Colors["Brown"],
        Colors["OffBlue"],
        Colors["OffRed"],
        Colors["OffPurple"],
        Colors["OffCyan"],
        Colors["OffOrange"],
        Colors["OffPink"],
        Colors["OffBrown"],
        Colors["OffIndigo"],
        Colors["Olive"],
        Colors["Maroon"]
    };
}