using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

// Patches: Ad hoc elevation data in lat long boxes
// Tiles: Elevation data in strict lat long boxes for tile codes and at specific resolution

public static class FssElevationConsts
{
    // FssElevationConsts.InvalidEle
    public static float InvalidEle      = -9999f;
    public static float InvalidEleCheck = -9990f; // For checking < or > comparisons
}
