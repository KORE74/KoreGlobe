using Godot;
using System.Collections.Generic;

public static class FssColorUtil
{
    // --------------------------------------------------------------------------------------------
    // #MARK: Color manipulation functions
    // --------------------------------------------------------------------------------------------

    // Function to output a new color with a random noise factor on each RGB channel

    // FssColorUtil.ColorWithRGBNoise(new Color(0.5f, 0.5f, 0.5f), 0.1f);

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
    // #MARK: Internal static list of named colors
    // --------------------------------------------------------------------------------------------

    // Usage: FssColorUtil.Colors["OffYellow"];

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
        { "DarkGray",    new Color(0.25f, 0.25f, 0.25f) },
        { "Gray",        new Color(0.5f, 0.5f, 0.5f) },
        { "LightGray",   new Color(0.75f, 0.75f, 0.75f) },
        { "OffWhite",    new Color(0.9f, 0.9f, 0.9f) },
        { "White",       new Color(1.0f, 1.0f, 1.0f) },

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


}