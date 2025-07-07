using System.Collections.Generic;

// GloColorPalette
// This class is used to create a color palette for common colours

// GloColorPalette.Colors["Magenta"]
public static class GloColorPalette
{
    // --------------------------------------------------------------------------------------------
    // MARK: GloColorRGB Palette
    // --------------------------------------------------------------------------------------------

    public static readonly Dictionary<string, GloColorRGB> Colors = new Dictionary<string, GloColorRGB>
    {
        // Primary colors (1s & 0s)
        { "Red",         new GloColorRGB(1.0f, 0.0f, 0.0f) },
        { "Green",       new GloColorRGB(0.0f, 1.0f, 0.0f) },
        { "Blue",        new GloColorRGB(0.0f, 0.0f, 1.0f) },
        { "Yellow",      new GloColorRGB(1.0f, 1.0f, 0.0f) },
        { "Magenta",     new GloColorRGB(1.0f, 0.0f, 1.0f) },
        { "Cyan",        new GloColorRGB(0.0f, 1.0f, 1.0f) },

        // Monochrome colors
        { "Black",       new GloColorRGB(0.0f,  0.0f,  0.0f) },
        { "DarkGrey",    new GloColorRGB(0.25f, 0.25f, 0.25f) },
        { "Grey",        new GloColorRGB(0.5f,  0.5f,  0.5f) },
        { "LightGrey",   new GloColorRGB(0.75f, 0.75f, 0.75f) },
        { "OffWhite",    new GloColorRGB(0.9f,  0.9f,  0.9f) },
        { "White",       new GloColorRGB(1.0f,  1.0f,  1.0f) },

        { "Grey05pct",   new GloColorRGB(0.05f, 0.05f, 0.05f) },
        { "Grey10pct",   new GloColorRGB(0.10f, 0.10f, 0.10f) },
        { "Grey15pct",   new GloColorRGB(0.15f, 0.15f, 0.15f) },
        { "Grey20pct",   new GloColorRGB(0.20f, 0.20f, 0.20f) },
        { "Grey25pct",   new GloColorRGB(0.25f, 0.25f, 0.25f) },
        { "Grey30pct",   new GloColorRGB(0.30f, 0.30f, 0.30f) },
        { "Grey35pct",   new GloColorRGB(0.35f, 0.35f, 0.35f) },
        { "Grey40pct",   new GloColorRGB(0.40f, 0.40f, 0.40f) },
        { "Grey45pct",   new GloColorRGB(0.45f, 0.45f, 0.45f) },
        { "Grey50pct",   new GloColorRGB(0.50f, 0.50f, 0.50f) },
        { "Grey55pct",   new GloColorRGB(0.55f, 0.55f, 0.55f) },
        { "Grey60pct",   new GloColorRGB(0.60f, 0.60f, 0.60f) },
        { "Grey65pct",   new GloColorRGB(0.65f, 0.65f, 0.65f) },
        { "Grey70pct",   new GloColorRGB(0.70f, 0.70f, 0.70f) },
        { "Grey75pct",   new GloColorRGB(0.75f, 0.75f, 0.75f) },
        { "Grey80pct",   new GloColorRGB(0.80f, 0.80f, 0.80f) },
        { "Grey85pct",   new GloColorRGB(0.85f, 0.85f, 0.85f) },
        { "Grey90pct",   new GloColorRGB(0.90f, 0.90f, 0.90f) },
        { "Grey95pct",   new GloColorRGB(0.95f, 0.95f, 0.95f) },

        // Light color pastels
        { "LightRed",      new GloColorRGB(1.0f, 0.7f, 0.7f) },
        { "LightGreen",    new GloColorRGB(0.7f, 1.0f, 0.7f) },
        { "LightBlue",     new GloColorRGB(0.7f, 0.7f, 1.0f) },
        { "LightYellow",   new GloColorRGB(1.0f, 1.0f, 0.7f) },
        { "LightCyan",     new GloColorRGB(0.7f, 1.0f, 1.0f) },
        { "LightMagenta",  new GloColorRGB(1.0f, 0.7f, 1.0f) },
        { "LightOrange",   new GloColorRGB(1.0f, 0.85f, 0.7f) },
        { "LightPurple",   new GloColorRGB(0.85f, 0.7f, 1.0f) },

        // Dark versions
        { "DarkRed",     new GloColorRGB(0.25f, 0.0f, 0.0f) },
        { "DarkGreen",   new GloColorRGB(0.0f, 0.25f, 0.0f) },
        { "DarkBlue",    new GloColorRGB(0.0f, 0.0f, 0.25f) },
        { "DarkYellow",  new GloColorRGB(0.25f, 0.25f, 0.0f) },
        { "DarkCyan",    new GloColorRGB(0.0f, 0.25f, 0.25f) },
        { "DarkMagenta", new GloColorRGB(0.25f, 0.0f, 0.25f) },

        // Secondary and midtone colors (0s, 0.5s, 1s)
        { "Navy",        new GloColorRGB(0.0f, 0.0f, 0.5f) },
        { "MidGreen",    new GloColorRGB(0.0f, 0.5f, 0.0f) },
        { "Teal",        new GloColorRGB(0.0f, 0.5f, 0.5f) },
        { "Azure",       new GloColorRGB(0.0f, 0.5f, 1.0f) },
        { "SpringGreen", new GloColorRGB(0.0f, 1.0f, 0.5f) },
        { "Maroon",      new GloColorRGB(0.5f, 0.0f, 0.0f) },
        { "Purple",      new GloColorRGB(0.5f, 0.0f, 0.5f) },
        { "Violet",      new GloColorRGB(0.5f, 0.0f, 1.0f) },
        { "Olive",       new GloColorRGB(0.5f, 0.5f, 0.0f) },
        { "MidBlue",     new GloColorRGB(0.5f, 0.5f, 1.0f) },
        { "Chartreuse",  new GloColorRGB(0.5f, 1.0f, 0.0f) },
        { "MidCyan",     new GloColorRGB(0.5f, 1.0f, 1.0f) },
        { "Rose",        new GloColorRGB(1.0f, 0.0f, 0.5f) },
        { "Orange",      new GloColorRGB(1.0f, 0.5f, 0.0f) },

        // Notable named colors
        { "Brown",       new GloColorRGB(0.6f, 0.4f, 0.2f) },
        { "Pink",        new GloColorRGB(1.0f, 0.75f, 0.8f) },
        { "Lime",        new GloColorRGB(0.75f, 1.0f, 0.0f) },
        { "Indigo",      new GloColorRGB(0.29f, 0.0f, 0.51f) },
        { "Gold",        new GloColorRGB(1.0f, 0.84f, 0.0f) },

        // Off/Muted colors
        { "MutedYellow", new GloColorRGB(0.8f, 0.8f, 0.3f) },
        { "MutedGreen",  new GloColorRGB(0.3f, 0.8f, 0.3f) },
        { "MutedBlue",   new GloColorRGB(0.3f, 0.3f, 0.9f) },
        { "MutedRed",    new GloColorRGB(0.8f, 0.3f, 0.3f) },
        { "MutedPurple", new GloColorRGB(0.8f, 0.3f, 0.8f) },
        { "MutedCyan",   new GloColorRGB(0.3f, 0.8f, 0.8f) },
        { "MutedOrange", new GloColorRGB(0.8f, 0.5f, 0.3f) },
        { "DustyPink",   new GloColorRGB(0.8f, 0.5f, 0.5f) },
        { "MutedBrown",  new GloColorRGB(0.8f, 0.6f, 0.4f) },
        { "MutedLime",   new GloColorRGB(0.5f, 0.8f, 0.3f) },
        { "MutedIndigo", new GloColorRGB(0.3f, 0.0f, 0.5f) },
        { "MutedGold",   new GloColorRGB(0.8f, 0.7f, 0.0f) }
    };
}
