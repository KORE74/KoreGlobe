using System;
using System.Collections.Generic;

// FssMap is a top level class for a map data structure.
public class FssMap
{
    public string RootDir = "";

    public FssMapTileArray MapTiles;

    // --------------------------------------------------------------------------------------------
    // Constants
    // --------------------------------------------------------------------------------------------

    public static string[] ConstDataTypes  = { "Ele", "SatImg" };
    public static string[] ConstPathPerLvl = { "L0_30x30Degs/", "L1_5x5Degs/", "L2_1x1Degs/", "L3_0p2x0p2Degs/", "L4_0p04x0p04Degs/" };

    // --------------------------------------------------------------------------------------------

    public FssMap()
    {
        MapTiles = new FssMapTileArray(FssLLBox.GlobalBox, 0, new FssFloat2DArray(10, 10));
    }

    // --------------------------------------------------------------------------------------------

    public void SetRootDir(string rootDir)
    {
        // Check the root dir is valid
        if (string.IsNullOrEmpty(rootDir)) return;
        if (!System.IO.Directory.Exists(rootDir)) return;

        RootDir = rootDir;
    }

    // --------------------------------------------------------------------------------------------


    /*
    public void CreateTile(string tilecode, int lonRes)
    {
        FssMapTileCode tile = new FssMapTileCode(tileCode);

        // Create the tile directory
        string tileDir = System.IO.Path.Combine(RootDir, tile.FullCode);
        if (!System.IO.Directory.Exists(tileDir))
            System.IO.Directory.CreateDirectory(tileDir);

        // Create the tile data directories
        foreach (string currType in ConstDataTypes)
        {
            string dataTypePath = System.IO.Path.Combine(tileDir, currType);
            if (!System.IO.Directory.Exists(dataTypePath))
                System.IO.Directory.CreateDirectory(dataTypePath);
        }
    }


    */

}