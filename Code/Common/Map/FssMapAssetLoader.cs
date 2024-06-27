using System;

using Godot;

public static class FssMapAssetLoader
{
    public static string[] ConstDataTypes  = { "Ele", "SatImg" };
    public static string[] ConstPathPerLvl = { "L0_30x30Degs/", "L1_5x5Degs/", "L2_1x1Degs/", "L3_0p2x0p2Degs/", "L4_0p04x0p04Degs/" };
    
    public static void LoadElevationData(string rootDir, FssMapTileCode tileCode)
    {
    }

    // public static Texture2D LoadMapTileTexture(string rootDir, FssMapTileCode tileCode)
    // {
    // }
}
