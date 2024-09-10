using System;
using System.Collections.Generic;

#nullable enable

// FssMap is a top level class for a map data structure.
public class FssElevationSystem
{
    // public string RootDir = "";

    private List<FssElevationTile> TileList = new List<FssElevationTile>();

    // public FssMapTileArray MapTiles;

    // --------------------------------------------------------------------------------------------
    // MARK: Get Tile
    // --------------------------------------------------------------------------------------------

    // Loop through the TileList, grabbing points from the highest resolution tile that contains the position.
    // Loop aross the points in a tile, populating the requested array.

    public FssElevationTile CreateTile(FssLLBox llBox, int latRes, int lonRes)
    {
        FssFloat2DArray data = new FssFloat2DArray(latRes, lonRes);

        double startLatDegs = llBox.MinLatDegs;
        double startLonDegs = llBox.MinLonDegs;
        double deltaLatDegs = llBox.DeltaLatDegs / latRes;
        double deltaLonDegs = llBox.DeltaLonDegs / lonRes;

        FssLLPoint pos = new FssLLPoint(0, 0);

        for (int latIdx = 0; latIdx < latRes; latIdx++)
        {
            for (int lonIdx = 0; lonIdx < lonRes; lonIdx++)
            {
                pos.LatDegs = startLatDegs + latIdx * deltaLatDegs;
                pos.LonDegs = startLonDegs + lonIdx * deltaLonDegs;

                GetElevationAtPosition(pos);

                data.SetValue(latIdx, lonIdx, (float)(lat + lon));
            }
        }
    }

    public float ElevationAtPos(FssLLPoint pos)
    {
        // Loop through the TileList, grabbing points from the highest resolution tile that contains the position.
        // Loop aross the points in a tile, populating the requested array.

        foreach (FssElevationTile tile in TileList)
        {
            if (tile.Contains(pos))
                return tile.ElevationAtPos(pos);
        }

        return 0;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Sort Tile List
    // --------------------------------------------------------------------------------------------

    // Sort the TileList from highest resolution to lowest, as returned by the TileRes function.

    public void SortTileList()
    {
        TileList.Sort((a, b) => a.TileRes().CompareTo(b.TileRes()));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Load / Save Arc ASCII Files
    // --------------------------------------------------------------------------------------------

    public static FssElevationTile? ArcASCIIToTile(string filename, FssLLBox llBox)
    {
        // Check the file exists
        if (!System.IO.File.Exists(filename)) return null;

        // Read the file
        FssFloat2DArray data = FssFloat2DArrayIO.LoadFromArcASIIGridFile(filename);

        // Create the tile
        FssElevationTile newTile = new FssElevationTile() { ElevationData = data, LLBox = llBox };

        return newTile;
    }

    // // --------------------------------------------------------------------------------------------
    // // Constants
    // // --------------------------------------------------------------------------------------------

    // public static string[] ConstDataTypes  = { "Ele", "SatImg" };
    // public static string[] ConstPathPerLvl = { "L0_30x30Degs/", "L1_5x5Degs/", "L2_1x1Degs/", "L3_0p2x0p2Degs/", "L4_0p04x0p04Degs/" };

    // // --------------------------------------------------------------------------------------------

    // public FssElevationSystem()
    // {
    //     MapTiles = new FssMapTileArray(FssLLBox.GlobalBox, 0, new FssFloat2DArray(10, 10));
    // }

    // // --------------------------------------------------------------------------------------------

    // public void SetRootDir(string rootDir)
    // {
    //     // Check the root dir is valid
    //     if (string.IsNullOrEmpty(rootDir)) return;
    //     if (!System.IO.Directory.Exists(rootDir)) return;

    //     RootDir = rootDir;
    // }

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