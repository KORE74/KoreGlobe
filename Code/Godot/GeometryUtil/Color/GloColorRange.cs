

// GloColorRange - A class to contain a list of numeric values associated colors, then provide interpolation between them from an input value.

using Godot;
using System;
using System.Collections.Generic;

// ------------------------------------------------------------------------------------------------

struct ColorRangeEntry
{
    public float value;
    public Color color;

    public ColorRangeEntry(float value, Color color)
    {
        this.value = value;
        this.color = color;
    }
}

// ------------------------------------------------------------------------------------------------

public class GloColorRange
{

    List<ColorRangeEntry> colorRangeList = new List<ColorRangeEntry>();

    public void AddEntry(float value, Color color)
    {
        colorRangeList.Add(new ColorRangeEntry(value, color));

        // Sort the list by value
        colorRangeList.Sort((a, b) => a.value.CompareTo(b.value));
    }

    public Color GetColor(float value)
    {
        // Return WHite if no entries
        if (colorRangeList.Count == 0)
        {
            return Colors.Magenta;
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

        // Interpolate between the two values
        for (int i = 0; i < colorRangeList.Count - 1; i++)
        {
            if (value >= colorRangeList[i].value && value <= colorRangeList[i + 1].value)
            {
                float fraction = (value - colorRangeList[i].value) / (colorRangeList[i + 1].value - colorRangeList[i].value);
                return colorRangeList[i].color.Lerp(colorRangeList[i + 1].color, fraction);
            }
        }

        // Should never get here - return a default color
        return Colors.Magenta;
    }

    public void Clear()
    {
        colorRangeList.Clear();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Standard color ranges
    // --------------------------------------------------------------------------------------------

    public static GloColorRange RedToGreen()
    {
        GloColorRange colorRange = new GloColorRange();
        colorRange.AddEntry(0f, Colors.Red);
        colorRange.AddEntry(1f, Colors.Green);
        return colorRange;
    }

    public static GloColorRange RedYellowGreen()
    {
        GloColorRange colorRange = new GloColorRange();
        colorRange.AddEntry(0.0f, Colors.Red);
        colorRange.AddEntry(0.5f, Colors.Yellow);
        colorRange.AddEntry(1.0f, Colors.Green);
        return colorRange;
    }

    public static GloColorRange BlueGreenYellowOrangeRed()
    {
        GloColorRange colorRange = new GloColorRange();
        colorRange.AddEntry(0.00f, Colors.Blue);
        colorRange.AddEntry(0.25f, Colors.Green);
        colorRange.AddEntry(0.50f, Colors.Yellow);
        colorRange.AddEntry(0.75f, Colors.Orange);
        colorRange.AddEntry(1.00f, Colors.Red);
        return colorRange;
    }

}
