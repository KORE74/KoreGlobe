

// GloColorRange - A class to contain a list of numeric values associated colors, then provide interpolation between them from an input value.

using System;
using System.Collections.Generic;

// ------------------------------------------------------------------------------------------------

struct GloColorRangeEntry
{
    public float       value;
    public GloColorRGB color;

    public GloColorRangeEntry(float value, GloColorRGB color)
    {
        this.value = value;
        this.color = color;
    }
}

// ------------------------------------------------------------------------------------------------

public class GloColorRange2
{
    List<GloColorRangeEntry> colorRangeList = new List<GloColorRangeEntry>();

    // --------------------------------------------------------------------------------------------
    // MARK: Add
    // --------------------------------------------------------------------------------------------

    public void AddEntry(float value, GloColorRGB color)
    {
        colorRangeList.Add(new GloColorRangeEntry(value, color));

        // Sort the list by value
        colorRangeList.Sort((a, b) => a.value.CompareTo(b.value));
    }

    public void Clear()
    {
        colorRangeList.Clear();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Get
    // --------------------------------------------------------------------------------------------

    public GloColorRGB GetColor(float value)
    {
        // Return WHite if no entries
        if (colorRangeList.Count == 0)
        {
            return GloColorPalette.Colors["Magenta"];
        }

        // Return the first color if the value is less than the first entry
        if (value <= colorRangeList[0].value)
        {
            return colorRangeList[0].color;
        }

        // Return the last color if the value is greater than the last entry
        if (value >= colorRangeList[colorRangeList.Count - 1].value)
        {
            return colorRangeList[colorRangeList.Count - 1].color;
        }

        // Interpolate between the two values in the list (different to the lerp between the two fractions once they're found)
        for (int i = 0; i < colorRangeList.Count - 1; i++)
        {
            if (value >= colorRangeList[i].value && value <= colorRangeList[i + 1].value)
            {
                // Find the fraction between the two values, then lerp the colours
                float fraction = (value - colorRangeList[i].value) / (colorRangeList[i + 1].value - colorRangeList[i].value);
                return GloColorRGB.Lerp(colorRangeList[i].color, colorRangeList[i + 1].color, fraction);
            }
        }

        // Should never get here - return a default color
        return GloColorPalette.Colors["Magenta"];
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Standard color ranges
    // --------------------------------------------------------------------------------------------

    public static GloColorRange2 RedToGreen()
    {
        GloColorRange2 colorRange = new GloColorRange2();
        colorRange.AddEntry(0f, GloColorPalette.Colors["Red"]);
        colorRange.AddEntry(1f, GloColorPalette.Colors["Green"]);
        return colorRange;
    }

    public static GloColorRange2 RedYellowGreen()
    {
        GloColorRange2 colorRange = new GloColorRange2();
        colorRange.AddEntry(0.0f, GloColorPalette.Colors["Red"]);
        colorRange.AddEntry(0.5f, GloColorPalette.Colors["Yellow"]);
        colorRange.AddEntry(1.0f, GloColorPalette.Colors["Green"]);
        return colorRange;
    }

    public static GloColorRange2 BlueGreenYellowOrangeRed()
    {
        GloColorRange2 colorRange = new GloColorRange2();
        colorRange.AddEntry(0.00f, GloColorPalette.Colors["Blue"]);
        colorRange.AddEntry(0.25f, GloColorPalette.Colors["Green"]);
        colorRange.AddEntry(0.50f, GloColorPalette.Colors["Yellow"]);
        colorRange.AddEntry(0.75f, GloColorPalette.Colors["Orange"]);
        colorRange.AddEntry(1.00f, GloColorPalette.Colors["Red"]);
        return colorRange;
    }

}
