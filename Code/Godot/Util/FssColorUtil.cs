using Godot;

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

}