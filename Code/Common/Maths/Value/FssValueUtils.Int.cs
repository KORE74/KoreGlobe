using System;

// FssValueUtils: A static class for common (double precision) math routines, useful as helper routines for higher-level functionality.

public static partial class FssValueUtils
{
    // FssValueUtils.Clamp

    static public int Clamp(int val, int min, int max)
    {
        return (val < min) ? min : (val > max) ? max : val;
    }

    static public int Wrap(int val, int min, int max)
    {
        int range = max - min + 1;
        if (val < min)
        {
            val += range * ((min - val) / range + 1);
        }
        return min + (val - min) % range;
    }

}
